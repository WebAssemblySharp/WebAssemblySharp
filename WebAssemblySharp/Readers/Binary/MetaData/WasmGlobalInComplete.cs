using System;
using System.Collections.Generic;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Readers.Binary.MetaData;

public class WasmGlobalInComplete: WasmGlobal
{
    public Boolean? MutableReaded;
    public List<WasmInstruction> ProcessedInstructions { get; private set; }
    
    public WasmGlobalInComplete()
    {
        ProcessedInstructions = new List<WasmInstruction>();
    }
    
    public WasmGlobal ToCompleted()
    {
        WasmGlobal l_Global = new WasmGlobal();
        l_Global.Type = Type;
        l_Global.Mutable = MutableReaded.Value;
        l_Global.InitInstructions = ProcessedInstructions.ToArray();
        return l_Global;
    }
    
}