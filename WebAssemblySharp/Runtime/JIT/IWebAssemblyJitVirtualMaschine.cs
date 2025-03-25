using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public interface IWebAssemblyJitVirtualMaschine
{
    WebAssemblyJitValue[] ExecuteFrame(WasmFuncType p_FuncType, WasmCode p_Code, WebAssemblyJitStackFrame p_Frame);
}