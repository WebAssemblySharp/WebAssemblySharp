using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public interface IWebAssemblyJitVirtualMaschine
{
    WebAssemblyJitExecutionContext CreateContext(WasmFuncType p_FuncType, WasmCode p_Code, WebAssemblyJitStackFrame p_Frame);
    bool ExecuteFrame(ref WebAssemblyJitExecutionContext p_Context, int p_InstructionsToExecute);
    Span<WebAssemblyJitValue> FinishContext(ref WebAssemblyJitExecutionContext p_Context);
    
}