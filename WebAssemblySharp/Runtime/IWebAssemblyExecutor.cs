using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyExecutor
{
    IWebAssemblyMethod GetMethod(string p_Name);

    void LoadCode(WasmMetaData p_WasmMetaData);
    
    void OptimizeCode();
    
    void DefineImport(string p_Name, Delegate p_Delegate);
    void Init();
}