using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.Runtime;

namespace WebAssemblySharpTest.Runtime.OpCodes;

[TestClass]
public class WasmI32ConstTest
{
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task OpCodeTest(Type p_RuntimeType)
    {
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
                    new WasmI32Const() {Const = 12345}
                ]
            }
        ];

        WebAssemblyRuntime l_Runtime = await WebAssemblyRuntimeBuilder.Create(p_RuntimeType).LoadModule("main", null, l_Data).Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        int l_Result = await l_Module.Call<int>("test");
        Assert.AreEqual(12345, l_Result);
    }
}
