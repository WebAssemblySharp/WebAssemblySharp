using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
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

        object[] l_Objects = (Object[]) await l_Module.Call("itoa", p_Number);
        Assert.AreEqual(2, l_Objects.Length);

        Span<byte> l_Access = l_Module.GetMemoryAccess((int)l_Objects[1], (int)l_Objects[0]);
        string l_Result = System.Text.Encoding.UTF8.GetString(l_Access);
        Assert.AreEqual(Convert.ToString(p_Number), l_Result);
    }
}