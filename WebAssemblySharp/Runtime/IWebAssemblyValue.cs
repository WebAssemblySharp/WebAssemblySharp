namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyValue
{
    void Load(object p_Result, IWebAssemblyExecutor p_Executor);
}