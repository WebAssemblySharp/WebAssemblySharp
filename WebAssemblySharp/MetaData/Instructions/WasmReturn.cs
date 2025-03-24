namespace WebAssemblySharp.MetaData.Instructions;

public class WasmReturn: WasmInstruction
{
    public static readonly WasmReturn Instance = new WasmReturn(); 
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.Return;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}