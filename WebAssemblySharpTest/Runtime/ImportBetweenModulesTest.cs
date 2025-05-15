using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class ImportBetweenModulesTest
{
    [TestMethod]
    public async Task ExecuteImportBetweenModulesTest()
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("mod1", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod1.wasm"));
        await l_RuntimeBuilder.LoadModule("mod2", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod2.wasm"));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();

        WebAssemblyModule l_Module = l_Runtime.GetModule("mod2");

        int l_Result = await l_Module.Call<int, int>("twiceplus5", 10);
        
        Assert.AreEqual(25, l_Result);
    }
    
}