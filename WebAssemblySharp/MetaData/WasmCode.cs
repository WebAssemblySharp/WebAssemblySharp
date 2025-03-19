using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.MetaData;

public class WasmCode
{
    public long CodeSize { get; set; }
    public WasmCodeLocal[] Locals { get; set; }
    public WasmInstruction[] Instructions { get; set; }
}