﻿namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32LtU: WasmInstruction
{
    public static readonly WasmI32LtU Instance = new WasmI32LtU();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32LtU;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}