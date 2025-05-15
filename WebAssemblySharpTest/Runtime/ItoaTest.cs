using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
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
            for (int i = 0; i < 10000; i++)
            {
                yield return new object[] { i };
            }
        }
    }

    
    [TestMethod]
    [DynamicData(nameof(Numbers))]
    public async Task ExecuteItoaTest(int p_Number)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.itoa.wasm"));

        WebAssemblyUTF8String l_StringValue = await l_Module.Call<WebAssemblyUTF8String, int>("itoa", p_Number);
        Assert.AreEqual(Convert.ToString(p_Number), l_StringValue);
    }
}