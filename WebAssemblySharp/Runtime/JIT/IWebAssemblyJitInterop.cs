namespace WebAssemblySharp.Runtime.JIT;

public interface IWebAssemblyJitInterop
{
    void CallFunction(uint p_FunctionIndex, IWebAssemblyJitInteropStack p_Stack);
}