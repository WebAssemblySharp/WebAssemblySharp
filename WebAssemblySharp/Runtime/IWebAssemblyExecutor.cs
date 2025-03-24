namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyExecutor
{
    IWebAssemblyMethod GetMethod(string p_Name);
    
}