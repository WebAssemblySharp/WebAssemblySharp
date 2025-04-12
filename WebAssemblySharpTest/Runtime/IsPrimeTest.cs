using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class IsPrimeTest
{
    public static IEnumerable<object[]> IsPrimeData
    {
        get
        {
            for (int i = 0; i < 10000; i++)
            {
                yield return new object[] { i, IsPrime(i) };
            }
        }
    }

    //[TestMethod]
    public async Task ExecuteIsprimeWasmTest_1_000_000()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModule l_Module =
            await (await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.isprime.wasm"))).Build();

        for (int i = 0; i < 1_000_000; i++)
        {
            int l_Result = (int)await l_Module.Call("is_prime", 99);
        }
    }

    [TestMethod]
    [DynamicData(nameof(IsPrimeData))]
    public async Task ExecuteIsprimeWasmTest(int p_Number, bool p_IsPrime)
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModule l_Module =
            await (await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.isprime.wasm"))).Build();

        int l_Result = (int)await l_Module.Call("is_prime", p_Number);

        if (p_IsPrime)
            Assert.AreEqual(1, l_Result);
        else
            Assert.AreEqual(0, l_Result);
    }


    private static bool IsPrime(int p_Number)
    {
        if (p_Number <= 1) return false;
        if (p_Number == 2) return true;
        if (p_Number % 2 == 0) return false;


        var l_Boundary = (int)Math.Floor(Math.Sqrt(p_Number));

        for (int i = 3; i <= l_Boundary; i += 2)
            if (p_Number % i == 0)
                return false;

        return true;
    }
}