using System;

namespace WebAssemblySharp.MetaData.Instructions;

public class WasmIf: WasmBlockInstruction
{
    public WasmInstruction[] IfBody { get; set; }
    public WasmInstruction[] ElseBody { get; set; }
    
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
        
        return true;
    }

    public override void AddInstruction(WasmInstruction p_Instruction)
    {
        
        
        
    }

    public override void HandleBlockOpCode(WasmOpcode p_Opcode)
    {
        if (p_Opcode == WasmOpcode.Else)
        {
            ElseBody = new WasmInstruction[0];
        }
        
        base.HandleBlockOpCode(p_Opcode);
    }
}
