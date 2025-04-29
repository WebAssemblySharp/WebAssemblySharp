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
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();

        int l_Result = await l_Module.Call<int>("return_default");
        Assert.AreEqual(0, l_Result);
    }
    
    [TestMethod]
    public async Task unnamed_localsTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();

        object[] l_Result = (Object[])await l_Module.Call("unnamed_locals", 11, 22);
        Assert.AreEqual(3, l_Result.Length);
        Assert.AreEqual(11, l_Result[0]);
        Assert.AreEqual(22, l_Result[1]);
        Assert.AreEqual(44, l_Result[2]);
    }
    
    [TestMethod]
    public async Task some_unnamedTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();

        object[] l_Result = (Object[])await l_Module.Call("some_unnamed", 11, 22);
        Assert.AreEqual(4, l_Result.Length);
        Assert.AreEqual(11, l_Result[0]);
        Assert.AreEqual(22, l_Result[1]);
        Assert.AreEqual(77, l_Result[2]);
        Assert.AreEqual(44, l_Result[3]);
    }
    
    [TestMethod]
    public async Task named_by_indexTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();

        object[] l_Result = (Object[])await l_Module.Call("named_by_index", 11, 22);
        Assert.AreEqual(3, l_Result.Length);
        Assert.AreEqual(11, l_Result[0]);
        Assert.AreEqual(22, l_Result[1]);
        Assert.AreEqual(88, l_Result[2]);
    }
    
    [TestMethod]
    public async Task multi_declTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.locals.wasm"));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();

        object[] l_Result = (Object[])await l_Module.Call("multi_decl", 11, 22);
        Assert.AreEqual(4, l_Result.Length);
        Assert.AreEqual(11, l_Result[0]);
        Assert.AreEqual(22, l_Result[1]);
        Assert.AreEqual(33, l_Result[2]);
        Assert.AreEqual(44, l_Result[3]);
    }
}