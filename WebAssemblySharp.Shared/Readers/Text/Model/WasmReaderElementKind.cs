namespace WebAssemblySharp.Readers.Text.Model;

public enum WasmReaderElementKind
{
    OpenElement,
    CloseElement,
    Module,
    Function,
    Export,
    Param,
    Result,
    I32,
    I64,
    F32,
    F64,
    Local,
    Table,
    Mem,
    Global,
    Data,
    Text
}