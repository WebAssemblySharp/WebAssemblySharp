namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Eq: WasmInstruction
{
    public static readonly WasmI32Eq Instance = new WasmI32Eq();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Eq;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}