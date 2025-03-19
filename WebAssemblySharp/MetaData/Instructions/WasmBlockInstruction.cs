using System;

namespace WebAssemblySharp.MetaData.Instructions;

public abstract class WasmBlockInstruction: WasmInstruction
{
    public WasmBlockType BlockType { get; set; }
    
    public abstract void AddInstruction(WasmInstruction p_Instruction);

    public virtual void HandleBlockOpCode(WasmOpcode p_Opcode)
    {
        throw new Exception($"Invalid Block Opcode {p_Opcode} for block instruction {GetOpcode()}");
    }
}