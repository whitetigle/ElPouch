module CoreTests

open PouchDB.Core
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Thoth.Json
open Fable.PowerPack
module Encode = ElPouch.Encoder

type Test = 
  {
    Id:string
    Rev:string option
    Deleted:bool option
    SomeInformation: string option
  }    
  static member Encode ( o:Test) = 
    Encode.empty
      |> Encode.string Encode.Required "_id" (Some o.Id)
      |> Encode.string Encode.Optional "_rev" o.Rev
      |> Encode.bool Encode.Optional "_deleted" o.Deleted
      |> Encode.string Encode.Optional "someInformation" o.SomeInformation
      |> Encode.toPlainJsObj

  static member Decoder : Decode.Decoder<Test> = 
    Decode.object
      (fun get -> 
        {
          Id = get.Required.Field "_id" Decode.string
          Rev = get.Optional.Field "_rev" (Decode.option Decode.string) |> Option.defaultValue None
          Deleted = get.Optional.Field "_deleted" (Decode.option Decode.bool) |> Option.defaultValue None
          SomeInformation = get.Optional.Field "someInformation" (Decode.option Decode.string) |> Option.defaultValue None
        } 
    )

[<Global>]
let it (msg: string) (f: unit->JS.Promise<'T>): unit = jsNative
    
let inline equal (expected: 'T) (actual: 'T): unit = Testing.Assert.AreEqual(expected, actual)

[<Global>]
let describe (_msg: string) (f: unit->unit): unit = jsNative

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

