using System;
using System.Collections.Generic;
using System.Linq;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Readers.Binary.MetaData;

public class WasmCodeInComplete: WasmCode
{
    public long CodeSizeRemaining;
    public List<WasmInstruction> ProcessedInstructions { get; private set; }
    public WasmCodeLocalInComplete[] ProcessedLocals { get; set; }

    public WasmCodeInComplete()
    {
        ProcessedInstructions = new List<WasmInstruction>();
    }

    public WasmCode ToCompleted()
    {
        WasmCode l_WasmCode = new WasmCode();
        l_WasmCode.CodeSize = CodeSize;
        
        if (ProcessedLocals != null && ProcessedLocals.Length > 0)
        {
            // Expand the locals to the correct size
            // In the binary format, if locals 2 and 3 are i32 and local 4 is f64, they would be encoded as:
            // 2 locals of type i32
            // 1 local of type f64
            
            List<WasmDataType> l_Locals = new List<WasmDataType>();
            
            foreach (WasmCodeLocalInComplete l_Local in ProcessedLocals)
            {
                for (int i = 0; i < l_Local.Number; i++)
                {
                    l_Locals.Add(l_Local.ValueType);
                }
            }
            
            l_WasmCode.Locals = l_Locals.ToArray();
        }
        else
        {
            l_WasmCode.Locals = Array.Empty<WasmDataType>();    
        }
        
        
        l_WasmCode.Instructions = ProcessedInstructions.ToArray();
        return l_WasmCode;
    }
}