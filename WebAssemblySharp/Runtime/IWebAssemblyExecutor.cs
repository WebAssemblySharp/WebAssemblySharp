using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyExecutor
{
    IWebAssemblyMethod GetMethod(string p_Name);

    void LoadCode(WasmMetaData p_WasmMetaData);
}