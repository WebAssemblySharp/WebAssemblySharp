using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitImportMethod
{
    public WasmImport WasmImport { get; private set; }
    public WasmFuncType FuncType { get; private set; }
    public Delegate Delegate { get; private set; }

    public WebAssemblyJitImportMethod(WasmImport p_WasmImport, WasmFuncType p_FuncType, Delegate p_Delegate)
    {
        WasmImport = p_WasmImport;
        FuncType = p_FuncType;
        Delegate = p_Delegate;
    }
}