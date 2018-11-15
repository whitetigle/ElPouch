namespace ElPouch

open Fable.Core.JsInterop

module Relax = 

  module Database = 

    let create (name:string) = 
      PouchDB.Core.instance.Create(!^name,None)
