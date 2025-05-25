namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Extend8S: WasmInstruction
{
    public static readonly WasmI32Extend8S Instance = new WasmI32Extend8S();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Extend8_s;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}