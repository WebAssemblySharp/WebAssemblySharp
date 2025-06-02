using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.Runtime;

namespace WebAssemblySharpTest.Runtime.OpCodes;

[TestClass]
public class WasmLocalGetTest
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
                Parameters = [WasmDataType.I32],
                Results = [WasmDataType.I32]
            }
        ];
        l_Data.Code =
        [
            new WasmCode()
            {
                CodeSize = -1,
                Locals =[WasmDataType.I32],
                Instructions =
                [
                    new WasmLocalGet() {LocalIndex = 0}
                ]
            }
        ];

        WebAssemblyRuntime l_Runtime = await WebAssemblyRuntimeBuilder.Create(p_RuntimeType).LoadModule("main", null, l_Data).Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        int l_Result = await l_Module.Call<int, int>("test", 42);
        Assert.AreEqual(42, l_Result);
    }

    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task OpCodeTest_I64(Type p_RuntimeType)
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
                Parameters = [WasmDataType.I64],
                Results = [WasmDataType.I64]
            }
        ];
        l_Data.Code =
        [
            new WasmCode()
            {
                CodeSize = -1,
                Locals =[WasmDataType.I64],
                Instructions =
                [
                    new WasmLocalGet() {LocalIndex = 0}
                ]
            }
        ];

        WebAssemblyRuntime l_Runtime = await WebAssemblyRuntimeBuilder.Create(p_RuntimeType).LoadModule("main", null, l_Data).Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        long l_Result = await l_Module.Call<long, long>("test", 1234567890123L);
        Assert.AreEqual(1234567890123L, l_Result);
    }

    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task OpCodeTest_F32(Type p_RuntimeType)
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
                Parameters = [WasmDataType.F32],
                Results = [WasmDataType.F32]
            }
        ];
        l_Data.Code =
        [
            new WasmCode()
            {
                CodeSize = -1,
                Locals =[WasmDataType.F32],
                Instructions =
                [
                    new WasmLocalGet() {LocalIndex = 0}
                ]
            }
        ];

        WebAssemblyRuntime l_Runtime = await WebAssemblyRuntimeBuilder.Create(p_RuntimeType).LoadModule("main", null, l_Data).Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        float l_Result = await l_Module.Call<float, float>("test", 3.14f);
        Assert.AreEqual(3.14f, l_Result);
    }

    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task OpCodeTest_F64(Type p_RuntimeType)
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
                Parameters = [WasmDataType.F64],
                Results = [WasmDataType.F64]
            }
        ];
        l_Data.Code =
        [
            new WasmCode()
            {
                CodeSize = -1,
                Locals =[WasmDataType.F64],
                Instructions =
                [
                    new WasmLocalGet() {LocalIndex = 0}
                ]
            }
        ];

        WebAssemblyRuntime l_Runtime = await WebAssemblyRuntimeBuilder.Create(p_RuntimeType).LoadModule("main", null, l_Data).Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        double l_Result = await l_Module.Call<double, double>("test", 2.718);
        Assert.AreEqual(2.718, l_Result);
    }
}
