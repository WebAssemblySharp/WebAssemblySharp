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
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.itoa.wasm"));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();

        WebAssemblyUTF8String l_StringValue = await l_Module.Call<WebAssemblyUTF8String>("itoa", p_Number);
        Assert.AreEqual(Convert.ToString(p_Number), l_StringValue);
    }
}