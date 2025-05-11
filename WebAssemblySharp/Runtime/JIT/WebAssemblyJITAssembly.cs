using System.Collections.Generic;
using System.Reflection;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITAssembly
{
    public Dictionary<string, IWebAssemblyMethod> ExportedMethodes { get; private set; }
    
    public WebAssemblyJITAssembly(Dictionary<string, IWebAssemblyMethod> p_ExportedMethods)
    {
        ExportedMethodes = p_ExportedMethods;
    }
}