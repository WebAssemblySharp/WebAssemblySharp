namespace WebAssemblySharp.MetaData;

public class WasmMetaData
{
    public WasmCode[] Code { get; set; }
    public WasmExport[] Export { get; set; }
    
    public WasmImport[] Import { get; set; }
    public long[] FuncIndex { get; set; }
    public WasmFuncType[] FunctionType { get; set; }

    public WasmMemory[] Memory { get; set; }
    
    public WasmGlobal[] Globals { get; set; }
    
    public WasmData[] Data { get; set; }
    public int Version { get; set; }
}