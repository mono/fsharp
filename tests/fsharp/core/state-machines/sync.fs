
module Tests.SyncBuilder

open System
open FSharp.Core.CompilerServices.StateMachineHelpers

[<AbstractClass>]
type SyncMachine<'T>() =
    
    abstract Step : unit -> 'T

    member this.Start() = this.Step()

type SyncCode<'T> = unit -> 'T

type SyncBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f: unit -> SyncCode<'T>) : SyncCode<'T> = (fun () -> __expand_f () ())

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : SyncCode<'T>) : 'T = 
        if __generateCompiledStateMachines then
            (__compiledStateMachine
                { new SyncMachine<'T>() with 
                    member __.Step ()  = __compiledStateMachineCode 0 (__expand_code ()) }).Start()
        else
            let sm = 
                { new SyncMachine<'T>() with 
                    member sm.Step () = 
                        __expand_code () }
            sm.Start()

    [<NoDynamicInvocation>]
    member inline __.Zero() : SyncCode<unit> = 
        (fun () -> ())

    [<NoDynamicInvocation>]
    member inline __.Return (x: 'T) : SyncCode<'T> =
        (fun () -> x)

    [<NoDynamicInvocation>]
    member inline __.Combine(__expand_code1: SyncCode<unit>, __expand_code2: SyncCode<'T>) : SyncCode<'T> =
        (fun () -> 
            __expand_code1()
            __expand_code2())

    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : SyncCode<unit>) : SyncCode<unit> =
        (fun () -> 
            while __expand_condition() do
                __expand_body ())

    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : SyncCode<'T>, __expand_catch : exn -> 'T) : SyncCode<'T> =
        (fun () -> 
            try
                __expand_body ()
            with exn -> 
                __expand_catch exn)

    [<NoDynamicInvocation>]
    member inline __.TryFinally(__expand_body: SyncCode<'T>, compensation : unit -> unit) : SyncCode<'T> =
        (fun () -> 
            let ``__machine_step$cont`` = 
                try
                    __expand_body ()
                with _ ->
                    compensation()
                    reraise()
            compensation()
            ``__machine_step$cont``)

    [<NoDynamicInvocation>]
    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> SyncCode<'T>) : SyncCode<'T> = 
        this.TryFinally(
            (fun () -> __expand_body disp ()),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    [<NoDynamicInvocation>]
    member inline this.For(sequence : seq<'T>, __expand_body : 'T -> SyncCode<unit>) : SyncCode<unit> =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current ()))))

    [<NoDynamicInvocation>]
    member inline __.ReturnFrom (value: 'T) : SyncCode<'T> =
        (fun () -> 
             value)

(*
    [<NoDynamicInvocation>]
    member inline __.Bind (__expand_code1: SyncCode<'TResult1>, __expand_continuation: 'TResult1 -> SyncCode<'TResult2>) : SyncCode<'TResult2> =
        (fun () -> 
             let ``__machine_step$cont`` = __expand_code1 ()
             __expand_continuation ``__machine_step$cont`` ())
*)
    [<NoDynamicInvocation>]
    member inline __.Bind (v: 'TResult1, __expand_continuation: 'TResult1 -> SyncCode<'TResult2>) : SyncCode<'TResult2> =
        (fun () -> 
             __expand_continuation v ())

let sync = SyncBuilder()

module Examples =

    let t1 y = 
        sync {
           printfn "in t1"
           let x = 4 + 5 + y
           return x
        }

    let t2 y = 
        sync {
           printfn "in t2"
           let! x = t1 y
           return x + y
        }


    printfn "t2 6 = %d" (t2 6)