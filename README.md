# Disclaimer
:warning: This is a highly experimental project. While many things work, it should be noted that the Api may change. So for now use at your own risks.  :warning:

# ElPouch

Fable wrappers and helpers for [PouchDB](http://pouchdb.com)

Package | Stable | Prerelease
--- | --- | ---
ElPouch | [![NuGet Badge](https://buildstats.info/nuget/ElPouch)](https://www.nuget.org/packages/ElPouch/) | [![NuGet Badge](https://buildstats.info/nuget/ElPouch?includePreReleases=true)](https://www.nuget.org/packages/ElPouch/)

**Install with paket:**
```
paket add ElPouch --project /path/to/Project.fsproj
```

## Goals
ElPouch wants to easy things when it comes to using pouchDb with Fable and F#.

The library is powered by well known Fable libs:
- [Fable.Elmish](https://github.com/elmish/elmish)
- [Thoth.Json](https://github.com/MangelMaxime/Thoth)
- [Fable.PowerPack](https://github.com/fable-compiler/fable-powerpack)

The package comes with: 
- Core library bindings: **PouchDB.Core**
- Helpers : **ElPouch.Relax**
- Helpers for Elmish: **ElPouch.Elmish.Relax**

While the core lib works on quite any JS project and have been tested, Helpers have been designed in a very *opiniated* way: they fit your Fable + Elmish + Thoth project.

## Test suite

Type `yarn test` to prepare project and run tests locally.

## How to start?

Today, there's no proper doc. 
But we do have easy to follow tests in the [`tests`](https://github.com/whitetigle/ElPouch/tree/master/tests) folder. So that's the easiest way to get started.

# What's ready to use?

The bindings are already to Use. Simply import `PouchDB.Core` and start playing with PouchDB!
Samples are ready to follow in the test suite.

## Implemented Operations
- PUT to insert, update and delete
- GET
- BULK INSERT
- ALLDOCS

# Next 
The order is random and this is work in progress:

- Put vs post: managing optional ids and removing the need for System.Guid
- allDocs for queries
- allDocs with pagination
- replication
- map/reduce queries
- proper test suites: mimic and comply with pouchdb offical test suites.
- db management (compaction, pouchdb adapters)
- Relax: easy to use helpers for everything with concrete cases (manage conflicts, auto-setup adapters, etc...)
