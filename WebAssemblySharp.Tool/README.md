# WebAssemblySharp.Tool

A .NET CLI tool for generating C# source code from WebAssembly modules.

## Installation

```bash
dotnet tool install --global WebAssemblySharp.Tool --version VersionToUse
```

## Usage

```bash
wasmsharp generate path\myWasm.wasm Path\Generated\Prime
```

This command generates C# source files from the specified `.wasm` file into the output directory.

For more details and advanced usage, see the [README.md](https://github.com/WebAssemblySharp/WebAssemblySharp/).
