using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class LocalsTest
{
    [TestMethod]
    public async Task return_defaultTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));
        
        int l_Result = await l_Module.Call<int>("return_default");
        Assert.AreEqual(0, l_Result);
    }
    
    [TestMethod]
    public async Task unnamed_localsTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));

        object[] l_Result = (Object[])await l_Module.DynamicCall("unnamed_locals", 11, 22);
        Assert.AreEqual(3, l_Result.Length);
        Assert.AreEqual(11, l_Result[0]);
        Assert.AreEqual(22, l_Result[1]);
        Assert.AreEqual(44, l_Result[2]);
    }
    
    [TestMethod]
    public async Task some_unnamedTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));

        object[] l_Result = (Object[])await l_Module.DynamicCall("some_unnamed", 11, 22);
        Assert.AreEqual(4, l_Result.Length);
        Assert.AreEqual(11, l_Result[0]);
        Assert.AreEqual(22, l_Result[1]);
        Assert.AreEqual(77, l_Result[2]);
        Assert.AreEqual(44, l_Result[3]);
    }
    
    [TestMethod]
    public async Task named_by_indexTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));

        object[] l_Result = (Object[])await l_Module.DynamicCall("named_by_index", 11, 22);
        Assert.AreEqual(3, l_Result.Length);
        Assert.AreEqual(11, l_Result[0]);
        Assert.AreEqual(22, l_Result[1]);
        Assert.AreEqual(88, l_Result[2]);
    }
    
    [TestMethod]
    public async Task multi_declTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));

        object[] l_Result = (Object[])await l_Module.DynamicCall("multi_decl", 11, 22);
        Assert.AreEqual(4, l_Result.Length);
        Assert.AreEqual(11, l_Result[0]);
        Assert.AreEqual(22, l_Result[1]);
        Assert.AreEqual(33, l_Result[2]);
        Assert.AreEqual(44, l_Result[3]);
    }
}