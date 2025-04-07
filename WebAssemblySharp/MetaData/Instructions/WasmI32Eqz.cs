namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Eqz: WasmInstruction
{
    public static readonly WasmI32Eqz Instance = new WasmI32Eqz();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Eqz;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}