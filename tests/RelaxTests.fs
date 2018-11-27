module RelaxTests

open PouchDB.Core
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Thoth.Json
open Fable.PowerPack
open Types
open ElPouch.Types

[<Global>]
let it (msg: string) (f: unit->JS.Promise<'T>): unit = jsNative
    
let inline equal (expected: 'T) (actual: 'T): unit = Testing.Assert.AreEqual(expected, actual)

[<Global>]
let describe (_msg: string) (f: unit->unit): unit = jsNative

// our test db
describe "Relax.Database" <| fun _ ->

    it "Put" <| fun () ->
      let expected = System.Guid.NewGuid().ToString()
      let db = ElPouch.Relax.Database.create "relax"
      promise {
        
        // insert
        let! inserted = 
          { Id = expected; Rev = None; Deleted = None; SomeInformation=None}
            |> Test.Encode
            |> db.put

        return inserted.id
      }
        |> Promise.map(fun actual -> equal expected actual )    

describe "Relax" <| fun _ ->

    it "Get" <| fun () ->
      let db = ElPouch.Relax.Database.create "relax"
      let expected = System.Guid.NewGuid().ToString()

      // insert
      { Id = expected; Rev = None; Deleted = None; SomeInformation=None}
        |> Test.Encode
        |> db.put
        |> ignore

      expected
        |> ElPouch.Relax.get Test.Decoder db        
        |> Promise.map(fun result -> 
          match result with 
          | Found result -> 
            match result with 
            | Ok actual -> equal expected actual.Id
            | Error _ -> equal expected "wrong one" 
          | NotFound _ -> equal "expected" "wrong one" 
          )

    it "Get should fail" <| fun () ->
      let db = ElPouch.Relax.Database.create "relax"
      let expected = System.Guid.NewGuid().ToString()

      expected
        |> ElPouch.Relax.get Test.Decoder db        
        |> Promise.map(fun result -> 
          match result with 
          | Found result -> 
            match result with 
            | Ok actual -> equal expected actual.Id
            | Error _ -> equal expected "wrong one" 
          | NotFound _ -> equal "wrong" "wrong" 
        )

    it "Get should fail with status code 404" <| fun () ->
      let db = ElPouch.Relax.Database.create "relax"
      let expected = 404

      "unknown id"
        |> ElPouch.Relax.get Test.Decoder db        
        |> Promise.map(fun result -> 
          match result with 
          | Found result -> 
            match result with 
            | Ok actual -> equal expected -1
            | Error _ -> equal expected -1 
          | NotFound error -> 
            match error.status with 
            | Some s -> equal expected s
            | _ -> equal expected -1
        )
