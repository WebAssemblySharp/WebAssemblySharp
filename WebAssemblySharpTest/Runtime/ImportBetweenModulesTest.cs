using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.Interpreter;
using WebAssemblySharp.Runtime.JIT;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class ImportBetweenModulesTest
{
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task ExecuteImportBetweenModulesTest(Type p_RuntimeType)
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create(p_RuntimeType);
        await l_RuntimeBuilder.LoadModule("mod1", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod1.wasm"));
        await l_RuntimeBuilder.LoadModule("mod2", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod2.wasm"));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();

        WebAssemblyModule l_Module = l_Runtime.GetModule("mod2");

        int l_Result = await l_Module.Call<int, int>("twiceplus5", 10);
        
        Assert.AreEqual(25, l_Result);
    }
    
    
    [TestMethod]
    public async Task ExecuteMultiRuntimeImportBetweenModulesTest()
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("mod1", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod1.wasm"), null, null, typeof(WebAssemblyInterpreterExecutor));
        await l_RuntimeBuilder.LoadModule("mod2", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod2.wasm"), null, null, typeof(WebAssemblyJITExecutor));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();

        WebAssemblyModule l_Module = l_Runtime.GetModule("mod2");

        int l_Result = await l_Module.Call<int, int>("twiceplus5", 10);
        
        Assert.AreEqual(25, l_Result);
    }
    
}