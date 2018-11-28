namespace ElPouch

open Fable.Core.JsInterop
open Thoth.Json
open Fable.PowerPack
open PouchDB.Core
open ElPouch.Types

module Relax =

  module Database =

    let create(name: string) =
      PouchDB.Core.instance.Create(!^name,None)

  let get (decoder: Decode.Decoder<'a>) (db: PouchDB.Core.PouchDB.Database) (id: string) =
      promise {
        try
          let! document = db.get(id)

          return
            document
              |> Fable.Import.JS.JSON.stringify
              |> Decode.fromString decoder
              |> function
                | Ok doc -> GetResult.Found doc
                | Error parsingError -> GetResult.NotFound(JsonError parsingError)

        with e ->
          let ex: PouchDB.Core.Error = !!e
          return (GetResult.NotFound(ServerError ex))
      }

  let bulkInsert (db: PouchDB.Core.PouchDB.Database) (docs: 'T []) =
    let options: PouchDB.Core.BulkDocsOptions = jsOptions(fun opt ->
      opt.docs <- !!docs
    )
    db.bulkDocs options

  module AllDocs =

    let private decode response (decoder: Decode.Decoder<'a>) =
      let strDoc = Fable.Import.JS.JSON.stringify(response)
      Helpers.decodeAllDocs decoder strDoc
        |> function
        | Ok innerDocuments -> innerDocuments |> List.map(fun d -> d.Doc) |> Ok
        | Error s -> Error s

    let all (decoder: Decode.Decoder<'a>) (db: PouchDB.Core.PouchDB.Database) =
      promise {
        let! response = db.allDocs (Selectors.retrieveDocs)
        return decode response decoder
      }

    module Range =
      let between (decoder: Decode.Decoder<'a>) (db: PouchDB.Core.PouchDB.Database) (startKey: string,endkey: string) =
        promise {
          let options: PouchDB.Core.AllDocsWithinRangeOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.startkey <- startKey
            opt.endkey <- endkey
          )
          let! response = db.allDocs options
          return decode response decoder
        }

      let startingWith (decoder: Decode.Decoder<'a>) (db: PouchDB.Core.PouchDB.Database) (key: string) =
        promise {
          let options: PouchDB.Core.AllDocsWithinRangeOptions = jsOptions(fun opt ->
            opt.include_docs <- Some true
            opt.startkey <- key
            opt.endkey <- key + "\ufff0"
          )
          let! response = db.allDocs options
          return decode response decoder
        }
