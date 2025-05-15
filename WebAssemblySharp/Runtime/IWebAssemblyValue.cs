namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyValue
{
    void Load(object p_Result, IWebAssemblyValueAccess p_Executor);
}