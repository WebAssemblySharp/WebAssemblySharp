using System;
using System.Collections.Generic;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitVirtualMaschine : IWebAssemblyJitVirtualMaschine
{
    private Dictionary<WasmOpcode, ExecuteInstructionDelegate> m_InstructionHandlers;

    public WebAssemblyJitValue[] ExecuteFrame(WasmFuncType p_FuncType, WasmCode p_Code, WebAssemblyJitStackFrame p_Frame)
    {
        Stack<WebAssemblyJitValue> l_Stack = CreateStack();

        ExecuteInstructions(p_Code.Instructions, l_Stack, p_Frame);

        if (p_FuncType.Results.Length != l_Stack.Count)
        {
            throw new WebAssemblyJitException(
                $"Invalid Stack Size after execute instructions. Expected {p_FuncType.Results.Length} but got {l_Stack.Count}");
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

    private int ExecuteInstructions(IEnumerable<WasmInstruction> p_CodeInstructions, Stack<WebAssemblyJitValue> p_Stack,
        WebAssemblyJitStackFrame p_Frame)
    {
        foreach (WasmInstruction l_Instruction in p_CodeInstructions)
        {
            ExecuteInstructionDelegate l_Func = (ExecuteInstructionDelegate)l_Instruction.VmData;

            int l_BreakCount = l_Func(l_Instruction, p_Stack, p_Frame);

            if (l_BreakCount > 0)
            {
                // Exit current bock
                return l_BreakCount;
            }
        }

        return 0;
    }

    private int HandleI32Const(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WasmI32Const l_Instruction = (WasmI32Const)p_Instruction;
        p_Stack.Push(new WebAssemblyJitValue(WasmDataType.I32, l_Instruction.Const));
        return 0;
    }


    private int HandleI32Add(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value1.Value + (int)l_Value2.Value);
        p_Stack.Push(l_Result);
        return 0;
    }

    private int HandleLocalGet(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WasmLocalGet l_Instruction = (WasmLocalGet)p_Instruction;
        WebAssemblyJitValue l_Value = p_Frame.GetLocal(l_Instruction.LocalIndex);
        p_Stack.Push(l_Value);
        return 0;
    }

    public void OptimizeCode(WasmMetaData p_WasmMetaData)
    {
        InitInstructionHandlers();

        if (p_WasmMetaData.Code != null)
        {
            foreach (WasmCode l_Code in p_WasmMetaData.Code)
            {
                OptimizeInstructions(l_Code.Instructions);
            }
        }
    }

    private void InitInstructionHandlers()
    {
        m_InstructionHandlers = new Dictionary<WasmOpcode, ExecuteInstructionDelegate>();
        m_InstructionHandlers.Add(WasmOpcode.LocalGet, HandleLocalGet);
        m_InstructionHandlers.Add(WasmOpcode.I32Add, HandleI32Add);
        m_InstructionHandlers.Add(WasmOpcode.I32Const, HandleI32Const);
        m_InstructionHandlers.Add(WasmOpcode.I32LtU, HandleI32LtU);
        m_InstructionHandlers.Add(WasmOpcode.If, HandleIf);
        m_InstructionHandlers.Add(WasmOpcode.Return, HandleReturn);
        m_InstructionHandlers.Add(WasmOpcode.I32Eq, HandleI32Eq);
        m_InstructionHandlers.Add(WasmOpcode.I32RemU, HandleI32RemU);
        m_InstructionHandlers.Add(WasmOpcode.LocalSet, HandleLocalSet);
        m_InstructionHandlers.Add(WasmOpcode.Loop, HandleLoop);
        m_InstructionHandlers.Add(WasmOpcode.Block, HandleBlock);
        m_InstructionHandlers.Add(WasmOpcode.I32GeU, HandleI32GeU);
        m_InstructionHandlers.Add(WasmOpcode.BrIf, HandleBrIf);
        m_InstructionHandlers.Add(WasmOpcode.Br, HandleBr);
    }

    private int HandleBr(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WasmBr l_Instruction = (WasmBr)p_Instruction;
        return (int)l_Instruction.LabelIndex + 1;
    }

    private int HandleBrIf(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WasmBrIf l_Instruction = (WasmBrIf)p_Instruction;
        WebAssemblyJitValue l_Value = p_Stack.Pop();

        if ((int)l_Value.Value != 0)
        {
            return (int)l_Instruction.LabelIndex + 1;
        }

        return 0;
    }

    private int HandleI32GeU(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value >= (int)l_Value1.Value ? 1 : 0);
        p_Stack.Push(l_Result);
        return 0;
    }

    private int HandleBlock(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WasmBlock l_Instruction = (WasmBlock)p_Instruction;

        int l_Instructions = ExecuteInstructions(l_Instruction.Body, p_Stack, p_Frame);

        if (l_Instructions > 0)
        {
            return l_Instructions - 1;
        }

        return l_Instructions;
    }

    private int HandleLoop(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WasmLoop l_Instruction = (WasmLoop)p_Instruction;

        while (true)
        {
            int l_BreakCount = ExecuteInstructions(l_Instruction.Body, p_Stack, p_Frame);

            if (l_BreakCount > 0)
            {
                // Exit the loop
                return l_BreakCount - 1;
            }
        }
    }

    private int HandleLocalSet(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WasmLocalSet l_Instruction = (WasmLocalSet)p_Instruction;
        WebAssemblyJitValue l_Value = p_Stack.Pop();
        p_Frame.GetLocal(l_Instruction.LocalIndex).CopyValueFrom(l_Value);
        return 0;
    }

    private int HandleI32RemU(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value % (int)l_Value1.Value);
        p_Stack.Push(l_Result);
        return 0;
    }

    private int HandleI32Eq(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value == (int)l_Value1.Value ? 1 : 0);
        p_Stack.Push(l_Result);
        return 0;
    }

    private int HandleReturn(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        // Exit the function
        return Int32.MaxValue;
    }

    private int HandleIf(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WasmIf l_Instruction = (WasmIf)p_Instruction;

        WebAssemblyJitValue l_Value = p_Stack.Pop();

        bool l_CompareResult = ((int)l_Value.Value != 0);

        if (l_CompareResult)
            return ExecuteInstructions(l_Instruction.IfBody, p_Stack, p_Frame);
        else
            return ExecuteInstructions(l_Instruction.ElseBody, p_Stack, p_Frame);
    }

    private int HandleI32LtU(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value < (int)l_Value1.Value ? 1 : 0);
        p_Stack.Push(l_Result);
        return 0;
    }

    private void OptimizeInstructions(IEnumerable<WasmInstruction> p_Instructions)
    {
        foreach (WasmInstruction l_Instruction in p_Instructions)
        {
            l_Instruction.VmData = m_InstructionHandlers[l_Instruction.Opcode];

            if (l_Instruction is WasmBlockInstruction)
            {
                WasmBlockInstruction l_BlockInstruction = (WasmBlockInstruction)l_Instruction;
                OptimizeInstructions(l_BlockInstruction.GetAllInstructions());
            }
        }
    }

    private delegate int ExecuteInstructionDelegate(WasmInstruction Instruction, Stack<WebAssemblyJitValue> Stack, WebAssemblyJitStackFrame Frame);
}