namespace WebAssemblySharp.MetaData;

public class WasmImportMemory: WasmImport
{
    public override WasmExternalKind Kind { get { return WasmExternalKind.Memory; } }
    
    public long Min { get; set; }
    public long Max { get; set; }
}