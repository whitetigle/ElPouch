namespace ElPouch

open Fable.Core.JsInterop
open Thoth.Json
open Fable.PowerPack
open PouchDB.Core
open ElPouch.Types

module Relax = 

  module Database = 

    let create (name:string) = 
      PouchDB.Core.instance.Create(!^name,None)

  let get (decoder: Decode.Decoder<'a> ) (db:PouchDB.Core.PouchDB.Database) (id:string) =
      promise {
        try 
          let! document = db.get(id) 
          let strDoc = Fable.Import.JS.JSON.stringify document
          let decoded= Decode.fromString decoder strDoc
          return (Found decoded)
        with e -> 
          let ex: PouchDB.Core.Error = !!e
          return (NotFound ex)
      }

  let allDocs (decoder: Decode.Decoder<'a> ) (db:PouchDB.Core.PouchDB.Database)  =
    promise {
      let! response =db.allDocs (Selectors.retrieveDocs)
      let strDoc = Fable.Import.JS.JSON.stringify(response)
      return 
        Helpers.decodeAllDocs decoder strDoc
          |> function
          | Ok innerDocuments -> innerDocuments |> List.map( fun d -> d.Doc) |> Ok
          | Error s -> Error s
    }