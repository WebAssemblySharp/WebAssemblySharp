using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.JIT;
using WebAssemblySharp.Runtime.Values;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class ItoaTest
{
    public static IEnumerable<object[]> Numbers
    {
        get
        {
            foreach (object[] l_Type in TestRuntimeProvider.RuntimeTypes)
            {
                for (int i = 0; i < 10000; i++)
                {
                    yield return new object[] { l_Type[0], i };
                }
            }
        }
    }


    [TestMethod]
    [DynamicData(nameof(Numbers))]
    public async Task ExecuteItoaTest(Type p_RuntimeType, int p_Number)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.itoa.wasm"));

        WebAssemblyUTF8String l_StringValue = await l_Module.CallAndConvert<WebAssemblyUTF8String, int>("itoa", p_Number);
        Assert.AreEqual(Convert.ToString(p_Number), l_StringValue);
    }
    
}