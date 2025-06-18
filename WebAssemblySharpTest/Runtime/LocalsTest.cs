using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class LocalsTest
{
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task return_defaultTest(Type p_RuntimeType)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));
        
        int l_Result = await l_Module.Call<int>("return_default");
        Assert.AreEqual(0, l_Result);
    }
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task unnamed_localsTest(Type p_RuntimeType)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));

        (int, int, int) l_Tuple = await l_Module.Call<ValueTuple<int, int, int>, int, int>("unnamed_locals", 11, 22);
        
        Assert.AreEqual(11, l_Tuple.Item1);
        Assert.AreEqual(22, l_Tuple.Item2);
        Assert.AreEqual(44, l_Tuple.Item3);
    }
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task some_unnamedTest(Type p_RuntimeType)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));

        (int, int, int, int) l_Tuple = await l_Module.Call<ValueTuple<int, int, int, int>, int, int>("some_unnamed", 11, 22);
        Assert.AreEqual(11, l_Tuple.Item1);
        Assert.AreEqual(22, l_Tuple.Item2);
        Assert.AreEqual(77, l_Tuple.Item3);
        Assert.AreEqual(44, l_Tuple.Item4);
    }
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task named_by_indexTest(Type p_RuntimeType)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));

        (int, int, int) l_Tuple = await l_Module.Call<ValueTuple<int, int, int>, int, int>("named_by_index", 11, 22);
        Assert.AreEqual(11, l_Tuple.Item1);
        Assert.AreEqual(22, l_Tuple.Item2);
        Assert.AreEqual(88, l_Tuple.Item3);
    }
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task multi_declTest(Type p_RuntimeType)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));

        (int, int, int, int) l_Tuple = await l_Module.Call<ValueTuple<int, int, int, int>, int, int>("multi_decl", 11, 22);
        Assert.AreEqual(11, l_Tuple.Item1);
        Assert.AreEqual(22, l_Tuple.Item2);
        Assert.AreEqual(33, l_Tuple.Item3);
        Assert.AreEqual(44, l_Tuple.Item4);
    }
}