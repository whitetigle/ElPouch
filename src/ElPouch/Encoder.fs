namespace ElPouch

open System
open Fable.Core.JsInterop

module Encoder =

    type Field = string * obj
    type FieldCheck =
      | Required
      | Optional

    let dateFormat = "yyyy-MM-ddTHH:mm:ssZ"

    let empty :Field list = List.empty

    let simpleList label (l:'A list) input =
      let output = [ label ==> (l |> List.toArray)]
      input @ output

    let list option label (l:('A list) option) (customEncoder: ('A->obj) option)  input =
      let optional =
        function
        // some content apply the value
        | Some c ->
          match customEncoder with
          | Some encoder ->
            let output = [ label ==> (c |> List.map encoder |> List.toArray)]
            input @ output
          | None ->
            let output = [ label ==> (c |> List.toArray)]
            input @ output
        // no content, so we don't add anything
        | None -> input

      let required =
        function
        | Some c ->
          match customEncoder with
          | Some encoder ->
            let output = [ label ==> (c |> List.map encoder |> List.toArray)]
            input @ output
          | None ->
            let output = [ label ==> (c |> List.toArray)]
            input @ output
        | None ->
          let output = [ label ==> None ]
          input @ output

      match option with
      | Optional -> optional l
      | Required -> required l

    let custom label (a:'B option) input =
      match a with
      // some content apply the value
      | Some c ->
        let output = [ label ==> c ]
        input @ output
      // no content, so we don't add anything
      | None -> input

    let object option label (a:obj option) input =
      let optional =
        function
        // some content apply the value
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        // no content, so we don't add anything
        | None -> input

      let required =
        function
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        | None ->
          let output = [ label ==> None ]
          input @ output

      match option with
      | Optional -> optional a
      | Required -> required a

    let date option label (a:System.DateTime option)  input =
      let toString (date:System.DateTime) = date.ToString(dateFormat)

      let optional =
        function
        // some content apply the value
        | Some c ->
          let output = [ label ==> (c |> toString) ]
          input @ output
        // no content, so we don't add anything
        | None -> input

      let required =
        function
        | Some c ->
          let output = [ label ==> (c |> toString) ]
          input @ output
        | None ->
          let output = [ label ==> None ]
          input @ output

      match option with
      | Optional -> optional a
      | Required -> required a

    let string option label (a:string option)  input =
      let optional =
        function
        // some content apply the value
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        // no content, so we don't add anything
        | None -> input

      let required =
        function
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        | None ->
          let output = [ label ==> None ]
          input @ output

      match option with
      | Optional -> optional a
      | Required -> required a

    let int option label (a:int option)  input =
      let optional =
        function
        // some content apply the value
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        // no content, so we don't add anything
        | None -> input

      let required =
        function
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        | None ->
          let output = [ label ==> None ]
          input @ output

      match option with
      | Optional -> optional a
      | Required -> required a

    let float option label (a:float option)  input =
      let optional =
        function
        // some content apply the value
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        // no content, so we don't add anything
        | None -> input

      let required =
        function
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        | None ->
          let output = [ label ==> None ]
          input @ output

      match option with
      | Optional -> optional a
      | Required -> required a

    let bool option label (a:bool option)  input =
      let optional =
        function
        // some content apply the value
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        // no content, so we don't add anything
        | None -> input

      let required =
        function
        | Some c ->
          let output = [ label ==> c ]
          input @ output
        | None ->
          let output = [ label ==> None ]
          input @ output

      match option with
      | Optional -> optional a
      | Required -> required a

    let toPlainJsObj input =
      createObj input

