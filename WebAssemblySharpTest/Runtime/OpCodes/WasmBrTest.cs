using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.Runtime;

namespace WebAssemblySharpTest.Runtime.OpCodes;

[TestClass]
public class WasmBrTest
{
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task OpCodeTest(Type p_RuntimeType)
    {
        // Funktion mit Block und Br, die vorzeitig aus dem Block springt und 42 zurückgibt
        WasmMetaData l_Data = new WasmMetaData();
        l_Data.Export =
        [
            new WasmExport()
            {
                Index = 0,
                Kind = WasmExternalKind.Function,
                Name = new WasmString("test")
            }
        ];
        l_Data.FuncIndex = [0];
        l_Data.FunctionType =
        [
            new WasmFuncType()
            {
                Parameters = [],
                Results = [WasmDataType.I32]
            }
        ];
        l_Data.Code =
        [
            new WasmCode()
            {
                CodeSize = -1,
                Locals =[],
                Instructions =
                [
                    new WasmBlock()
                    {
                        BlockType = WasmBlockType.I32,
                        Body = new WasmInstruction[]
                        {
                            new WasmI32Const() { Const = 42 },
                            new WasmBr() { LabelIndex = 0 },
                            new WasmI32Const() { Const = 99 } // wird nie ausgeführt
                        }
                    }
                ]
            }
        ];

        WebAssemblyRuntime l_Runtime = await WebAssemblyRuntimeBuilder.Create(p_RuntimeType).LoadModule("main", null, l_Data).Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        int l_Result = await l_Module.Call<int>("test");
        Assert.AreEqual(42, l_Result);
    }
}
