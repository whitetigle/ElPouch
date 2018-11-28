module RelaxTests

open PouchDB.Core
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Thoth.Json
open Fable.PowerPack
open Types
open ElPouch.Types
open ElPouch.Relax.AllDocs

[<Global>]
let it (msg: string) (f: unit -> JS.Promise<'T>): unit = jsNative

let inline equal (expected: 'T) (actual: 'T): unit = Testing.Assert.AreEqual(expected,actual)

[<Global>]
let describe (_msg: string) (f: unit -> unit): unit = jsNative

// our test db
describe "Relax.Database" <| fun _ ->

    it "Put" <| fun () ->
      let expected = System.Guid.NewGuid().ToString()
      let db = ElPouch.Relax.Database.create "relax"
      promise {

        // insert
        let! inserted =
          {Id = expected; Rev = None; Deleted = None; SomeInformation = None; SomeInt = 0}
            |> Test.Encode
            |> db.put

        return inserted.id
      }
        |> Promise.map(fun actual -> equal expected actual)

describe "Relax" <| fun _ ->

    it "Get" <| fun () ->
      let db = ElPouch.Relax.Database.create "db/relax"
      let expected = System.Guid.NewGuid().ToString()

      {// insert
      Id = expected; Rev = None; Deleted = None; SomeInformation = None; SomeInt = 0}
        |> Test.Encode
        |> db.put
        |> ignore

      expected
        |> ElPouch.Relax.get Test.Decoder db
        |> Promise.map(fun result ->
          match result with
          | GetResult.Found result ->
            equal expected result.Id
          | _ ->
            equal "expected" "wrong one"
          )

    it "Get should fail" <| fun () ->
      let db = ElPouch.Relax.Database.create "db/relax"
      let expected = System.Guid.NewGuid().ToString()

      expected
        |> ElPouch.Relax.get Test.Decoder db
        |> Promise.map(fun result ->
          match result with
          | GetResult.Found result ->
            equal expected result.Id
          | _ -> equal "wrong" "wrong"
        )

    it "Get should fail with status code 404" <| fun () ->
      let db = ElPouch.Relax.Database.create "db/relax"
      let expected = 404

      "unknown id"
        |> ElPouch.Relax.get Test.Decoder db
        |> Promise.map(fun result ->
          match result with
          | GetResult.Found result ->
            equal expected -1
          | GetResult.NotFound error ->
            match error with
            | ServerError err ->
              match err.status with
              | Some s -> equal expected s
              | _ -> equal expected -1
            | _ -> equal expected -1 // JSON error
        )

    describe "AllDocs" <| fun _ ->

        it "simple test " <| fun () ->
          let db = ElPouch.Relax.Database.create("db/" + System.Guid.NewGuid().ToString())
          [0..10]
            |> List.map(Test.Dummy >> Test.Encode)
            |> List.toArray
            |> ElPouch.Relax.bulkInsert db
            |> Promise.map(fun results ->
              let count = results.Count
              all Test.Decoder db
              |> Promise.map(fun results ->
                match results with
                | Ok docs -> equal docs.Length count
                | _ -> equal "expected" "wrong"
              )
            )

        it "simple test: with promise CE" <| fun () ->
          let db = ElPouch.Relax.Database.create("db/" + System.Guid.NewGuid().ToString())
          promise {
            let! insertedDocuments =
              [0..10]
                |> List.map(Test.Dummy >> Test.Encode)
                |> List.toArray
                |> ElPouch.Relax.bulkInsert db
            let count = insertedDocuments.Count
            let! docs = all Test.Decoder db
            match docs with
            | Ok found -> return count = found.Length
            | _ -> return false
          }
            |> Promise.map(fun isOk -> equal isOk true)

        it "Get documents with IDs in a certain range: starting with Marion" <| fun () ->
          let db = ElPouch.Relax.Database.create("db/" + System.Guid.NewGuid().ToString())
          promise {
            ["Marion";"Marion_IsTop";"Zeus";"Jacques";"Paul";"Pierre";"Marion_IsGreat"]
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray
              |> ElPouch.Relax.bulkInsert db
              |> ignore

            let! docs = Range.startingWith Test.Decoder db "Marion"
            match docs with
            | Ok found -> return 3 = found.Length
            | _ -> return false
          }
            |> Promise.map(fun isOk -> equal isOk true)

        it "Get documents with IDs in a certain range : names ranging from  M to P" <| fun () ->
          let db = ElPouch.Relax.Database.create("db/" + System.Guid.NewGuid().ToString())
          promise {
            ["Quentin";"Marion";"Marion_IsTop";"Zeus";"Jacques";"Paul";"Pierre";"Marion_IsGreat"]
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray
              |> ElPouch.Relax.bulkInsert db
              |> ignore

            // range is not inclusive. will match only M to P
            let! docs = Range.between Test.Decoder db ("M","Q")
            match docs with
            | Ok found -> return 5 = found.Length
            | _ -> return false
          }
            |> Promise.map(fun isOk -> equal isOk true)

        it "Get documents with IDs in a certain range : names ranging from  M to Q inclusive" <| fun () ->
          let db = ElPouch.Relax.Database.create("db/" + System.Guid.NewGuid().ToString())
          promise {
            ["Quentin";"Marion";"Marion_IsTop";"Zeus";"Jacques";"Paul";"Pierre";"Marion_IsGreat"]
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray
              |> ElPouch.Relax.bulkInsert db
              |> ignore

            // range is inclusive. will match M to Q included
            let! docs = Range.between Test.Decoder db ("M","Q\ufff0")
            match docs with
            | Ok found -> return 6 = found.Length
            | _ -> return false
          }
            |> Promise.map(fun isOk -> equal isOk true)

        it "Get documents with IDs in a certain range : All movies with Al Pacino" <| fun () ->
          let db = ElPouch.Relax.Database.create("db/" + System.Guid.NewGuid().ToString())
          promise {
            ["AlPacino_TheGodfather";"AlPacino_DickTracy";"MarlonBrando_TheMen";"AlPacino_Heat";"MarlonBrando_TheChase"]
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray
              |> ElPouch.Relax.bulkInsert db
              |> ignore

            let! docs = Range.startingWith Test.Decoder db "AlPacino"
            match docs with
            | Ok found -> return 3 = found.Length
            | _ -> return false
          }
            |> Promise.map(fun isOk -> equal isOk true)

        it "Get documents with IDs in a certain range : All movies with Marlon Brando shot in 1953" <| fun () ->
          let db = ElPouch.Relax.Database.create("db/" + System.Guid.NewGuid().ToString())
          promise {
            ["MarlonBrando_1953_JulesCesar";"MarlonBrando_1953_TheWildOne";"AlPacino_TheGodfather";"AlPacino_DickTracy"; "MarlonBrando_TheMen";"AlPacino_Heat";"MarlonBrando_TheChase"]
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray
              |> ElPouch.Relax.bulkInsert db
              |> ignore

            let! docs = Range.startingWith Test.Decoder db "MarlonBrando_1953"
            match docs with
            | Ok found -> return 2 = found.Length
            | _ -> return false
          }
            |> Promise.map(fun isOk -> equal isOk true)
