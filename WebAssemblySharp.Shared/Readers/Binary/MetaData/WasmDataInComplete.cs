using System.Collections.Generic;
using System.Linq;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Readers.Binary.MetaData;

public class WasmDataInComplete: WasmData
{
    public List<WasmInstruction> ProcessedInstructions { get; private set; }
    
    public WasmDataInComplete()
    {
        ProcessedInstructions = new List<WasmInstruction>();
    }
    
    public WasmData ToCompleted()
    {
        WasmData l_Data = new WasmData();
        l_Data.MemoryIndex = MemoryIndex;
        l_Data.Data = Data;
        l_Data.OffsetInstructions = ProcessedInstructions.ToArray();
        return l_Data;
    }
}