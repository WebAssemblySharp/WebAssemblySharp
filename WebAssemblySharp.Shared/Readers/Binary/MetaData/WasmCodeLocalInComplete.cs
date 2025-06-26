using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Readers.Binary.MetaData;

public class WasmCodeLocalInComplete
{
    public long Number { get; set; }
    public WasmDataType ValueType { get; set; }
}