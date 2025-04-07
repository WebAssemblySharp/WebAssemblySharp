using System;
using System.Collections.Generic;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.MetaData;

public class WasmData
{
    public uint MemoryIndex { get; set; }
    public IEnumerable<WasmInstruction> OffsetInstructions { get; set; }
    public WasmBinaryData Data { get; set; }
    
    
}