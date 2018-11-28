namespace ElPouch 

  open Thoth.Json
  open Fable.Core.JsInterop
  open PouchDB.Core

  module Types =
    type Code = int
    type Message = string
    type RequestError = Code * Message

    [<RequireQualifiedAccess>]
    type GetResult<'T> =
      | Found of 'T
      | NotFound of HelperError 

    and HelperError =
      | JsonError of string
      | ServerError of PouchDB.Core.Error

    and Value = 
      {
        Rev:string option
      }
      static member Decoder : Decode.Decoder<Value>  =
        Decode.object
          (fun get ->
            {
              Rev = get.Optional.Field "rev" (Decode.option Decode.string) |> Option.defaultValue None
            }
          )

    and InnerDocument<'T> =
      {
        Id:string
        Key:string option
        Value:Value option
        Doc:'T
      }
      static member Decoder customDecoder : Decode.Decoder<InnerDocument<'T>>  =
        Decode.object
          (fun get ->
            {
              Id = get.Required.Field "id" Decode.string
              Key = get.Optional.Field "key" (Decode.option Decode.string) |> Option.defaultValue None
              Value = get.Optional.Field "value" (Decode.option Value.Decoder) |> Option.defaultValue None
              Doc = get.Required.Field "doc" customDecoder
            }
          )

    and ErrorDocument =
      {
        Status:int
        Name:string option
        Message:string option
        Error:bool option
        DocId:string option
        Id:string option
      }
      static member Decoder : Decode.Decoder<ErrorDocument> =
          Decode.object
              (fun get ->
                  { 
                    Status = get.Required.Field "status" Decode.int
                    Name = get.Optional.Field "name" (Decode.option Decode.string) |> Option.defaultValue None
                    Message = get.Optional.Field "message" (Decode.option Decode.string) |> Option.defaultValue None
                    Error = get.Optional.Field "error" (Decode.option Decode.bool) |> Option.defaultValue None
                    DocId = get.Optional.Field "docId" (Decode.option Decode.string) |> Option.defaultValue None
                    Id = get.Optional.Field "id" (Decode.option Decode.string) |> Option.defaultValue None
                  } 
                )

    type AllDocsDocument<'T> =
      {
        TotalRows:int
        Offset:int
        Rows:InnerDocument<'T> list
      }
      static member Decoder customDecoder : Decode.Decoder<AllDocsDocument<'T>>=
        Decode.object
          (fun get  ->
            {
              TotalRows= get.Required.Field "total_rows" Decode.int
              Offset= get.Required.Field "offset" Decode.int
              Rows = get.Required.Field "rows" (Decode.list (InnerDocument.Decoder customDecoder) )
          } )

  module Helpers =
    open Types

    let getResponse handler (response:'a) =
      handler (Ok response)

    let decodeAllDocs<'T> (customDecoder:Decode.Decoder<'T>) stringToDecode : Result<InnerDocument<'T> list, HelperError> =
      Decode.fromString (AllDocsDocument.Decoder customDecoder) stringToDecode
      |> function
        | Ok result -> Ok result.Rows
        | Error error -> Error (JsonError error)

    let decodeDocuments customDecoder document :Result<list<'T >, HelperError> =
      let strDoc = document.ToString()
      decodeAllDocs customDecoder strDoc
      |> function
        | Ok innerDocuments -> innerDocuments |> List.map( fun d -> d.Doc) |> Ok
        | Error s -> Error s

    (*
    let checkError err =
        err
          |> string
          |> Decode.fromString ErrorDocument.Decoder
          |> function
              | Ok data -> Error (ServerError data) // server error
              | Error err -> Error (JsonError err) // decoder error
    *)

  module Selectors =

    open PouchDB.Core

    let retrieveDocs : PouchDB.Core.AllDocsOptions = jsOptions( fun opt ->
      opt.include_docs <- Some true
    )

  module Standard =

    open PouchDB.Core
    open Types
    
    let allDocs options (handler:'Msg) documentDecoder (store:PouchDB.Database) =
      Elmish.Cmd.ofPromise
        store.allDocs
        options
        (fun response ->
          let strDoc = response.ToString()
          let results =
            Helpers.decodeAllDocs documentDecoder strDoc
            |> function
              | Ok innerDocuments -> innerDocuments |> List.map( fun d -> d.Doc) |> Ok
              | Error s -> Error s
          handler results
        )
        (fun err -> 
          let ex: PouchDB.Core.Error = !!err
          handler (Error (ServerError ex)) )
