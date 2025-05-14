namespace WebAssemblySharp.MetaData;

public class WasmImportFunction: WasmImport
{
    public override WasmExternalKind Kind { get { return WasmExternalKind.Function; } }
    public long FunctionIndex { get; set; }
}