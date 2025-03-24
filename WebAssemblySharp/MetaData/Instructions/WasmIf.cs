using System;
using System.Collections.Generic;

namespace WebAssemblySharp.MetaData.Instructions;

public class WasmIf: WasmBlockInstruction
{
    public IEnumerable<WasmInstruction> IfBody { get; set; }
    public IEnumerable<WasmInstruction> ElseBody { get; set; }
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.If;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        // Read the block type
        ReadOnlySpan<byte> l_ReadBytes = p_Reader.ReadReadBytes(1);

        if (l_ReadBytes.IsEmpty)
            return false;
        
        BlockType = (WasmBlockType)l_ReadBytes[0];
        IfBody = new List<WasmInstruction>();
        
        return true;
    }

    public override void AddInstruction(WasmInstruction p_Instruction)
    {
        if (ElseBody != null)
        {
            ((List<WasmInstruction>)ElseBody).Add(p_Instruction);
        }
        else
        {
            ((List<WasmInstruction>)IfBody).Add(p_Instruction);
        }
        
        
    }

    public override void HandleBlockOpCode(WasmOpcode p_Opcode)
    {
        if (p_Opcode == WasmOpcode.Else)
        {
            ElseBody = new List<WasmInstruction>();
        }
        
        base.HandleBlockOpCode(p_Opcode);
    }

    public override void Finished()
    {
        base.Finished();
        
        // Optimize it to an Array
        IfBody = ((List<WasmInstruction>)IfBody).ToArray();
        
        if (ElseBody == null)
        {
            ElseBody = Array.Empty<WasmInstruction>();
        }
        else
        {
            ElseBody = ((List<WasmInstruction>)ElseBody).ToArray();
        }
    }
}
