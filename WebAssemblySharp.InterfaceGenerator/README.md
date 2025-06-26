# WebAssemblySharp.InterfaceGenerator

A Roslyn Source Generator for automatically generating C# interfaces for your WebAssembly modules.

## Usage

Add the following package reference to your project:

```xml
<PackageReference Include="WebAssemblySharp.InterfaceGenerator" Version="VersionToUse" OutputItemType="Analyzer" ReferenceOutputAssembly="false" Aliases="InternalWasmSourceGenerator"/>
```

### Example

```csharp
using WebAssemblySharp.Attributes;

[WebAssemblyModuleManifestResource("WebAssemblySharpExampleData.Programms.add.wasm", typeof(WebAssemblyExamples))]
[WebAssemblyModuleDefinition("add")]
public partial interface IAdd {}

// The simplest way to use the generated interface:
IAdd add = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime<IAdd>();
int result = await add.add(1, 2);
```

For more details and advanced usage, see the [README.md](https://github.com/WebAssemblySharp/WebAssemblySharp/).
