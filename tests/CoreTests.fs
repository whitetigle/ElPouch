module CoreTests

open PouchDB.Core
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Thoth.Json
open Fable.PowerPack
open Types

[<Global>]
let it (msg: string) (f: unit -> JS.Promise<'T>): unit = jsNative

let inline equal (expected: 'T) (actual: 'T): unit = Testing.Assert.AreEqual(expected,actual)

[<Global>]
let describe (_msg: string) (f: unit -> unit): unit = jsNative

// our test db
let db = PouchDB.Core.instance.Create(!^"db",None)

describe "Core tests" <| fun _ ->

  describe "PUT" <| fun _ ->

    it "Put data" <| fun () ->
      let expected = System.Guid.NewGuid().ToString()
      promise {

        // insert
        let! inserted =
          {Id = expected; Rev = None; Deleted = None; SomeInformation = None; SomeInt = 0}
            |> Test.Encode
            |> db.put

        return inserted.id
      }
        |> Promise.map(fun actual -> equal expected actual)

    it "Put, check output" <| fun () ->
      promise {

        // insert
        let id = System.Guid.NewGuid().ToString()
        let myObject = {Id = id; Rev = None; Deleted = None; SomeInformation = None; SomeInt = 0}
        let! inserted =
          myObject
            |> Test.Encode
            |> db.put

        // the put method returns an updated _rev field
        return inserted.id = myObject.Id && inserted.rev.Length > 0
      }
        |> Promise.map(fun actual -> equal true actual)

    it "Update" <| fun () ->
      let expected = "bar"
      promise {
        let id = System.Guid.NewGuid().ToString()

        {// Insert
        Id = id; Rev = None; Deleted = None; SomeInformation = Some "foo"; SomeInt = 0}
          |> Test.Encode
          |> db.put
          |> ignore

        // Get doc
        let! document = db.get id
        let strDoc = Fable.Import.JS.JSON.stringify document
        let myTest =
          Decode.fromString Test.Decoder strDoc
            |> function
              | Ok myDoc -> myDoc
              | Error _ -> failwith "badResult"

        {// Update doc
        myTest with SomeInformation = Some expected}
          |> Test.Encode
          |> db.put
          |> ignore

        // Get last version
        let! document = db.get id

        return
          document
            |> Fable.Import.JS.JSON.stringify
            |> Decode.fromString Test.Decoder
              |> function
                | Ok myDoc ->
                  match myDoc.SomeInformation with
                  | Some value -> value
                  | None -> "badResult"
                | Error _ -> "badResult"
      }
        |> Promise.map(fun actual -> equal expected actual)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")

    it "Delete" <| fun () ->

      promise {
        let id = System.Guid.NewGuid().ToString()

        {// Insert
        Id = id; Rev = None; Deleted = None; SomeInformation = Some "foo"; SomeInt = 0}
          |> Test.Encode
          |> db.put
          |> ignore

        // Get doc
        let! document = db.get id
        let strDoc = Fable.Import.JS.JSON.stringify document
        let myTest =
          Decode.fromString Test.Decoder strDoc
            |> function
              | Ok myDoc -> myDoc
              | Error _ -> failwith "badResult"

        {// Delete doc
        myTest with Deleted = Some true}
          |> Test.Encode
          |> db.put
          |> ignore

        // should fail!, object is not available anymore
        let! _ = db.get id |> Promise.catch(fun ex -> failwith ex.Message)
        return "whatever"
      }
        |> Promise.map(fun actual -> equal "nogood" actual)
        |> Promise.catch(fun e -> equal "isMissing" "isMissing")

  it "Get" <| fun () ->
    let expected = System.Guid.NewGuid().ToString()
    promise {

      {// insert
      Id = expected; Rev = None; Deleted = None; SomeInformation = None; SomeInt = 0}
        |> Test.Encode
        |> db.put
        |> ignore

      // Get
      let! document = db.get expected

      return
        document
        |> Fable.Import.JS.JSON.stringify
        |> Decode.fromString Test.Decoder
          |> function
            | Ok myDoc -> myDoc.Id
            | Error _ -> "badResult"
    }
      |> Promise.map(fun actual -> equal expected actual)

  it "Bulk" <| fun () ->
    promise {

      let data =
        [0..100]
          |> List.map(Test.Dummy >> Test.Encode)
          |> List.toArray

      let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
        opt.docs <- !!data
      )
      let! results = db.bulkDocs options
      return results.Count = data.Length
    }
      |> Promise.map(fun actual -> equal true actual)
      |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")

