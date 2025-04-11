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

    public void DefineImport(string p_Name, Delegate p_Delegate)
    {
        
    }

    public async Task Init()
    {
        // Best Case load the precompiled assembly
    }

    public Span<byte> GetMemoryAccess(long p_Address, int p_Length)
    {
        throw new NotImplementedException();
    }
}