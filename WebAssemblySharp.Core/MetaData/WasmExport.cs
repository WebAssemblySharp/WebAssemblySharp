namespace WebAssemblySharp.MetaData;

public class WasmExport
{
    public WasmString Name { get; set; }
    public WasmExternalKind Kind { get; set; }
    public long Index { get; set; }
}