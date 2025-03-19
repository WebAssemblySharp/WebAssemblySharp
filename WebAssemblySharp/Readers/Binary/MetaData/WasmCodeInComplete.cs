using System.Collections.Generic;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Readers.Binary.MetaData;

public class WasmCodeInComplete: WasmCode
{
    public long CodeSizeRemaining;
    public List<WasmInstruction> ProcessedInstructions { get; private set; }

    public WasmCodeInComplete()
    {
        ProcessedInstructions = new List<WasmInstruction>();
    }

    public WasmCode ToCompleted()
    {
        WasmCode l_WasmCode = new WasmCode();
        l_WasmCode.CodeSize = CodeSize;
        l_WasmCode.Locals = Locals;
        l_WasmCode.Instructions = ProcessedInstructions.ToArray();
        return l_WasmCode;
    }
}