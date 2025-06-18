# WebAssemblySharp

WebAssemblySharp is a WebAssembly runtime for C#, enabling the execution of WebAssembly modules within C# applications.

## Features
- Wasm Module Execution: Load and execute WebAssembly modules within C# applications.

## Getting Started

See the [root README.md](../README.md) for full installation and usage instructions.

## Example

```csharp
WebAssemblyModule module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
    typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.add.wasm"));
int result = await module.Call<int, int, int>("add", 1, 2);
```

For more details and advanced usage, see the [README.md](https://github.com/WebAssemblySharp/WebAssemblySharp/).
