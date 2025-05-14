using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITAssembly
{
    public IDictionary ExportedMethodes { get; private set; }
    
    public WebAssemblyJITAssembly(IDictionary p_ExportedMethods)
    {
        ExportedMethodes = p_ExportedMethods;
    }
}