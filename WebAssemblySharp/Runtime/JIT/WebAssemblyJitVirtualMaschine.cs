using System;
using System.Collections.Generic;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitVirtualMaschine: IWebAssemblyJitVirtualMaschine
{
    public WebAssemblyJitValue[] ExecuteFrame(WasmFuncType p_FuncType, WasmCode p_Code, WebAssemblyJitStackFrame p_Frame)
    {
        Stack<WebAssemblyJitValue> l_Stack = CreateStack();

        ExecuteInstructions(p_Code.Instructions, l_Stack, p_Frame);

        if (p_FuncType.Results.Length != l_Stack.Count)
        {
            throw new WebAssemblyJitException($"Invalid Stack Size after execute instructions. Expected {p_FuncType.Results.Length} but got {l_Stack.Count}");
        }
        
        if (p_FuncType.Results.Length == 0)
        {
            return Array.Empty<WebAssemblyJitValue>();
        }
        
        return l_Stack.ToArray();
    }

    private Stack<WebAssemblyJitValue> CreateStack()
    {
        return new Stack<WebAssemblyJitValue>(16);
    }

    private void ExecuteInstructions(WasmInstruction[] p_CodeInstructions, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {

        for (int i = 0; i < p_CodeInstructions.Length; i++)
        {
            WasmInstruction l_Instruction = p_CodeInstructions[i];
            ExecuteInstruction(l_Instruction, p_Stack, p_Frame);
        }
        
    }

    private void ExecuteInstruction(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        switch (p_Instruction.Opcode)
        {
            case WasmOpcode.LocalGet:
                HandleLocalGet((WasmLocalGet)p_Instruction, p_Stack, p_Frame);
                break;
            case WasmOpcode.I32Add:
                HandleI32Add((WasmI32Add)p_Instruction, p_Stack, p_Frame);
                break;
            case WasmOpcode.I32Const:
                HandleI32Const((WasmI32Const)p_Instruction, p_Stack, p_Frame);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(p_Instruction.Opcode), p_Instruction.Opcode, null);
        }
        
    }

    private void HandleI32Const(WasmI32Const p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        p_Stack.Push(new WebAssemblyJitValue(WasmDataType.I32, p_Instruction.Const));
    }


    private void HandleI32Add(WasmI32Add p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value1.Value + (int)l_Value2.Value);
        p_Stack.Push(l_Result);
    }

    private void HandleLocalGet(WasmLocalGet p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WebAssemblyJitValue l_Value = p_Frame.GetLocal(p_Instruction.LocalIndex);
        p_Stack.Push(l_Value);
    }
}