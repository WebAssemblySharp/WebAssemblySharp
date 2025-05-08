using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

// Starting Point for the JIT/ILCompiler
public class WebAssemblyJITExecutor: IWebAssemblyExecutor
{
    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        throw new NotImplementedException();
    }

    public void LoadCode(WasmMetaData p_WasmMetaData)
    {
        throw new NotImplementedException();
    }

    public void OptimizeCode()
    {
        // QQQ Precompile global wasm code maybe also other functions  
    }

    public IWebAssemblyMemoryArea GetMemoryArea(string p_Name)
    {
        throw new NotImplementedException();
    }

    public void ImportMemoryArea(string p_Name, IWebAssemblyMemoryArea p_Memory)
    {
        
    }

    public void ImportMethod(string p_Name, Delegate p_Delegate)
    {
        
    }

    public IWebAssemblyMemoryAreaReadAccess GetInternalMemoryArea(int p_Index = 0)
    {
        throw new NotImplementedException();
    }

    public async Task Init()
    {
        // Best Case load the precompiled assembly
    }
}