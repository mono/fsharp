// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Collections.Array3D module

namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections

open System
open FSharp.Core.UnitTests.LibraryTestFx
open NUnit.Framework

(*
[Test Strategy]
Make sure each method works on:
* Integer Array3D (value type)
* String  Array3D (reference type)
* Empty   Array3D (0 elements)
* Null    Array3D (null)
*)


[<TestFixture>][<Category "Collections.Array">][<Category "FSharp.Core.Collections">]
type Array3Module() =

    let VerifyDimensions arr x y z =
        if Array3D.length1 arr <> x then Assert.Fail("Array3D does not have expected dimensions.")
        if Array3D.length2 arr <> y then Assert.Fail("Array3D does not have expected dimensions.")
        if Array3D.length3 arr <> z then Assert.Fail("Array3D does not have expected dimensions.")
        ()

    let array3d (arrs: 'a array array array ) = Array3D.init arrs.Length arrs.[0].Length arrs.[0].[0].Length  (fun i j k -> arrs.[i].[j].[k])



    [<Test>]
    member this.Create() =
        // integer array  
        let intArr = Array3D.create 3 4 5 168
        if intArr.[1,2,1] <> 168 then Assert.Fail()
        VerifyDimensions intArr 3 4 5
        
        // string array 
        let strArr = Array3D.create 2 3 4 "foo"
        if strArr.[1,2,3] <> "foo" then Assert.Fail()
        VerifyDimensions strArr 2 3 4
        
        // empty array     
        let eptArr1 = Array3D.create 0 0 0 'a'
        let eptArr2 = Array3D.create 0 0 0 'b'
        if eptArr1 <> eptArr2 then Assert.Fail()
        
        () 
        
    [<Test>]
    member this.Init() =
            
        // integer array  
        let intArr = Array3D.init 3 3 3 (fun i j k -> i*100 + j*10 + k)
        if intArr.[1,1,1] <> 111 then Assert.Fail()
        if intArr.[2,2,2] <> 222 then Assert.Fail()
        VerifyDimensions intArr 3 3 3
        
        // ref array 
        let strArr = Array3D.init 3 3 3 (fun i j k-> (i, j, k))
        if strArr.[2,0,1] <> (2, 0, 1) then Assert.Fail()
        if strArr.[0,1,2] <> (0, 1, 2) then Assert.Fail()
        VerifyDimensions intArr 3 3 3
        ()

    [<Test>]
    member this.Get() =
        
        // integer array  
        let intArr = Array3D.init 2 3 2 (fun i j k -> i*100 + j*10 + k)
        let resultInt = Array3D.get intArr  1 2 0
        if resultInt <> 120 then Assert.Fail()
        
        // string array 
        let strArr = Array3D.init 2 3 2 (fun i j k-> i.ToString() + "-" + j.ToString() + "-" + k.ToString())
        let resultStr = Array3D.get strArr 0 2 1
        if resultStr <> "0-2-1" then Assert.Fail()
        
        CheckThrowsIndexOutRangException(fun () -> Array3D.get strArr 2 0 0 |> ignore)
        CheckThrowsIndexOutRangException(fun () -> Array3D.get strArr 0 3 0 |> ignore)
        CheckThrowsIndexOutRangException(fun () -> Array3D.get strArr 0 0 2 |> ignore)
        
        // empty array  
        let emptyArray = Array3D.init 0 0 0 (fun i j k -> Assert.Fail())
        CheckThrowsIndexOutRangException (fun () -> Array3D.get emptyArray 1 0 0 |> ignore)
        CheckThrowsIndexOutRangException (fun () -> Array3D.get emptyArray 0 1 0 |> ignore)
        CheckThrowsIndexOutRangException (fun () -> Array3D.get emptyArray 0 0 1 |> ignore)

        // null array
        let nullArr : string[,,] = null
        CheckThrowsNullRefException (fun () -> Array3D.get nullArr 1 1 1 |> ignore)  
        ()
    
    [<Test>]
    member this.Iter() =

        // integer array  
        let intArr = Array3D.init 2 3 2 (fun i j k -> i*100 + j*10 + k)
        
        let resultInt = ref 0 
        let addToTotal x = resultInt := !resultInt + x              
        
        Array3D.iter addToTotal intArr 
        Assert.IsTrue(!resultInt = 726)
        
        // string array 
        let strArr = Array3D.init 2 3 2 (fun i j k-> i.ToString() + "-" + j.ToString() + "-" + k.ToString())
        
        let resultStr = ref ""
        let addElement (x:string) = resultStr := (!resultStr) + x + ","  

        Array3D.iter addElement strArr  
        Assert.IsTrue(!resultStr = "0-0-0,0-0-1,0-1-0,0-1-1,0-2-0,0-2-1,1-0-0,1-0-1,1-1-0,1-1-1,1-2-0,1-2-1,")
        
        // empty array
        let emptyArray = Array3D.create 0 0 0 0
        Array3D.iter (fun x -> Assert.Fail()) emptyArray
        
        // null array
        let nullArr : string[,,] = null
        CheckThrowsArgumentNullException(fun () -> Array3D.iter (fun x -> Assert.Fail("Souldn't be called")) nullArr)
        ()   

    [<Test>]
    member this.Iteri() =

        // integer array  
        let intArr = Array3D.init 2 3 2 (fun i j k -> i*100 + j*10 + k)
        let resultInt = ref 0 
        let funInt (x:int) (y:int) (z:int) (a:int) =   
            resultInt := !resultInt + x  + y + z + a         
            () 
            
        Array3D.iteri funInt intArr 
        if !resultInt <> 750 then Assert.Fail()
        
        // string array 
        let strArr = Array3D.init 2 3 2 (fun i j k-> i.ToString() + "-" + j.ToString() + "-" + k.ToString())
        let resultStr = ref ""
        let funStr (x:int) (y:int) (z:int) (a:string)=
            resultStr := (!resultStr) + "[" + x.ToString() + "," + y.ToString()+"," + z.ToString() + "]" + "=" + a + "; "  
            ()
        Array3D.iteri funStr strArr  
        if !resultStr <> "[0,0,0]=0-0-0; [0,0,1]=0-0-1; [0,1,0]=0-1-0; [0,1,1]=0-1-1; [0,2,0]=0-2-0; [0,2,1]=0-2-1; [1,0,0]=1-0-0; [1,0,1]=1-0-1; [1,1,0]=1-1-0; [1,1,1]=1-1-1; [1,2,0]=1-2-0; [1,2,1]=1-2-1; " then Assert.Fail()
        
        // empty array    
        let emptyArray = Array3D.create 0 0 0 0
        Array3D.iter (fun x -> Assert.Fail()) emptyArray
        
        // null array
        let nullArr = null:string[,,]    
        CheckThrowsArgumentNullException (fun () -> Array3D.iteri funStr nullArr |> ignore)  
        ()  

    [<Test>]
    member this.Length1() =
    
        // integer array  
        let intArr = Array3D.create 2 3 2 168
        let resultInt = Array3D.length1 intArr
        if  resultInt <> 2 then Assert.Fail()

        
        // string array 
        let strArr = Array3D.create 2 3 2 "enmity"
        let resultStr = Array3D.length1 strArr
        if resultStr <> 2 then Assert.Fail()
        
        // empty array     
        let eptArr = Array3D.create 0 0 0 1
        let resultEpt = Array3D.length1 eptArr
        if resultEpt <> 0  then Assert.Fail()

        // null array
        let nullArr = null : string[,,]    
        CheckThrowsNullRefException (fun () -> Array3D.length1 nullArr |> ignore)  
        () 

    [<Test>]
    member this.Length2() =
    
        // integer array  
        let intArr = Array3D.create 2 3 2 168
        let resultInt = Array3D.length2 intArr
        if  resultInt <> 3 then Assert.Fail()

        
        // string array 
        let strArr = Array3D.create 2 3 2 "enmity"
        let resultStr = Array3D.length2 strArr
        if resultStr <> 3 then Assert.Fail()
        
        // empty array     
        let eptArr = Array3D.create 0 0 0 1
        let resultEpt = Array3D.length2 eptArr
        if resultEpt <> 0  then Assert.Fail()

        // null array
        let nullArr = null : string[,,]    
        CheckThrowsNullRefException (fun () -> Array3D.length2 nullArr |> ignore)  
        () 

    [<Test>]
    member this.Length3() = 
    
        // integer array  
        let intArr = Array3D.create 2 3 5 168
        let resultInt = Array3D.length3 intArr
        if  resultInt <> 5 then Assert.Fail()
        
        // string array 
        let strArr = Array3D.create 2 3 5 "enmity"
        let resultStr = Array3D.length3 strArr
        if resultStr <> 5 then Assert.Fail()
        
        // empty array     
        let eptArr = Array3D.create 0 0 0 1
        let resultEpt = Array3D.length3 eptArr
        if resultEpt <> 0  then Assert.Fail()

        // null array
        let nullArr = null : string[,,]    
        CheckThrowsNullRefException (fun () -> Array3D.length3 nullArr |> ignore)  
        ()  

    [<Test>]
    member this.Map() =
        
        // integer array  
        let intArr = Array3D.create 2 3 5 168
        let funInt x = x.ToString()
        let resultInt = Array3D.map funInt intArr 
        if resultInt <> Array3D.create 2 3 5 "168" then Assert.Fail()
        
        // string array 
        let strArr = Array3D.create 2 2 2 "value"
        let funStr (x:string) = x.ToUpper()
        
        let resultStr = Array3D.map funStr strArr
        resultStr |> Array3D.iter (fun x -> if x <> "VALUE" then Assert.Fail())
        
        // empty array     
        let eptArr = Array3D.create 0 0 0 1
        let resultEpt = Array3D.map (fun x -> Assert.Fail()) eptArr

        // null array
        let nullArr = null : string[,,]    
        CheckThrowsArgumentNullException (fun () -> Array3D.map funStr nullArr |> ignore)  
        ()   

    [<Test>]
    member this.Mapi() =

        // integer array  
        let intArr = Array3D.init 2 3 2 (fun i j k -> i*100 + j*10 + k)
        let funInt x y z a = x+y+z+a
        let resultInt = Array3D.mapi funInt intArr 
        if resultInt <> (Array3D.init 2 3 2(fun i j k-> i*100 + j*10 + k + i + j + k)) then 
            Assert.Fail()

        
        // string array 
        let strArr = Array3D.init 2 3 2(fun i j k-> "goodboy")
        let funStr (x:int) (y:int) (z:int) (a:string) = x.ToString() + y.ToString() + z.ToString() + a.ToUpper()
        let resultStr = Array3D.mapi funStr strArr
        if resultStr <> Array3D.init 2 3 2(fun i j k-> i.ToString() + j.ToString() + k.ToString() + "GOODBOY") then 
            Assert.Fail()
        
        // empty array     
        let eptArr = Array3D.create 0 0 0 1
        let resultEpt = Array3D.mapi (fun i j k x -> Assert.Fail()) eptArr
        
        // null array
        let nullArr = null : string[,,]    
        CheckThrowsArgumentNullException (fun () -> Array3D.mapi (fun i j k x -> Assert.Fail("shouldn't execute this")) nullArr |> ignore)  
        () 


    [<Test>]
    member this.Set() =

        // integer array  
        let intArr = Array3D.init 2 3 2(fun i j k -> i*100 + j*10 + k)
        
        Assert.IsFalse(intArr.[1,1,1] = -1)
        Array3D.set intArr 1 1 1 -1
        Assert.IsTrue(intArr.[1,1,1] = -1)

        // string array 
        let strArr = Array3D.init 2 3 2 (fun i j k-> i.ToString() + "-" + j.ToString()+ "-" + k.ToString())
        
        Assert.IsFalse(strArr.[1,1,1] = "custom")
        Array3D.set strArr 1 1 1 "custom"
        Assert.IsTrue(strArr.[1,1,1] = "custom")

        // Out of bounds checks
        CheckThrowsIndexOutRangException(fun () -> Array3D.set strArr 2 0 0 "out of bounds")
        CheckThrowsIndexOutRangException(fun () -> Array3D.set strArr 0 3 0 "out of bounds")
        CheckThrowsIndexOutRangException(fun () -> Array3D.set strArr 0 0 2 "out of bounds")
        
        // empty array  
        let emptArr = Array3D.create 0 0 0 'z'
        CheckThrowsIndexOutRangException(fun () -> Array3D.set emptArr 0 0 0 'a')

        // null array
        let nullArr = null : string[,,]    
        CheckThrowsNullRefException (fun () -> Array3D.set  nullArr 0 0 0 "")  
        ()  

    [<Test>]
    member this.ZeroCreate() =
            
        let intArr : int[,,] = Array3D.zeroCreate 2 3 2
        if Array3D.get intArr 1 1 1 <> 0 then 
            Assert.Fail()
            
        let structArray : DateTime[,,] = Array3D.zeroCreate 1 1 1
        let defaultVal = new DateTime()
        Assert.IsTrue(Array3D.get structArray 0 0 0 = defaultVal)

        let strArr : string[,,] = Array3D.zeroCreate 2 3 2
        for i in 0 .. 1 do
            for j in 0 .. 2 do
                for k in 0 .. 1 do
                    Assert.AreEqual(null, strArr.[i, j, k])
                    
        // Test invalid values
        CheckThrowsArgumentException(fun () -> Array3D.zeroCreate -1 1 1 |> ignore)
        CheckThrowsArgumentException(fun () -> Array3D.zeroCreate 1 -1 1 |> ignore)
        CheckThrowsArgumentException(fun () -> Array3D.zeroCreate 1 1 -1 |> ignore)
        
        ()

    [<Test>]
    member this.``Slicing with reverse index in all 3 slice expr behaves as expected``()  = 
        let arr = Array3D.create 5 5 5 1

        Assert.That(arr.[..^1, ^1..^0, ^2..], Is.EquivalentTo(arr.[..3, 3..4, 2..]))

    [<Test>]
    member this.``Set slice with reverse index in all 3 slice expr behaves as expected``()  = 
        let arr1 = Array3D.create 5 5 5 1
        let arr2 = Array3D.create 5 5 5 1

        let setSlice = Array3D.create 2 2 2 0

        arr1.[^1..^0, ^2..^3, ^1..^2] <- setSlice
        arr2.[^1..^0, ^2..^3, ^1..^2] <- setSlice

        Assert.That(arr1, Is.EquivalentTo(arr2))

    [<Test>]
    member this.``Indexer with reverse index in one dim behaves as expected``() = 
        let arr1 = Array3D.init 5 5 5 (fun i j k -> i*100 + j*10 + k)
 
        Assert.That(arr1.[^1,0,0], Is.EqualTo(300))

    [<Test>]
    member this.``Indexer with reverse index in all dim behaves as expected``() = 
        let arr1 = Array3D.init 5 5 5 (fun i j k -> i*100 + j*10 + k)
 
        Assert.That(arr1.[^0,^1,^0], Is.EqualTo(434))

    [<Test>]
    member this.``Set item with reverse index in all dims behave as expected``() = 
        let arr1 = Array3D.create 5 5 5 2

        arr1.[^1,^0,^0] <- 9
        Assert.That(arr1.[3,4,4], Is.EqualTo(9))
    member this.SlicingSingleFixed1() =
        let m1 = (array3d [| 
                            [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                               [| 11.0;21.0;31.0;41.0;51.0;61.0 |]  |]
                            [| [| 10.0;20.0;30.0;40.0;50.0;60.0 |];
                               [| 100.0;200.0;300.0;400.0;500.0;600.0 |]  |] |])

        let newSlice = [|0.; 0.; 0.; 0.; 0. ; 0.;|]
        m1.[0,0,*] <- newSlice
        Assert.AreEqual(m1.[1,0,0], 10.0)
        Assert.AreEqual(m1.[0,0,*], newSlice)
        
    [<Test>]
    member this.SlicingSingleFixed2() =
        let m1 = (array3d [| 
                        [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                           [| 11.0;21.0;31.0;41.0;51.0;61.0 |]  |]
                        [| [| 10.0;20.0;30.0;40.0;50.0;60.0 |];
                           [| 100.0;200.0;300.0;400.0;500.0;600.0 |]  |] |])

        let newSlice = [|0.; 0.;|]
        m1.[0,*,0] <- newSlice
        Assert.AreEqual(m1.[0,0,1], 2.0)
        Assert.AreEqual(m1.[0,*,0], newSlice)

    [<Test>]
    member this.SlicingSingleFixed3() =
        let m1 = (array3d [| 
                        [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                           [| 11.0;21.0;31.0;41.0;51.0;61.0 |]  |]
                        [| [| 10.0;20.0;30.0;40.0;50.0;60.0 |];
                           [| 100.0;200.0;300.0;400.0;500.0;600.0 |]  |] |])

        let newSlice = [|0.; 0.|]
        m1.[*,0,0] <- newSlice
        Assert.AreEqual(m1.[0,0,1], 2.0)
        Assert.AreEqual(m1.[*,0,0], newSlice)

    [<Test>]
    member this.SlicingDoubleFixed1() =
        let m1 = (array3d [| 
                        [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                           [| 11.0;21.0;31.0;41.0;51.0;61.0 |]  |]
                        [| [| 10.0;20.0;30.0;40.0;50.0;60.0 |];
                           [| 100.0;200.0;300.0;400.0;500.0;600.0 |]  |] |])

        let newSlice = array2D [| [|0.; 0.; 0.; 0.; 0. ; 0.;|]; [|0.; 0.; 0.; 0.; 0. ; 0.;|] |]
        m1.[0,*,*] <- newSlice
        Assert.AreEqual(m1.[1,0,0], 10.0)
        if m1.[0,*,*] <> newSlice then Assert.Fail()
        
    [<Test>]
    member this.SlicingDoubleFixed2() =
        let m1 = (array3d [| 
                        [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                           [| 11.0;21.0;31.0;41.0;51.0;61.0 |]  |]
                        [| [| 10.0;20.0;30.0;40.0;50.0;60.0 |];
                           [| 100.0;200.0;300.0;400.0;500.0;600.0 |]  |] |])

        let newSlice = array2D [| [|0.; 0.;|]; [|0.; 0.;|] |]
        m1.[*,*,0] <- newSlice
        Assert.AreEqual(m1.[0,0,1], 2.0)
        if m1.[*,*,0] <> newSlice then Assert.Fail()

    [<Test>]
    member this.SlicingDoubleFixed3() =
        let m1 = (array3d [| 
                        [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                           [| 11.0;21.0;31.0;41.0;51.0;61.0 |]  |]
                        [| [| 10.0;20.0;30.0;40.0;50.0;60.0 |];
                           [| 100.0;200.0;300.0;400.0;500.0;600.0 |]  |] |])

        let newSlice = array2D [| [|0.; 0.; 0.; 0.; 0. ; 0.;|]; [|0.; 0.; 0.; 0.; 0. ; 0.;|] |]
        m1.[*,0,*] <- newSlice
        Assert.AreEqual(m1.[0,1,0], 11.0)
        if m1.[*,0,*] <> newSlice then Assert.Fail()
