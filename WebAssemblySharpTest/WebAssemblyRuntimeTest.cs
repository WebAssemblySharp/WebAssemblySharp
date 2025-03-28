﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;

namespace WebAssemblySharpTest;

[TestClass]
public class WebAssemblyRuntimeTest
{
    [TestMethod]
    public async Task ExecuteAddWasmTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModule l_Module =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyRuntimeTest).Assembly.GetManifestResourceStream("WebAssemblySharpTest.Data.Example.add.wasm"));

        int l_Result = (int)l_Module.Call("add", 1, 2);
        Assert.AreEqual(3, l_Result);
    }

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

    [TestMethod]
    [DynamicData(nameof(IsPrimeData))]
    public async Task ExecuteIsprimeWasmTest(int p_Number, bool p_IsPrime)
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModule l_Module =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyRuntimeTest).Assembly.GetManifestResourceStream("WebAssemblySharpTest.Data.Example.isprime.wasm"));

        int l_Result = (int)l_Module.Call("is_prime", p_Number);

        if (p_IsPrime)
            Assert.AreEqual(1, l_Result);
        else
            Assert.AreEqual(0, l_Result);
    }


    public static bool IsPrime(int p_Number)
    {
        if (p_Number <= 1) return false;
        if (p_Number == 2) return true;
        if (p_Number % 2 == 0) return false;

        var boundary = (int)Math.Floor(Math.Sqrt(p_Number));

        for (int i = 3; i <= boundary; i += 2)
            if (p_Number % i == 0)
                return false;

        return true;
    }
}