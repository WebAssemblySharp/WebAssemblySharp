namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyMethod
{
    object Invoke(params object[] p_Args);
}