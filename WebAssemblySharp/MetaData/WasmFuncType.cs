namespace WebAssemblySharp.MetaData;

public class WasmFuncType
{
    public WasmDataType[] Parameters { get; set; }
    public WasmDataType[] Results { get; set; }
}