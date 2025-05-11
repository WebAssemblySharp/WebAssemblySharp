using System.Collections.Generic;
using WebAssemblySharp.Runtime.Interpreter;
using WebAssemblySharp.Runtime.JIT;

namespace WebAssemblySharpTest.Runtime;

public class TestRuntimeProvider
{
    public static IEnumerable<object[]> RuntimeTypes
    {
        get
        {
            yield return new object[] { typeof(WebAssemblyInterpreterExecutor) };
            yield return new object[] { typeof(WebAssemblyJITExecutor) };
        }
    }
    
}