namespace WebAssemblySharp.Runtime.Interpreter;

public interface IWebAssemblyInterpreterInteropStack
{
    WebAssemblyInterpreterValue PopFromStack();
    void PushToStack(WebAssemblyInterpreterValue p_Value);
}