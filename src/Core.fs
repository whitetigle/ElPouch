namespace PouchDB

module rec Core =

  open System
  open System.Text.RegularExpressions
  open Fable.Core
  open Fable.Import.JS
  open Fable.Import.Browser

  [<Import("default", from="pouchdb")>]
  let instance: PouchDB.Static = jsNative

  type [<AllowNullLiteral>] Buffer =
      inherit Uint8Array
      abstract write: string:string * ?offset:float * ?length:float * ?encoding:string -> float
      abstract toString: ?encoding:string * ?start:float * ?``end``:float -> string
      abstract toJSON: unit -> obj
      abstract equals: otherBuffer:Buffer -> bool
      abstract compare: otherBuffer:Buffer * ?targetStart:float * ?targetEnd:float * ?sourceStart:float * ?sourceEnd:float -> float
      abstract copy: targetBuffer:Buffer * ?targetStart:float * ?sourceStart:float * ?sourceEnd:float -> float
      abstract slice: ?start:float * ?``end``:float -> Buffer
      abstract writeUIntLE: value:float * offset:float * byteLength:float * ?noAssert:bool -> float
      abstract writeUIntBE: value:float * offset:float * byteLength:float * ?noAssert:bool -> float
      abstract writeIntLE: value:float * offset:float * byteLength:float * ?noAssert:bool -> float
      abstract writeIntBE: value:float * offset:float * byteLength:float * ?noAssert:bool -> float
      abstract readUIntLE: offset:float * byteLength:float * ?noAssert:bool -> float
      abstract readUIntBE: offset:float * byteLength:float * ?noAssert:bool -> float
      abstract readIntLE: offset:float * byteLength:float * ?noAssert:bool -> float
      abstract readIntBE: offset:float * byteLength:float * ?noAssert:bool -> float
      abstract readUInt8: offset:float * ?noAssert:bool -> float
      abstract readUInt16LE: offset:float * ?noAssert:bool -> float
      abstract readUInt16BE: offset:float * ?noAssert:bool -> float
      abstract readUInt32LE: offset:float * ?noAssert:bool -> float
      abstract readUInt32BE: offset:float * ?noAssert:bool -> float
      abstract readInt8: offset:float * ?noAssert:bool -> float
      abstract readInt16LE: offset:float * ?noAssert:bool -> float
      abstract readInt16BE: offset:float * ?noAssert:bool -> float
      abstract readInt32LE: offset:float * ?noAssert:bool -> float
      abstract readInt32BE: offset:float * ?noAssert:bool -> float
      abstract readFloatLE: offset:float * ?noAssert:bool -> float
      abstract readFloatBE: offset:float * ?noAssert:bool -> float
      abstract readDoubleLE: offset:float * ?noAssert:bool -> float
      abstract readDoubleBE: offset:float * ?noAssert:bool -> float
      abstract swap16: unit -> Buffer
      abstract swap32: unit -> Buffer
      abstract swap64: unit -> Buffer
      abstract writeUInt8: value:float * offset:float * ?noAssert:bool -> float
      abstract writeUInt16LE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeUInt16BE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeUInt32LE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeUInt32BE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeInt8: value:float * offset:float * ?noAssert:bool -> float
      abstract writeInt16LE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeInt16BE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeInt32LE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeInt32BE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeFloatLE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeFloatBE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeDoubleLE: value:float * offset:float * ?noAssert:bool -> float
      abstract writeDoubleBE: value:float * offset:float * ?noAssert:bool -> float
      abstract fill: value:obj * ?offset:float * ?``end``:float -> obj
      abstract indexOf: value:U3<string,float,Buffer> * ?byteOffset:float * ?encoding:string -> float
      abstract lastIndexOf: value:U3<string,float,Buffer> * ?byteOffset:float * ?encoding:string -> float
      abstract entries: unit -> IterableIterator<float * float>
      abstract includes: value:U3<string,float,Buffer> * ?byteOffset:float * ?encoding:string -> bool
      abstract keys: unit -> IterableIterator<float>
      abstract values: unit -> IterableIterator<float>

  and  [<AllowNullLiteral>] EventEmitter =
      abstract addListener: event:U2<string,Symbol> * listener:Function -> obj
      abstract on: event:U2<string,Symbol> * listener:Function -> obj
      abstract once: event:U2<string,Symbol> * listener:Function -> obj
      abstract removeListener: event:U2<string,Symbol> * listener:Function -> obj
      abstract removeAllListeners: event:U2<string,Symbol> -> obj
      abstract setMaxListeners: n:float -> obj
      abstract getMaxListeners: unit -> float
      abstract listeners: event:U2<string,Symbol> -> ResizeArray<Function>
      abstract emit: event:U2<string,Symbol> *  [<ParamArray>] args:obj [] -> bool
      abstract listenerCount: ``type``:U2<string,Symbol> -> float
      abstract prependListener: event:U2<string,Symbol> * listener:Function -> obj
      abstract prependOnceListener: event:U2<string,Symbol> * listener:Function -> obj
      abstract eventNames: unit -> ResizeArray<U2<string,Symbol>>



  module PouchDB =
      type Plugin =
          obj

      module Configuration =
          type [<AllowNullLiteral>] CommonDatabaseConfiguration =
              abstract name: string option with get, set
              abstract adapter: string option with get, set

          and  [<AllowNullLiteral>] LocalDatabaseConfiguration =
              inherit CommonDatabaseConfiguration
              abstract auto_compaction: bool option with get, set
              abstract revs_limit: float option with get, set

          and  [<AllowNullLiteral>] RemoteRequesterConfiguration =
              abstract timeout: float option with get, set
              abstract cache: bool option with get, set
              abstract headers: obj option with get, set
              abstract withCredentials: bool option with get, set

          and  [<AllowNullLiteral>] RemoteDatabaseConfiguration =
              inherit CommonDatabaseConfiguration
              abstract ajax: RemoteRequesterConfiguration option with get, set
              abstract auth: obj option with get, set
              abstract skip_setup: bool option with get, set

          and DatabaseConfiguration =
              U2<LocalDatabaseConfiguration,RemoteDatabaseConfiguration>

      module Core =
          type [<AllowNullLiteral>] Error =
              abstract status: int option with get, set
              abstract name: string option with get, set
              abstract message: string option with get, set
              abstract reason: string option with get, set
              abstract error: U2<string,bool> option with get, set
              abstract id: string option with get, set
              abstract rev: RevisionId option with get, set

          and Callback<'R> =
              Func<U2<Error,obj>,U2<'R,obj>,unit>

          and DocumentId =
              string

          and DocumentKey =
              string

          and AttachmentId =
              string

          and RevisionId =
              string

          and  [<StringEnum>] Availability = | Available | Compacted | ``Not compacted`` | Missing

          and AttachmentData =
              U3<string,Blob,Buffer>

          and Options =
              abstract ajax: Configuration.RemoteRequesterConfiguration option with get, set

          and  [<AllowNullLiteral>] BasicResponse =
              abstract ok: bool with get, set

          and  [<AllowNullLiteral>] Response =
              inherit BasicResponse
              abstract id: DocumentId with get, set
              abstract rev: RevisionId with get, set

          and  [<AllowNullLiteral>] DatabaseInfo =
              abstract db_name: string with get, set
              abstract doc_count: int with get, set
              abstract update_seq: U2<int,string> with get, set

          and  [<AllowNullLiteral>] Revision<'Content> =
              abstract ok: obj with get, set

          and  [<AllowNullLiteral>] RevisionInfo =
              abstract rev: RevisionId with get, set
              abstract status: Availability with get, set

          and  [<AllowNullLiteral>] RevisionDiffOptions =
              [<Emit("$0[$1]{{=$2}}")>] abstract Item: DocumentId:string -> ResizeArray<string> with get, set

          and  [<AllowNullLiteral>] RevisionDiff =
              abstract missing: ResizeArray<string> option with get, set
              abstract possible_ancestors: ResizeArray<string> option with get, set

          and  [<AllowNullLiteral>] RevisionDiffResponse =
              [<Emit("$0[$1]{{=$2}}")>] abstract Item: DocumentId:string -> RevisionDiff with get, set

          and  [<AllowNullLiteral>] IdMeta =
              abstract _id: DocumentId with get, set

          and  [<AllowNullLiteral>] RevisionIdMeta =
              abstract _rev: RevisionId with get, set

          and  [<AllowNullLiteral>] GetMeta =
              abstract _conflicts: ResizeArray<RevisionId> option with get, set
              abstract _rev: RevisionId with get, set
              abstract _revs_info: ResizeArray<RevisionInfo> option with get, set
              abstract _revisions: obj option with get, set
              abstract _attachments: Attachments option with get, set

          and  [<AllowNullLiteral>] StubAttachment =
              abstract content_type: string with get, set
              abstract digest: string with get, set
              abstract stub: obj with get, set
              abstract ``true``: obj with get, set
              abstract length: int with get, set

          and  [<AllowNullLiteral>] FullAttachment =
              abstract content_type: string with get, set
              abstract digest: string option with get, set
              abstract data: AttachmentData with get, set

          and Attachment =
              U2<StubAttachment,FullAttachment>

          and  [<AllowNullLiteral>] Attachments =
              [<Emit("$0[$1]{{=$2}}")>] abstract Item: attachmentId:string -> Attachment with get, set

          and NewDocument = obj

          and Document = obj

          and ExistingDocument = obj

          and RemoveDocument = obj

          and PostDocument = obj

          and PutDocument = obj

          and AllDocsOptions =
              inherit Options
              abstract attachments: bool option with get, set
              abstract binary: bool option with get, set
              abstract conflicts: bool option with get, set
              abstract descending: bool option with get, set
              abstract include_docs: bool option with get, set
              abstract limit: int option with get, set
              abstract skip: int option with get, set

          and AllDocsWithKeyOptions =
              inherit AllDocsOptions
              abstract key: DocumentKey with get, set

          and AllDocsWithKeysOptions =
              inherit AllDocsOptions
              abstract keys: ResizeArray<DocumentId> with get, set

          and AllDocsWithinRangeOptions =
              inherit AllDocsOptions
              abstract startkey: DocumentKey with get, set
              abstract endkey: DocumentKey with get, set
              abstract inclusive_end: bool option with get, set

          and  [<AllowNullLiteral>] AllDocsMeta =
              abstract _conflicts: ResizeArray<RevisionId> option with get, set
              abstract _attachments: Attachments option with get, set

          and  [<AllowNullLiteral>] AllDocsResponse<'Content> =
              abstract offset: int with get, set
              abstract total_rows: int with get, set
              abstract rows: ResizeArray<obj> with get, set

          and BulkDocsOptions =
              inherit Options
              abstract docs: ResizeArray<Core.PutDocument> with get, set
              abstract new_edits: bool option with get, set

          and BulkGetOptions =
              inherit Options
              abstract docs: ResizeArray<obj> with get, set
              abstract revs: bool option with get, set
              abstract attachments: bool option with get, set
              abstract binary: bool option with get, set

          and  [<AllowNullLiteral>] BulkGetResponse<'Content> =
              abstract results: obj with get, set

          and  [<AllowNullLiteral>] ChangesMeta =
              abstract _conflicts: ResizeArray<RevisionId> option with get, set
              abstract _deleted: bool option with get, set
              abstract _attachments: Attachments option with get, set

          and  [<AllowNullLiteral>] ChangesOptions =
              abstract live: bool option with get, set
              abstract since: (* TODO StringEnum now |  |  *) string option with get, set
              abstract timeout: U2<int,obj> option with get, set
              abstract include_docs: bool option with get, set
              abstract limit: U2<int,obj> option with get, set
              abstract conflicts: bool option with get, set
              abstract attachments: bool option with get, set
              abstract binary: bool option with get, set
              abstract descending: bool option with get, set
              abstract heartbeat: U2<int,obj> option with get, set
              abstract filter: U2<string,Func<obj,obj,obj>> option with get, set
              abstract doc_ids: ResizeArray<string> option with get, set
              abstract query_params: obj option with get, set
              abstract view: string option with get, set

          and  [<AllowNullLiteral>] ChangesResponseChange<'Content> =
              abstract id: string with get, set
              abstract seq: U2<int,string> with get, set
              abstract changes: ResizeArray<obj> with get, set
              abstract deleted: bool option with get, set
              abstract doc: ExistingDocument option with get, set

          and  [<AllowNullLiteral>] ChangesResponse =
              abstract status: string with get, set
              abstract last_seq: U2<int,string> with get, set
              abstract results: ResizeArray<ChangesResponseChange<'Content>> with get, set

          and  [<AllowNullLiteral>] Changes =
              inherit EventEmitter
              inherit Promise<ChangesResponse>
              [<Emit("$0.on('change',$1...)")>] abstract on_change: listener:Func<ChangesResponseChange<'Content>,obj> -> obj
              [<Emit("$0.on('complete',$1...)")>] abstract on_complete: listener:Func<ChangesResponse,obj> -> obj
              [<Emit("$0.on('error',$1...)")>] abstract on_error: listener:Func<obj,obj> -> obj
              abstract cancel: unit -> unit

          and GetOptions =
              inherit Options
              abstract conflicts: bool option with get, set
              abstract rev: RevisionId option with get, set
              abstract revs: bool option with get, set
              abstract revs_info: bool option with get, set
              abstract attachments: bool option with get, set
              abstract binary: bool option with get, set
              abstract latest: bool option with get, set

          and GetOpenRevisions =
              inherit Options
              abstract open_revs: (* TODO StringEnum all |  *) string with get, set
              abstract revs: bool option with get, set

          and CompactOptions =
              inherit Options
              abstract interval: float option with get, set

          and RemoveAttachmentResponse =
              inherit BasicResponse
              abstract id: DocumentId with get, set
              abstract rev: RevisionId with get, set

      module Find =
          type IConditionOperators =
              [<Emit("$0.$lt{{=$1}}")>] abstract lt: obj with get, set
              [<Emit("$0.$gt{{=$1}}")>] abstract gt: obj with get, set
              [<Emit("$0.$lte{{=$1}}")>] abstract lte: obj with get, set
              [<Emit("$0.$gte{{=$1}}")>] abstract gte: obj with get, set
              [<Emit("$0.$eq{{=$1}}")>] abstract eq: obj with get, set
              [<Emit("$0.$ne{{=$1}}")>] abstract ne: obj with get, set
              [<Emit("$0.$exists{{=$1}}")>] abstract exists: bool with get, set
              [<Emit("$0.$type{{=$1}}")>] abstract ``type``: (* TODO StringEnum null | boolean | number | string | array | object *) string with get, set
              [<Emit("$0.$in{{=$1}}")>] abstract ``in``: ResizeArray<obj> with get, set
              [<Emit("$0.$nin{{=$1}}")>] abstract nin: ResizeArray<obj> with get, set
              [<Emit("$0.$size{{=$1}}")>] abstract size: float with get, set
              [<Emit("$0.$mod{{=$1}}")>] abstract ``mod``: float * float with get, set
              [<Emit("$0.$regex{{=$1}}")>] abstract regex: string with get, set
              [<Emit("$0.$all{{=$1}}")>] abstract all: ResizeArray<obj> with get, set
              [<Emit("$0.$elemMatch{{=$1}}")>] abstract elemMatch: IConditionOperators with get, set

          and  [<AllowNullLiteral>] ICombinationOperators =
              [<Emit("$0.$and{{=$1}}")>] abstract ``and``: ResizeArray<Selector> with get, set
              [<Emit("$0.$or{{=$1}}")>] abstract ``or``: ResizeArray<Selector> with get, set
              [<Emit("$0.$not{{=$1}}")>] abstract not: Selector with get, set
              [<Emit("$0.$nor{{=$1}}")>] abstract nor: ResizeArray<Selector> with get, set

          and  [<AllowNullLiteral>] Selector =
              inherit ICombinationOperators
              [<Emit("$0[$1]{{=$2}}")>] abstract Item: field:string -> U4<Selector,ResizeArray<Selector>,IConditionOperators,obj> with get, set
              abstract _id: IConditionOperators option with get, set

          and  [<AllowNullLiteral>] FindRequest =
              abstract selector: Selector with get, set
              abstract fields: ResizeArray<string> option with get, set
              abstract sort: ResizeArray<string> option with get, set
  //            abstract sort: ResizeArray<U2<string, obj>> option with get, set
              abstract limit: int option with get, set
              abstract skip: int option with get, set
              abstract use_index: U2<string,string * string> option with get, set

          and  [<AllowNullLiteral>] FindResponse =
              abstract docs: ResizeArray<Core.Document> with get, set

          // manually added
          and  [<AllowNullLiteral>] CreateIndexRequest =
              abstract fields: ResizeArray<string> option with get, set

          and  [<AllowNullLiteral>] CreateIndexOptions =
              abstract index: CreateIndexRequest with get, set

          and  [<AllowNullLiteral>] CreateIndexResponse =
              abstract result: string with get, set

          and  [<AllowNullLiteral>] Index =
              abstract name: string with get, set
              abstract ddoc: U2<string,obj> with get, set
              abstract ``null``: obj with get, set
              abstract ``type``: string with get, set
              abstract def: obj with get, set

          and  [<AllowNullLiteral>] GetIndexesResponse =
              abstract indexes: ResizeArray<Index> with get, set

          and  [<AllowNullLiteral>] DeleteIndexOptions =
              abstract name: string with get, set
              abstract ddoc: string with get, set
              abstract ``type``: string option with get, set

          and  [<AllowNullLiteral>] DeleteIndexResponse<'Content> =
              [<Emit("$0[$1]{{=$2}}")>] abstract Item: propertyName:string -> obj with get, set

        module Query =

          type Stale =
            |  [<Emit("ok")>] Ok
            |  [<Emit("update_after ")>] UpdateAfter

          type Options =
              abstract reduce: U2<string,bool> option with get, set
              abstract include_docs: bool option with get, set
              abstract startkey: string option with get, set
              abstract endkey: string option with get, set
              abstract inclusive_end: bool option with get, set
              abstract limit: int option with get, set
              abstract skip: int option with get, set
              abstract descending: bool option with get, set
              abstract key: string option with get, set
              abstract keys: ResizeArray<string> option with get, set
              abstract group: bool option with get, set
              abstract group_level: int option with get, set
              abstract stale: Stale option with get, set
              abstract update_seq: bool option with get, set


        module Replication =

            type [<AllowNullLiteral>] ReplicateOptions =
                /// If true, starts subscribing to future changes in the source database and continue replicating them.
                abstract live: bool option with get, set
                /// If true will attempt to retry replications in the case of failure (due to being offline),
                /// using a backoff algorithm that retries at longer and longer intervals until a connection is re-established,
                /// with a maximum delay of 10 minutes. Only applicable if options.live is also true.
                abstract retry: bool option with get, set
                /// Reference a filter function from a design document to selectively get updates.
                /// To use a view function, pass '_view' here and provide a reference to the view function in options.view.
                /// See filtered changes for details.
                abstract filter: U2<string,obj option -> obj option -> obj option> option with get, set
                /// Only show changes for docs with these ids (array of strings).
                abstract doc_ids: ResizeArray<string> option with get, set
                /// Object containing properties that are passed to the filter function, e.g. {"foo:"bar"},
                /// where "bar" will be available in the filter function as params.query.foo.
                /// To access the params, define your filter function like function (doc, params).
                abstract query_params: obj option with get, set
                /// Specify a view function (e.g. 'design_doc_name/view_name' or 'view_name' as shorthand for 'view_name/view_name') to act as a filter.
                /// Documents counted as “passed” for a view filter if a map function emits at least one record for them.
                /// Note: options.filter must be set to '_view' for this option to work.
                abstract view: string option with get, set
                /// Replicate changes after the given sequence number.
                abstract since: obj option with get, set
                /// Configure the heartbeat supported by CouchDB which keeps the change connection alive.
                abstract heartbeat: obj option with get, set
                /// Request timeout (in milliseconds).
                abstract timeout: U2<float,obj> option with get, set
                /// Number of change feed items to process at a time. Defaults to 100.
                /// This affects the number of docs and attachments held in memory and the number sent at a time to the target server.
                /// You may need to adjust downward if targeting devices with low amounts of memory
                /// e.g. or if the documents and/or attachments are large in size or if there are many conflicted revisions.
                /// If your documents are small in size, then increasing this number will probably speed replication up.
                abstract batch_size: int option with get, set
                /// Number of batches to process at a time. Defaults to 10.
                /// This (along wtih batch_size) controls how many docs are kept in memory at a time,
                /// so the maximum docs in memory at once would equal batch_size × batches_limit.
                abstract batches_limit: int option with get, set
                /// Backoff function to be used in retry replication. This is a function that takes the current
                /// backoff as input (or 0 the first time) and returns a new backoff in milliseconds.
                /// You can use this to tweak when and how replication will try to reconnect to a remote database when the user goes offline.
                /// Defaults to a function that chooses a random backoff between 0 and 2 seconds and doubles every time it fails to connect.
                /// The default delay will never exceed 10 minutes.
                abstract back_off_function: delay:float -> float

            type [<AllowNullLiteral>] ReplicationEventEmitter<'Content,'C,'F> =
                inherit EventEmitter
                [<Emit "$0.on('change',$1)">] abstract on_change: listener:('C -> obj option) -> ReplicationEventEmitter<'Content,'C,'F>
                abstract on: event:U3<string,string,string> * listener:(obj -> obj option) -> ReplicationEventEmitter<'Content,'C,'F>
                [<Emit "$0.on('active',$1)">] abstract on_active: listener:(unit -> obj option) -> ReplicationEventEmitter<'Content,'C,'F>
                [<Emit "$0.on('complete',$1)">] abstract on_complete: listener:('F -> obj option) -> ReplicationEventEmitter<'Content,'C,'F>
                abstract cancel: unit -> unit

            type [<AllowNullLiteral>] ReplicationResult<'Content> =
                abstract doc_write_failures: int with get, set
                abstract docs_read: int with get, set
                abstract docs_written: int with get, set
                abstract last_seq: int with get, set
                abstract start_time: DateTime with get, set
                abstract ok: bool with get, set
                abstract errors: ResizeArray<obj option> with get, set
                abstract docs: ResizeArray<Core.Document> with get, set

            type [<AllowNullLiteral>] ReplicationResultComplete<'Content> =
                inherit ReplicationResult<'Content>
                abstract end_time: DateTime with get, set
                abstract status: string with get, set

      // manually added
      type Database =

          abstract put: doc:Core.PutDocument * ?options:Core.Options -> Promise<Core.Response>
          abstract get: docId:Core.DocumentId * ?options:Core.Options -> Promise<Core.Document>
          abstract find: request:Find.FindRequest -> Promise<Find.FindResponse>
          abstract bulkDocs: options:Core.BulkDocsOptions -> Promise<ResizeArray<U2<Core.Response,Core.Error>>>
          abstract bulkDocs: docs:ResizeArray<Core.PutDocument> * options:Core.BulkDocsOptions -> Promise<ResizeArray<U2<Core.Response,Core.Error>>>
          abstract allDocs: options:Core.AllDocsOptions -> Promise<Core.AllDocsResponse<'Content>>
          abstract query: view:string * options:Query.Options option -> Promise<Core.AllDocsResponse<'Content>>
          abstract sync: remote:string * options:SyncOptions option -> SyncChanges
          abstract createIndex: index:Find.CreateIndexOptions -> Promise<Find.CreateIndexResponse>

          [<Emit("$0.replicate.to($1)")>]
          abstract ReplicateTo: remote:string -> Promise<Replication.ReplicationResultComplete<'Content>>
          [<Emit("$0.replicate.from($1)")>]
          abstract ReplicateFrom: remote:string -> Promise<Core.Document>
          [<Emit("$0.replicate.to($1,$2)")>]
          abstract Replicate: remote:Database * options:Replication.ReplicateOptions option -> Promise<Replication.ReplicationResultComplete<'Content>>

          abstract login: username:string * password:string -> Promise<Core.BasicResponse>
          abstract signup: username:string * password:string -> Promise<Core.Response>

          [<Emit("$0.getSession({ajax:{withCredentials:true}})")>]
          abstract getSession: unit -> Promise<Core.Document>

          abstract logout: unit -> Promise<Core.BasicResponse>
          abstract getUser: user:string -> Promise<Core.Document>
          abstract destroy: unit -> Promise<Core.BasicResponse>

          //abstract signup: credentials:LoginRequest * meta:obj option -> Promise<Core.Response>
          abstract putUser: user:string * meta:obj option -> Promise<Core.BasicResponse>
          abstract deleteUser: user:string -> Promise<Core.Response>
          abstract changePassword: username:string * password:string -> Promise<Core.Response>
          abstract changeUsername: oldusername:string * newusername:string -> Promise<Core.Response>
          abstract signUpAdmin: username:string * password:string * meta:obj option -> Promise<Core.Response>
          abstract deleteAdmin: user:string -> Promise<Core.Response>

      and CurrentUser =
        abstract name: string option with get, set
        abstract roles: string list option with get, set

      and SyncOptions =
        abstract live: bool with get, set
        abstract retry: bool with get, set
        abstract continuous: bool with get, set

      and SyncChangesChanges = obj

      and  [<AllowNullLiteral>] SyncChanges =
          inherit EventEmitter
          [<Emit("$0.on('paused',$1...)")>] abstract OnPaused: listener:Func<SyncChangesChanges,obj> -> unit
          [<Emit("$0.on('change',$1...)")>] abstract OnChange: listener:(SyncChangesChanges -> unit) -> unit
          [<Emit("$0.on('error',$1...)")>] abstract OnError: listener:Func<obj,obj> -> unit
          [<Emit("$0.on('active',$1...)")>] abstract OnActive: listener:Func<SyncChangesChanges,obj> -> unit

      and  [<AllowNullLiteral>] Static =
          inherit EventEmitter
          abstract version: string with get, set
  //        abstract debug: debug.IDebug with get, set
          abstract plugin: plugin:Plugin -> Static
          abstract on: event:(* TODO StringEnum created | destroyed *) string * listener:Func<string,obj> -> obj
          [<Emit("new $0($1,$2)")>] abstract Create: name:U2<string,unit> * options:Configuration.DatabaseConfiguration option -> Database
          [<Emit("new $0($1,$2)")>] abstract CreateLocal: name:U2<string,unit> * options:Configuration.LocalDatabaseConfiguration option -> Database
          [<Emit("new $0($1,$2)")>] abstract CreateRemote: name:U2<string,unit> * options:Configuration.RemoteDatabaseConfiguration option -> Database
