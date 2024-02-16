# PrimeView

![CI](https://github.com/rbergen/PrimeView/actions/workflows/azure-static-web-apps-agreeable-mud-0b27ba210.yml/badge.svg)
![CI](https://github.com/PlummersSoftwareLLC/PrimeView/actions/workflows/github-pages.yml/badge.svg)

This is a Blazor WebAssembly static in-browser web application to view benchmark reports generated in/for the [Primes](https://github.com/PlummersSoftwareLLC/Primes) project.

The application loads benchmark reports from an API that has been developed and published for the purpose. More information about the API reader can be found in [RestAPIReader/README.md](src/RestAPIReader/README.md).

Previously, the application loaded benchmark reports in JSON format, either from a configured location or using a default approach. The report reader in question is still included; more information on how it works can be found in [JsonFileReader/README.md](src/JsonFileReader/README.md).

As the report reader back-ends are isolated from the front-end (and added via dependency injection), it's easy to add and use another report provider.

## Building

The solution can be built by running the following commands from the repository root directory, once [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) is installed:

```shell
dotnet workload install wasm-tools-net6
dotnet publish
```

Note that:

* the first command installs the tools required for AOT compilation. That command has to be executed only once for any system PrimeView is built on.
* the AOT compilation can take multiple minutes to complete.

At the end of the build process, the location of the build output will be indicated in the following line:

```shell
Frontend -> <repo root>\src\Frontend\bin\Release\net8.0\publish\
```

## Implementation notes

Where applicable, implementation notes can be found in README.md files in the directories for the respective C#/Blazor projects.

## Attribution

* The source code that gets and sets query string parameters is based on [a blog post](https://www.meziantou.net/bind-parameters-from-the-query-string-in-blazor.htm) by G&eacute;rald Barr&eacute;.
* Local storage is implemented using [Blazored LocalStorage](https://github.com/Blazored/LocalStorage).
* The tables of report summaries and report results are implemented using [BlazorTable](https://github.com/IvanJosipovic/BlazorTable).
* The checkered flag in favicon.ico was made by [Freepik](https://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/).
