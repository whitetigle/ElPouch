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
  }    
  static member Encode ( o:Test) = 
    Encode.empty
      |> Encode.string Encode.Required "_id" (Some o.Id)
      |> Encode.string Encode.Optional "_rev" o.Rev
      |> Encode.bool Encode.Optional "_deleted" o.Deleted
      |> Encode.toPlainJsObj

  static member Decoder : Decode.Decoder<Test> = 
    Decode.object
      (fun get -> 
        {
          Id = get.Required.Field "_id" Decode.string
          Rev = get.Optional.Field "_rev" (Decode.option Decode.string) |> Option.defaultValue None
          Deleted = get.Optional.Field "_deleted" (Decode.option Decode.bool) |> Option.defaultValue None
        } 
    )

[<Global>]
let it (msg: string) (f: unit->JS.Promise<'T>): unit = jsNative
    
let inline equal (expected: 'T) (actual: 'T): unit = Testing.Assert.AreEqual(expected, actual)

[<Global>]
let describe (_msg: string) (f: unit->unit): unit = jsNative

describe "Core tests" <| fun _ ->
    it "Put" <| fun () ->
      let expected = System.Guid.NewGuid().ToString()
      promise {
        let db = PouchDB.Core.instance.Create(!^"test",None)
        let o = Test.Encode( { Id = expected; Rev = None; Deleted = None})
        let! r= db.put(o)
        return r.id
      }
      |> Promise.map(fun actual -> equal expected actual )
