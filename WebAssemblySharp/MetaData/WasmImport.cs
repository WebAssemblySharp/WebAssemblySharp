namespace WebAssemblySharp.MetaData;

public class WasmImport
{
    public WasmString Module { get; set; }
    public WasmString Name { get; set; }
    public WasmExternalKind Kind { get; set; }
    
    public long Index { get; set; }
    
}