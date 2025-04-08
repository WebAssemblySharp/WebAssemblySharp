using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.Interpreter;

public class WebAssemblyInterpreterImportMethod
{
    public WasmImport WasmImport { get; private set; }
    public WasmFuncType FuncType { get; private set; }
    public Delegate Delegate { get; private set; }

    public WebAssemblyInterpreterImportMethod(WasmImport p_WasmImport, WasmFuncType p_FuncType, Delegate p_Delegate)
    {
        WasmImport = p_WasmImport;
        FuncType = p_FuncType;
        Delegate = p_Delegate;
    }
}