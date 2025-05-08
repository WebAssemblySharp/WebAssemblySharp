using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.Memory;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class LoopsTest
{
    //[TestMethod]
    public async Task ExecuteAddAllTest()
    {
        Random l_Random = new Random();

        WebAssemblyHeapMemoryArea l_HeapMemoryArea = new WebAssemblyHeapMemoryArea(80);
        
        int l_StartOffset = 128;
        int l_Count = 50;

        Span<byte> l_MemoryAccess = l_HeapMemoryArea.GetMemoryAccess(l_StartOffset, l_Count);

        for (int i = 0; i < l_Count; i++)
        {
            l_MemoryAccess[i] = (byte)(i * 2 - 1);
        }
        
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("main", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.loops.wasm"), null);
        l_RuntimeBuilder.ImportMethod("log_i32", new Action<int>((x) => { throw new Exception("log_i32 not implemented"); }));
        l_RuntimeBuilder.ImportMethod("rand_i32", new Func<int>(() => { return l_Random.Next(); }));
        l_RuntimeBuilder.ImportMemoryArea("buffer", l_HeapMemoryArea);
        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
        WebAssemblyModule l_Module = l_WebAssemblyRuntime.GetModule("main");
        
        int l_Sum = await l_Module.Call<int>("add_all", l_StartOffset * 4, l_Count);

        l_MemoryAccess = l_HeapMemoryArea.GetMemoryAccess(l_StartOffset, l_Count);
        int l_WantSum = 0;
        
        for (int i = 0; i < l_Count; i++)
        {
            l_WantSum += l_MemoryAccess[i];
        }
        Assert.AreEqual(l_WantSum, l_Sum);
        
    }
}