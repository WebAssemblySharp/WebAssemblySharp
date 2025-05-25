namespace WebAssemblySharp.MetaData;

public enum WasmDataType: byte
{
    Unkown = 0x00,
    I32 = 0x7F,
    I64 = 0x7E,
    F32 = 0x7D,
    F64 = 0x7C
}