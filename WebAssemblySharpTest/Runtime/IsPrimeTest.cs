using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.JIT;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class IsPrimeTest
{
    public static IEnumerable<object[]> IsPrimeData
    {
        get
        {
            foreach (object[] l_Type in TestRuntimeProvider.RuntimeTypes)
            {
                for (int i = 0; i < 10000; i++)
                {
                    yield return new object[] { l_Type[0], i, IsPrime(i) };
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(IsPrimeData))]
    public async Task ExecuteIsprimeWasmTest(Type p_RunTimeType, int p_Number, bool p_IsPrime)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RunTimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.isprime.wasm"));

        int l_Result = (int)await l_Module.DynamicCall("is_prime", p_Number);

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