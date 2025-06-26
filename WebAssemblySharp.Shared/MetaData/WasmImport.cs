namespace WebAssemblySharp.MetaData;

public abstract class WasmImport
{
    public WasmString Module { get; set; }
    public WasmString Name { get; set; }
    public abstract WasmExternalKind Kind { get; }
}