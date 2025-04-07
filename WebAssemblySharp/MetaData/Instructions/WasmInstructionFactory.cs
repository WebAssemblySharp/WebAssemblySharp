using System;

namespace WebAssemblySharp.MetaData.Instructions;

public static class WasmInstructionFactory
{
    public static WasmInstruction CreateInstruction(WasmOpcode p_Opcode)
    {
        WasmInstruction l_Result = null;

        switch (p_Opcode)
        {
            case WasmOpcode.Unreachable:
                l_Result = WasmUnreachable.Instance;
                break;
            case WasmOpcode.Nop:
                break;
            case WasmOpcode.Block:
                l_Result = new WasmBlock();
                break;
            case WasmOpcode.Loop:
                l_Result = new WasmLoop();
                break;
            case WasmOpcode.If:
                l_Result = new WasmIf();
                break;
            case WasmOpcode.Else:
                // Should be handled by the If instruction
                break;
            case WasmOpcode.End:
                // Should be handled by the Block, Loop or If instruction
                break;
            case WasmOpcode.Br:
                l_Result = new WasmBr();
                break;
            case WasmOpcode.BrIf:
                l_Result = new WasmBrIf();
                break;
            case WasmOpcode.BrTable:
                break;
            case WasmOpcode.Return:
                l_Result = WasmReturn.Instance;
                break;
            case WasmOpcode.Call:
                l_Result = new WasmCall();
                break;
            case WasmOpcode.CallIndirect:
                break;
            case WasmOpcode.Drop:
                break;
            case WasmOpcode.Select:
                break;
            case WasmOpcode.LocalGet:
                l_Result = new WasmLocalGet();
                break;
            case WasmOpcode.LocalSet:
                l_Result = new WasmLocalSet();
                break;
            case WasmOpcode.LocalTee:
                break;
            case WasmOpcode.GlobalGet:
                l_Result = new WasmGlobalGet();
                break;
            case WasmOpcode.GlobalSet:
                break;
            case WasmOpcode.I32Load:
                break;
            case WasmOpcode.I64Load:
                break;
            case WasmOpcode.F32Load:
                break;
            case WasmOpcode.F64Load:
                break;
            case WasmOpcode.I32Load8S:
                break;
            case WasmOpcode.I32Load8U:
                l_Result = WasmI32Load8U.Instance;
                break;
            case WasmOpcode.I32Load16S:
                break;
            case WasmOpcode.I32Load16U:
                break;
            case WasmOpcode.I64Load8S:
                break;
            case WasmOpcode.I64Load8U:
                l_Result = WasmI64Load8U.Instance;
                break;
            case WasmOpcode.I64Load16S:
                l_Result = WasmI64Load16S.Instance;
                break;
            case WasmOpcode.I64Load16U:
                l_Result = WasmI64Load16U.Instance;
                break;
            case WasmOpcode.I64Load32S:
                l_Result = WasmI64Load32S.Instance;
                break;
            case WasmOpcode.I64Load32U:
                l_Result = WasmI64Load32U.Instance;
                break;
            case WasmOpcode.I32Store:
                l_Result = WasmI32Store.Instance;
                break;
            case WasmOpcode.F32Store:
                break;
            case WasmOpcode.F64Store:
                break;
            case WasmOpcode.I32Store8:
                l_Result = WasmI32Store8.Instance;
                break;
            case WasmOpcode.I32Store16:
                break;
            case WasmOpcode.I64Store8:
                break;
            case WasmOpcode.I64Store16:
                break;
            case WasmOpcode.I64Store32:
                l_Result = WasmI64Store32.Instance;
                break;
            case WasmOpcode.MemorySize:
                break;
            case WasmOpcode.MemoryGrow:
                break;
            case WasmOpcode.I32Const:
                l_Result = new WasmI32Const();
                break;
            case WasmOpcode.I64Const:
                break;
            case WasmOpcode.F32Const:
                break;
            case WasmOpcode.F64Const:
                break;
            case WasmOpcode.I32Eqz:
                l_Result = WasmI32Eqz.Instance;
                break;
            case WasmOpcode.I32Eq:
                l_Result = WasmI32Eq.Instance;
                break;
            case WasmOpcode.I32Ne:
                break;
            case WasmOpcode.I32LtS:
                l_Result = WasmI32LtS.Instance;
                break;
            case WasmOpcode.I32LtU:
                l_Result = WasmI32LtU.Instance;
                break;
            case WasmOpcode.I32GtS:
                break;
            case WasmOpcode.I32GtU:
                break;
            case WasmOpcode.I32LeS:
                break;
            case WasmOpcode.I32LeU:
                break;
            case WasmOpcode.I32GeS:
                break;
            case WasmOpcode.I32GeU:
                l_Result = WasmI32GeU.Instance;
                break;
            case WasmOpcode.I64Eqz:
                break;
            case WasmOpcode.I64Eq:
                break;
            case WasmOpcode.I64Ne:
                break;
            case WasmOpcode.I64LtS:
                break;
            case WasmOpcode.I64LtU:
                break;
            case WasmOpcode.I64GtS:
                break;
            case WasmOpcode.I64GtU:
                break;
            case WasmOpcode.I64LeS:
                break;
            case WasmOpcode.I64LeU:
                break;
            case WasmOpcode.I64GeS:
                break;
            case WasmOpcode.I64GeU:
                break;
            case WasmOpcode.F32Eq:
                break;
            case WasmOpcode.F32Ne:
                break;
            case WasmOpcode.F32Lt:
                break;
            case WasmOpcode.F32Gt:
                break;
            case WasmOpcode.F32Le:
                break;
            case WasmOpcode.F32Ge:
                break;
            case WasmOpcode.F64Eq:
                break;
            case WasmOpcode.F64Ne:
                break;
            case WasmOpcode.F64Lt:
                break;
            case WasmOpcode.F64Gt:
                break;
            case WasmOpcode.F64Le:
                break;
            case WasmOpcode.F64Ge:
                break;
            case WasmOpcode.I32Clz:
                break;
            case WasmOpcode.I32Ctz:
                break;
            case WasmOpcode.I32Popcnt:
                break;
            case WasmOpcode.I32Add:
                l_Result = WasmI32Add.Instance;
                break;
            case WasmOpcode.I32Sub:
                l_Result = WasmI32Sub.Instance;
                break;
            case WasmOpcode.I32Mul:
                break;
            case WasmOpcode.I32DivS:
                break;
            case WasmOpcode.I32DivU:
                l_Result = WasmI32DivU.Instance;
                break;
            case WasmOpcode.I32RemS:
                break;
            case WasmOpcode.I32RemU:
                l_Result = WasmI32RemU.Instance;
                break;
            case WasmOpcode.I32And:
                break;
            case WasmOpcode.I32Or:
                break;
            case WasmOpcode.I32Xor:
                break;
            case WasmOpcode.I32Shl:
                break;
            case WasmOpcode.I32ShrS:
                break;
            case WasmOpcode.I32ShrU:
                break;
            case WasmOpcode.I32Rotl:
                break;
            case WasmOpcode.I32Rotr:
                break;
            case WasmOpcode.I64Clz:
                break;
            case WasmOpcode.I64Ctz:
                break;
            case WasmOpcode.I64Popcnt:
                break;
            case WasmOpcode.I64Add:
                break;
            case WasmOpcode.I64Sub:
                break;
            case WasmOpcode.I64Mul:
                break;
            case WasmOpcode.I64DivS:
                break;
            case WasmOpcode.I64DivU:
                break;
            case WasmOpcode.I64RemS:
                break;
            case WasmOpcode.I64RemU:
                break;
            case WasmOpcode.I64And:
                break;
            case WasmOpcode.I64Or:
                break;
            case WasmOpcode.I64Xor:
                break;
            case WasmOpcode.I64Shl:
                break;
            case WasmOpcode.I64ShrS:
                break;
            case WasmOpcode.I64ShrU:
                break;
            case WasmOpcode.I64Rotl:
                break;
            case WasmOpcode.I64Rotr:
                break;
            case WasmOpcode.F32Abs:
                break;
            case WasmOpcode.F32Neg:
                break;
            case WasmOpcode.F32Ceil:
                break;
            case WasmOpcode.F32Floor:
                break;
            case WasmOpcode.F32Trunc:
                break;
            case WasmOpcode.F32Nearest:
                break;
            case WasmOpcode.F32Sqrt:
                break;
            case WasmOpcode.F32Add:
                break;
            case WasmOpcode.F32Sub:
                break;
            case WasmOpcode.F32Mul:
                break;
            case WasmOpcode.F32Div:
                break;
            case WasmOpcode.F32Min:
                break;
            case WasmOpcode.F32Max:
                break;
            case WasmOpcode.F32Copysign:
                break;
            case WasmOpcode.F64Abs:
                break;
            case WasmOpcode.F64Neg:
                break;
            case WasmOpcode.F64Ceil:
                break;
            case WasmOpcode.F64Floor:
                break;
            case WasmOpcode.F64Trunc:
                break;
            case WasmOpcode.F64Nearest:
                break;
            case WasmOpcode.F64Sqrt:
                break;
            case WasmOpcode.F64Add:
                break;
            case WasmOpcode.F64Sub:
                break;
            case WasmOpcode.F64Mul:
                break;
            case WasmOpcode.F64Div:
                break;
            case WasmOpcode.F64Min:
                break;
            case WasmOpcode.F64Max:
                break;
            case WasmOpcode.F64Copysign:
                break;
            case WasmOpcode.I32WrapI64:
                break;
            case WasmOpcode.I32TruncF32S:
                break;
            case WasmOpcode.I32TruncF32U:
                break;
            case WasmOpcode.I32TruncF64S:
                break;
            case WasmOpcode.I32TruncF64U:
                break;
            case WasmOpcode.I64ExtendI32S:
                break;
            case WasmOpcode.I64ExtendI32U:
                break;
            case WasmOpcode.I64TruncF32S:
                break;
            case WasmOpcode.I64TruncF32U:
                break;
            case WasmOpcode.I64TruncF64S:
                break;
            case WasmOpcode.I64TruncF64U:
                break;
            case WasmOpcode.F32ConvertI32S:
                break;
            case WasmOpcode.F32ConvertI32U:
                break;
            case WasmOpcode.F32ConvertI64S:
                break;
            case WasmOpcode.F32ConvertI64U:
                break;
            case WasmOpcode.F32DemoteF64:
                break;
            case WasmOpcode.F64ConvertI32S:
                break;
            case WasmOpcode.F64ConvertI32U:
                break;
            case WasmOpcode.F64ConvertI64S:
                break;
            case WasmOpcode.F64ConvertI64U:
                break;
            case WasmOpcode.F64PromoteF32:
                break;
            case WasmOpcode.I32ReinterpretF32:
                break;
            case WasmOpcode.I64ReinterpretF64:
                break;
            case WasmOpcode.F32ReinterpretI32:
                break;
            case WasmOpcode.F64ReinterpretI64:
                break;
            case WasmOpcode.I32Extend8_s:
                l_Result = WasmI32Extend8S.Instance;
                break;
            case WasmOpcode.I32Extend16_s:
                break;
            case WasmOpcode.I64Extend8_s:
                break;
            case WasmOpcode.I64Extend16_s:
                break;
            case WasmOpcode.I64Extend32_s:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(p_Opcode), p_Opcode, "Invalid opcode");
        }

        if (l_Result == null)
            throw new Exception("No Instruction found for Opcode " + p_Opcode);

        return l_Result;
    }
}