namespace WebAssemblySharp.Runtime.JIT;

public interface IWebAssemblyJitInteropStack
{
    WebAssemblyJitValue PopFromStack();
    void PushToStack(WebAssemblyJitValue p_Value);
}