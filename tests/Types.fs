module Types
open Thoth.Json

module Encode = ElPouch.Encoder

type Test =
  {
    Id: string
    Rev: string option
    Deleted: bool option
    SomeInformation: string option
    SomeInt: int
  }

  static member Base() =
    {
      Id = System.Guid.NewGuid().ToString()
      Rev = None
      Deleted = None
      SomeInformation = None
      SomeInt = 0
    }

  static member Dummy i =
    {
      Test.Base() with
        SomeInformation = Some(sprintf "message#%i" i)
    }

  static member Encode(o: Test) =
    Encode.empty
      |> Encode.string Encode.Required "_id" (Some o.Id)
      |> Encode.string Encode.Optional "_rev" o.Rev
      |> Encode.bool Encode.Optional "_deleted" o.Deleted
      |> Encode.string Encode.Optional "someInformation" o.SomeInformation
      |> Encode.int Encode.Required "someInt" (Some o.SomeInt)
      |> Encode.toPlainJsObj

  static member Decoder: Decode.Decoder<Test> =
    Decode.object
      (fun get ->
        {
          Id = get.Required.Field "_id" Decode.string
          Rev = get.Optional.Field "_rev" (Decode.option Decode.string) |> Option.defaultValue None
          Deleted = get.Optional.Field "_deleted" (Decode.option Decode.bool) |> Option.defaultValue None
          SomeInformation = get.Optional.Field "someInformation" (Decode.option Decode.string) |> Option.defaultValue None
          SomeInt = get.Required.Field "someInt" Decode.int
        }
    )
