namespace WebAssemblySharp.MetaData;

public enum WasmExternalKind: byte
{
    Function = 0x00,
    Table = 0x01,
    Memory = 0x02,
    Global = 0x03,
    Unknown = 0xFF   
}