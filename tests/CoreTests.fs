module CoreTests

open System
open PouchDB.Core
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

let inline equal (expected: 'T) (actual: 'T): unit =
    Testing.Assert.AreEqual(expected, actual)

[<Global>]
let it (_msg: string) (f: unit->unit): unit = jsNative

[<Global>]
let describe (_msg: string) (f: unit->unit): unit = jsNative

describe "Core tests" <| fun _ ->
    it "Simple test" <| fun () ->
        "0" |> equal "0"