using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.Memory;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class LoopsTest
{
    
    public static IEnumerable<object[]> First_power_over_limitData
    {
        get
        {
            // Generate test data for the first_power_over_limit function
            yield return new object[] { 2, 1000, 1024 };
            yield return new object[] { 2, 0, 1 };
            yield return new object[] { 3, 25, 27 };
            yield return new object[] { 25, 10000, 15625 };
        }
    }
    

    [TestMethod]
    [DynamicData(nameof(First_power_over_limitData))]
    public async Task ExecuteFirst_power_over_limitTest(int p_Base, int p_Limit, int p_Expected)
    {
        var (l_HeapMemoryArea, l_StartOffset, l_Count, l_Module) = await SetupMemoryAndRuntime();

        int l_Result = await l_Module.Call<int>("first_power_over_limit", p_Base, p_Limit);
        
        Assert.AreEqual(p_Expected, l_Result);
    }
    
    
    [TestMethod]
    public async Task ExecuteRand_multiple_of_10Test()
    {
        var (l_HeapMemoryArea, l_StartOffset, l_Count, l_Module) = await SetupMemoryAndRuntime();

        for (int i = 0; i < 100; i++)
        {
            int l_R10 = await l_Module.Call<int>("rand_multiple_of_10");
            // Make sure rand_multiple_of_10 always returns a multiple of 10.
            Assert.IsTrue(l_R10 % 10 == 0);
        }
        
    }
    
    [TestMethod]
    public async Task ExecuteAddAllTest()
    {
        
        var (l_HeapMemoryArea, l_StartOffset, l_Count, l_Module) = await SetupMemoryAndRuntime();
        
        int l_Sum = await l_Module.Call<int>("add_all", l_StartOffset, l_Count);

        Span<byte> l_MemoryAccess = l_HeapMemoryArea.GetMemoryAccess(l_StartOffset, l_Count * sizeof(int));
        int l_WantSum = 0;
        
        for (int i = 0; i < l_Count; i = i + sizeof(int))
        {
            l_WantSum = l_WantSum + BitConverter.ToInt32(l_MemoryAccess.Slice(i, sizeof(int)));
        }
        Assert.AreEqual(l_WantSum, l_Sum);
        
    }

    private static async Task<(WebAssemblyHeapMemoryArea l_HeapMemoryArea, int l_StartOffset, int l_Count, WebAssemblyModule l_Module)> SetupMemoryAndRuntime()
    {
        Random l_Random = new Random();

        WebAssemblyHeapMemoryArea l_HeapMemoryArea = new WebAssemblyHeapMemoryArea(80);
        
        int l_StartOffset = 128;
        int l_Count = 50;

        Span<byte> l_MemoryAccess = l_HeapMemoryArea.GetMemoryAccess(l_StartOffset, l_Count * sizeof(int));

        for (int i = 0; i < l_Count; i = i + sizeof(int))
        {
            BitConverter.TryWriteBytes(l_MemoryAccess.Slice(i, sizeof(int)), (i * 2 - 1));
        }
        
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("main", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.loops.wasm"), null);
        l_RuntimeBuilder.ImportMethod("log_i32", new Action<int>((x) => { throw new Exception("log_i32 not implemented"); }));
        l_RuntimeBuilder.ImportMethod("rand_i32", new Func<int>(() => { return l_Random.Next(); }));
        l_RuntimeBuilder.ImportMemoryArea("buffer", l_HeapMemoryArea);
        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
        WebAssemblyModule l_Module = l_WebAssemblyRuntime.GetModule("main");
        return (l_HeapMemoryArea, l_StartOffset, l_Count, l_Module);
    }
}