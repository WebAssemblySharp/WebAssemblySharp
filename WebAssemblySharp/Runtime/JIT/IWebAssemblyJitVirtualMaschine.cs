using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public interface IWebAssemblyJitVirtualMaschine
{
    WebAssemblyJitExecutionContext CreateContext(WasmFuncType p_FuncType, WasmCode p_Code, object[] p_Args);
    Task<bool> ExecuteFrame(WebAssemblyJitExecutionContext p_Context, int p_InstructionsToExecute);
    Span<WebAssemblyJitValue> FinishContext(WebAssemblyJitExecutionContext p_Context);
    
}