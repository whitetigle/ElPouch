module CoreTests

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
let db = PouchDB.Core.instance.Create(!^"test",None)

describe "Core tests" <| fun _ ->

    it "Put" <| fun () ->
      let expected = System.Guid.NewGuid().ToString()
      promise {
        
        // insert
        let! inserted = 
          { Id = expected; Rev = None; Deleted = None; SomeInformation=None}
            |> Test.Encode
            |> db.put

        return inserted.id
      }
        |> Promise.map(fun actual -> equal expected actual )

    it "Put, check output" <| fun () ->
      promise {
        
        // insert
        let id = System.Guid.NewGuid().ToString()
        let myObject =  { Id = id; Rev = None; Deleted = None; SomeInformation=None}
        let! inserted = 
          myObject
            |> Test.Encode
            |> db.put

        // the put method returns an updated _rev field
        return inserted.id = myObject.Id && inserted.rev.Length > 0
      }
        |> Promise.map(fun actual -> equal true actual )

    it "Get" <| fun () ->
      let expected = System.Guid.NewGuid().ToString()
      promise {

        // insert
        { Id = expected; Rev = None; Deleted = None; SomeInformation=None}
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
        |> Promise.map(fun actual -> equal expected actual )

    it "Update" <| fun () ->
      let expected = "bar"
      promise {
        let id = System.Guid.NewGuid().ToString()
        
        // Insert
        { Id = id; Rev = None; Deleted = None; SomeInformation=Some "foo"}
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

        // Update doc
        {myTest with SomeInformation = Some expected} 
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
        |> Promise.map(fun actual -> equal expected actual )
        |> Promise.catch(fun e-> printfn "%s" e.Message; equal "expected" "wrong" )

    it "Delete" <| fun () ->

      promise {
        let id = System.Guid.NewGuid().ToString()
        
        // Insert
        { Id = id; Rev = None; Deleted = None; SomeInformation=Some "foo"}
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

        // Delete doc
        {myTest with Deleted = Some true} 
          |> Test.Encode
          |> db.put
          |> ignore
        
        // should fail!, object is not available anymore
        let! _ = db.get id  |> Promise.catch (fun ex -> failwith ex.Message)
        return "whatever"
      }
        |> Promise.map(fun actual -> equal "nogood" actual )
        |> Promise.catch(fun e-> equal "isMissing" "isMissing" )

    it "Bulk" <| fun () ->
      promise {

        let data = 
          [0..100] 
            |> List.map (Test.Dummy >> Test.Encode)
            |> List.toArray
        
        let options : PouchDB.Core.BulkDocsOptions = jsOptions( fun opt -> 
          opt.docs <- !!data
        )
        let! results = db.bulkDocs options
        return results.Count = data.Length
      }
        |> Promise.map(fun actual -> equal true actual )
        |> Promise.catch(fun e-> printfn "%s" e.Message; equal "expected" "wrong" )

    it "AllDocs" <| fun () ->
        let fakeDBName = System.Guid.NewGuid().ToString()
        let dbf = PouchDB.Core.instance.Create(!^fakeDBName,None)
        let count = ref 0 
        promise {

          let data = 
            [0..10] 
              |> List.map (Test.Dummy >> Test.Encode)
              |> List.toArray
          
          let options : PouchDB.Core.BulkDocsOptions = jsOptions( fun opt -> 
            opt.docs <- !!data
          )
          let! results = dbf.bulkDocs options
          count := results.Count
                    
          let options : PouchDB.Core.AllDocsOptions = jsOptions( fun opt ->
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
                | ElPouch.Types.HelperError.ServerError doc -> printfn "%i" doc.Status; -1
        }
        |> Promise.map(fun actual -> equal actual !count )
        |> Promise.catch(fun e-> printfn "%s" e.Message; equal "expected" "wrong" )

describe "Conflict resolution" <| fun _ ->
    it "Update conflict" <| fun () ->
      let expected = "isConflict"
      promise {

        let o = 
          { Id = expected; Rev = None; Deleted = None; SomeInformation=None}
            |> Test.Encode

        db.put(o) |> ignore
        // we didn't update the _rev field: conflict
        db.put(o) |> ignore
        return "whatever"
      }
        |> Promise.map(fun actual -> equal expected actual )
        |> Promise.catch(fun e -> equal "isConflict" expected )

