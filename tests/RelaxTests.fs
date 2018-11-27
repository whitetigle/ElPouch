module RelaxTests

open PouchDB.Core
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Thoth.Json
open Fable.PowerPack
open Types

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
          | Ok actual -> equal expected actual.Id
          | Error _ -> equal expected "wrong one" 
          )