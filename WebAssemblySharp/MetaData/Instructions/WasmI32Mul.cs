namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Mul: WasmInstruction
{
    public static readonly WasmI32Mul Instance = new WasmI32Mul();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Mul;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}