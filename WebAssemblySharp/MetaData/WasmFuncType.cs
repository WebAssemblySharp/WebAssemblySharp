namespace WebAssemblySharp.MetaData;

public class WasmFuncType
{
    public WasmDataType[] Parameters { get; set; }
    public WasmDataType[] Results { get; set; }

    public override string ToString()
    {
        if ((Parameters == null || Parameters.Length == 0) && (Results == null || Results.Length == 0))
        {
            return "(): void";
        }
        else if (Parameters == null || Parameters.Length == 0)
        {
            return $"(): ({string.Join(", ", Results)})";
        }
        else if (Results == null || Results.Length == 0)
        {
            return $"({string.Join(", ", Parameters)}): void";
        }
        else
        {
            return $"({string.Join(", ", Parameters)}): ({string.Join(", ", Results)})";
        }
    }
}