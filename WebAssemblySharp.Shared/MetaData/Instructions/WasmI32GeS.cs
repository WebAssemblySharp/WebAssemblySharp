namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32GeS: WasmInstruction
{
    public static readonly WasmI32GeS Instance = new WasmI32GeS();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32GeS;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}