using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class LoopsTest
{
    //[TestMethod]
    public async Task ExecuteAddAllTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.loops.wasm"));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();

        int l_Sum = await l_Module.Call<int>("add_all", 4, 1);
        Assert.AreEqual(4, l_Sum);
    }
}