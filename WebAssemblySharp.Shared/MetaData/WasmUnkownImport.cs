namespace WebAssemblySharp.MetaData;

public class WasmUnkownImport: WasmImport
{
    public override WasmExternalKind Kind { get { return WasmExternalKind.Unknown; } }
}