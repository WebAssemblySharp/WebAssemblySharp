using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class MemoryBasicsTest
{
    
    [TestMethod]
    public async Task MemoryTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.memory-basics.wasm"));
        
        // Check if the init is fine
        ///////////////////////////////////////
        
        int l_Size = l_Module.GetMemoryArea("memory").GetSize();
        Assert.AreEqual(1 * WebAssemblyConst.WASM_MEMORY_PAGE_SIZE, l_Size);
        
        CheckMemoryContainsInitContent(l_Module);
        
        int l_PageCount = await l_Module.Call<int>("wasm_size");
        Assert.AreEqual(1, l_PageCount);
        
        // Grow the memory
        ///////////////////////////////////////
        
        int l_OldPageSize = await l_Module.Call<int, int>("wasm_grow", 5);
        Assert.AreEqual(1, l_OldPageSize);
        
        l_Size = l_Module.GetMemoryArea("memory").GetSize();
        Assert.AreEqual(6 * WebAssemblyConst.WASM_MEMORY_PAGE_SIZE, l_Size);
        
        CheckMemoryContainsInitContent(l_Module);
        
        l_PageCount = await l_Module.Call<int>("wasm_size");
        Assert.AreEqual(6, l_PageCount);
        
        // Fill the memory
        ///////////////////////////////////////
        
        await l_Module.DynamicCall("wasm_fill", 1024, 127, 16);
        Span<byte> l_MemoryAccess = l_Module.GetMemoryArea("memory").GetMemoryAccess(1024, 16);
        
        for (int i = 0; i < l_MemoryAccess.Length; i++)
        {
            Assert.AreEqual(127, l_MemoryAccess[i]);
        }
    }

    private void CheckMemoryContainsInitContent(WebAssemblyModule p_Module)
    {
        IWebAssemblyMemoryArea l_MemoryArea = p_Module.GetMemoryArea("memory");

        Span<byte> l_MemoryAccess = l_MemoryArea.GetMemoryAccess(0, l_MemoryArea.GetSize());

        for (int i = 0; i < l_MemoryAccess.Length; i++)
        {
            if (i < 16 || (i > 31 && i <40))
                Assert.AreNotEqual(0, l_MemoryAccess[i]);
            else
                Assert.AreEqual(0, l_MemoryAccess[i]);
        }
        
        
    }
}