using System;
using System.Threading.Tasks;
using WebAssemblySharp.Attributes;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class AddInterfaceTest
{
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task ExecuteAddWasmTest(Type p_RuntimeType)
    {
        IAdd l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime<IAdd>(p_RuntimeType);
        int l_Result = await l_Module.add( 1, 2);
        Assert.AreEqual(3, l_Result);
    }
    
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task ExecuteAddAutoGenWasmTest(Type p_RuntimeType)
    {
        IAddAutoGen l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime<IAddAutoGen>(p_RuntimeType);
        int l_Result = await l_Module.add( 1, 2);
        Assert.AreEqual(3, l_Result);
    }
    
}

[WebAssemblyModuleManifestResource("WebAssemblySharpExampleData.Programms.add.wasm", typeof(WebAssemblyExamples))]
[WebAssemblyModuleDefinition("add")]
public interface IAdd
{
    public ValueTask<int> add(int p_A, int p_B);
}

[WebAssemblyModuleManifestResource("WebAssemblySharpExampleData.Programms.add.wasm", typeof(WebAssemblyExamples))]
[WebAssemblyModuleDefinition("add")]
public partial interface IAddAutoGen
{
        
}

