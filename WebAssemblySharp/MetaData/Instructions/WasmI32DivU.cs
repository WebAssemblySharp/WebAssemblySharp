namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32DivU: WasmInstruction
{
    public static readonly WasmI32DivU Instance = new WasmI32DivU(); 
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32DivU;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}