describe "All docs" <| fun _ ->

    it "simple test " <| fun () ->
        let fakeDBName = "db/" + System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        let count = ref 0
        promise {

          let data =
            [0..10]
              |> List.map(Test.Dummy >> Test.Encode)
              |> List.toArray

          let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
            opt.docs <- !!data
          )
          let! results = dbf.bulkDocs options
          count := results.Count

          let options: PouchDB.Core.AllDocsOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
          )
          let! response = dbf.allDocs options
          let strDoc = Fable.Import.JS.JSON.stringify(response)
          return
            ElPouch.Helpers.decodeAllDocs Test.Decoder strDoc
              |> function
              | Ok innerDocuments -> innerDocuments |> List.length
              | Error s ->
                match s with
                | ElPouch.Types.HelperError.JsonError message -> printfn "%s" message; -1
                | ElPouch.Types.HelperError.ServerError _ -> -1
        }
        |> Promise.map(fun actual -> equal actual !count)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")

    it "limit" <| fun () ->
        let fakeDBName = "db/" + System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        promise {

          let data =
            [0..10]
              |> List.map(Test.Dummy >> (fun doc -> {doc with SomeInt = 1}) >> Test.Encode)
              |> List.toArray

          let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
            opt.docs <- !!data
          )
          dbf.bulkDocs options |> ignore

          let options: PouchDB.Core.AllDocsOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.limit <- Some 5
          )
          let! response = dbf.allDocs options
          let strDoc = Fable.Import.JS.JSON.stringify(response)
          return
            ElPouch.Helpers.decodeAllDocs Test.Decoder strDoc
              |> function
              | Ok innerDocuments -> innerDocuments |> List.length
              | Error s ->
                match s with
                | ElPouch.Types.HelperError.JsonError message -> printfn "%s" message; -1
                | ElPouch.Types.HelperError.ServerError _ -> -1
        }
        |> Promise.map(fun actual -> equal actual 5)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")

    it "skip" <| fun () ->
        let fakeDBName = "db/" + System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        let expected = 6
        promise {

          let data =
            [1..10]
              |> List.map((fun i -> {(Test.Dummy i) with SomeInt = i}) >> Test.Encode)
              |> List.toArray

          let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
            opt.docs <- !!data
          )
          dbf.bulkDocs options |> ignore

          let options: PouchDB.Core.AllDocsOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.skip <- Some 4
          ) //            opt.limit <- Some 5

          let! response = dbf.allDocs options
          let strDoc = Fable.Import.JS.JSON.stringify(response)
          return
            ElPouch.Helpers.decodeAllDocs Test.Decoder strDoc
              |> function
              | Ok innerDocuments ->
                innerDocuments
                  |> Seq.length
              | Error s ->
                match s with
                | ElPouch.Types.HelperError.JsonError message -> printfn "%s" message; -1
                | ElPouch.Types.HelperError.ServerError _ -> -1
        }
        |> Promise.map(fun actual -> equal actual expected)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")

    it "key should only return documents with IDs matching this string key" <| fun () ->
        let fakeDBName = "db/" + System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        let names = ["Marion";"Mike";"Zeus";"Jacques";"Paul";"Pierre"]
        let key = names |> Seq.head
        let expected = key
        promise {

          let data =
            names
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray

          let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
            opt.docs <- !!data
          )
          dbf.bulkDocs options |> ignore

          let options: PouchDB.Core.AllDocsWithKeyOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.key <- key
          ) //            opt.limit <- Some 5

          let! response = dbf.allDocs options
          let strDoc = Fable.Import.JS.JSON.stringify(response)
          return
            ElPouch.Helpers.decodeAllDocs Test.Decoder strDoc
              |> function
              | Ok innerDocuments ->
                innerDocuments |> Seq.map(fun doc -> doc.Doc.Id) |> Seq.head
              | Error s ->
                match s with
                | ElPouch.Types.HelperError.JsonError message -> printfn "%s" message; "unknown"
                | ElPouch.Types.HelperError.ServerError _ -> "unknown"
        }
        |> Promise.map(fun actual -> equal actual expected)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")

    it "Get documents with IDs in a certain range: starting with Marion " <| fun () ->
        let fakeDBName = "db/" + System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        let names = ["Marion";"Marion_IsTop";"Zeus";"Jacques";"Paul";"Pierre";"Marion_IsGreat"]
        let key = names |> Seq.head
        let expected = 3
        promise {

          let data =
            names
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray

          let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
            opt.docs <- !!data
          )
          dbf.bulkDocs options |> ignore

          let options: PouchDB.Core.AllDocsWithinRangeOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.startkey <- key
            opt.endkey <- key + "\ufff0"
          ) //            opt.limit <- Some 5

          let! response = dbf.allDocs options
          let strDoc = Fable.Import.JS.JSON.stringify(response)
          return
            ElPouch.Helpers.decodeAllDocs Test.Decoder strDoc
              |> function
              | Ok innerDocuments ->
                printfn "%A" innerDocuments
                innerDocuments.Length
              | Error s ->
                match s with
                | ElPouch.Types.HelperError.JsonError message -> printfn "%s" message; -1
                | ElPouch.Types.HelperError.ServerError _ -> -1
        }
        |> Promise.map(fun actual -> equal actual expected)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")


    it "Get documents with IDs in a certain range : names ranging from  M to P" <| fun () ->
        let fakeDBName = "db/" + System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        let names = ["Quentin";"Marion";"Marion_IsTop";"Zeus";"Jacques";"Paul";"Pierre";"Marion_IsGreat"]
        let expected = 5
        promise {

          let data =
            names
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray

          let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
            opt.docs <- !!data
          )
          dbf.bulkDocs options |> ignore

          let options: PouchDB.Core.AllDocsWithinRangeOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.startkey <- "M"
            opt.endkey <- "Q"
          )

          let! response = dbf.allDocs options
          let strDoc = Fable.Import.JS.JSON.stringify(response)
          return
            ElPouch.Helpers.decodeAllDocs Test.Decoder strDoc
              |> function
              | Ok innerDocuments ->
                printfn "%A" innerDocuments
                innerDocuments.Length
              | Error s ->
                match s with
                | ElPouch.Types.HelperError.JsonError message -> printfn "%s" message; -1
                | ElPouch.Types.HelperError.ServerError _ -> -1
        }
        |> Promise.map(fun actual -> equal actual expected)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")

    it "Get documents with IDs in a certain range : All movies with Al Pacino" <| fun () ->
        let fakeDBName = "db/" + System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        let names = ["AlPacino_TheGodfather";"AlPacino_DickTracy";"MarlonBrando_TheMen";"AlPacino_Heat";"MarlonBrando_TheChase"]
        let expected = 3
        promise {

          let data =
            names
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray

          let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
            opt.docs <- !!data
          )
          dbf.bulkDocs options |> ignore

          let options: PouchDB.Core.AllDocsWithinRangeOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.startkey <- "AlPacino"
            opt.endkey <- "AlPacino\ufff0"
          ) //            opt.limit <- Some 5

          let! response = dbf.allDocs options
          let strDoc = Fable.Import.JS.JSON.stringify(response)
          return
            ElPouch.Helpers.decodeAllDocs Test.Decoder strDoc
              |> function
              | Ok innerDocuments ->
                printfn "%A" innerDocuments
                innerDocuments.Length
              | Error s ->
                match s with
                | ElPouch.Types.HelperError.JsonError message -> printfn "%s" message; -1
                | ElPouch.Types.HelperError.ServerError _ -> -1
        }
        |> Promise.map(fun actual -> equal actual expected)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")

    it "Get documents with IDs in a certain range : All movies with Marlon Brando shot in 1953 " <| fun () ->
        let fakeDBName = "db/" + System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        let names = ["MarlonBrando_1953_JulesCesar";"MarlonBrando_1953_TheWildOne";"AlPacino_TheGodfather";"AlPacino_DickTracy"; "MarlonBrando_TheMen";"AlPacino_Heat";"MarlonBrando_TheChase"]
        let expected = 2
        promise {

          let data =
            names
              |> List.map((fun name -> {(Test.Base()) with Id = name}) >> Test.Encode)
              |> List.toArray

          let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
            opt.docs <- !!data
          )
          dbf.bulkDocs options |> ignore

          let options: PouchDB.Core.AllDocsWithinRangeOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.startkey <- "MarlonBrando_1953"
            opt.endkey <- "MarlonBrando_1953\ufff0"
          ) //            opt.limit <- Some 5

          let! response = dbf.allDocs options
          let strDoc = Fable.Import.JS.JSON.stringify(response)
          return
            ElPouch.Helpers.decodeAllDocs Test.Decoder strDoc
              |> function
              | Ok innerDocuments ->
                printfn "%A" innerDocuments
                innerDocuments.Length
              | Error s ->
                match s with
                | ElPouch.Types.HelperError.JsonError message -> printfn "%s" message; -1
                | ElPouch.Types.HelperError.ServerError _ -> -1
        }
        |> Promise.map(fun actual -> equal actual expected)
        |> Promise.catch(fun e -> printfn "%s" e.Message; equal "expected" "wrong")


describe "Conflict resolution" <| fun _ ->

    it "check conflict" <| fun () ->
      let expected = "isConflict"
      promise {

        let o = Test.Base() |> Test.Encode

        db.put(o) |> ignore
        // we didn't update the _rev field: conflict
        db.put(o) |> ignore
        return "whatever"
      }
      |> Promise.map(fun actual -> equal expected actual)
      |> Promise.catch(fun _ -> equal "isConflict" expected)
