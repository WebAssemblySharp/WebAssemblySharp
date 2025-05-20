using System.Collections.Generic;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.MetaData;

public class WasmGlobal
{
    public WasmDataType Type { get; set; }
    public bool Mutable { get; set; }
    public IEnumerable<WasmInstruction> InitInstructions { get; set; }
}