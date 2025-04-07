namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Store8: WasmInstruction
{
    public static readonly WasmI32Store8 Instance = new WasmI32Store8();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Store8;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}