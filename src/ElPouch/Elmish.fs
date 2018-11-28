namespace ElPouch

open Helpers
open Thoth.Json
open PouchDB.Core

module Elmish =

  module Relax =

    let allDocs (handler: 'Msg) (documentDecoder: Decode.Decoder<'Doc>) (store: PouchDB.Database) =
      Standard.allDocs Selectors.retrieveDocs handler documentDecoder store
