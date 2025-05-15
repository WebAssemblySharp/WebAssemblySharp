using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class IfexprTest
{
    
    public static IEnumerable<object[]> Numbers
    {
        get
        {
            yield return new object[] { 100, 0, 101 };
            yield return new object[] { 100, 1, 101 };
            yield return new object[] { 100, 10, 101 };
            yield return new object[] { 100, -1, 99 };
            yield return new object[] { 100, -2, 99 };
            yield return new object[] { 100, -20, 99 };
        }
    }

    
    [TestMethod]
    [DynamicData(nameof(Numbers))]
    public async Task ExecuteItoaTest(int p_Number1, int p_Number2, int p_Result)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.ifexpr.wasm"));
        
        int l_ResultValue = await l_Module.Call<int, int, int>("ifexpr", p_Number1, p_Number2);
        Assert.AreEqual(p_Result, l_ResultValue);
    }
    
}