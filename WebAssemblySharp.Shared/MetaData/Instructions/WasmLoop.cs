using System.Collections.Generic;
#if NETSTANDARD2_0
using WebAssemblySharp.Polyfills;
#else
using System;
#endif

namespace WebAssemblySharp.MetaData.Instructions;

public class WasmLoop: WasmBlockInstruction
{
    public IEnumerable<WasmInstruction> Body { get; set; }
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.Loop;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        
        // Read the block type
        ReadOnlySpan<byte> l_ReadBytes = p_Reader.ReadReadBytes(1);

        if (l_ReadBytes.IsEmpty)
            return false;
        
        BlockType = (WasmBlockType)l_ReadBytes[0];
        Body = new List<WasmInstruction>();

        return true;
    }

    public override void AddInstruction(WasmInstruction p_Instruction)
    {
        ((List<WasmInstruction>)Body).Add(p_Instruction);
    }

    public override void Finished()
    {
        base.Finished();
        
        // Optimize it to an Array
        Body = ((List<WasmInstruction>)Body).ToArray();
    }

    public override IEnumerable<WasmInstruction> GetAllInstructions()
    {
        return Body;
    }
}