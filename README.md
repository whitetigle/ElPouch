# Disclaimer
:warning: This is a highly experimental project. While many things work, it should be noted that the Api may change. So for now use at your own risks.  :warning:

# ElPouch

Fable wrappers and helpers for [PouchDB](http://www.pouchdb.com)

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

## Docs

Today, there's no proper doc. 
But we do have easy to follow tests in the [`tests`](https://github.com/whitetigle/ElPouch/tree/master/tests) folder. So that's the easiest way to get started.


## Test suite

Type `yarn test` to prepare project and run tests locally.

