using System.Threading.Tasks;

namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyMethod
{
    Task<object> Invoke(params object[] p_Args);
}