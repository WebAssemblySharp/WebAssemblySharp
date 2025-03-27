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

        ExecuteInstructions(p_Code.Instructions, l_Stack, p_Frame, 0);

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
        WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        foreach (WasmInstruction l_Instruction in p_CodeInstructions)
        {
            ExecuteInstructionDelegate l_Func = (ExecuteInstructionDelegate)l_Instruction.VmData;

            int l_BreakLabel = l_Func(l_Instruction, p_Stack, p_Frame, p_CurrentBlockIndex);

            if (l_BreakLabel != -1)
            {
                // Exit current bock
                return l_BreakLabel;
            }
        }

        return -1;
    }

    private int HandleI32Const(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WasmI32Const l_Instruction = (WasmI32Const)p_Instruction;
        p_Stack.Push(new WebAssemblyJitValue(WasmDataType.I32, l_Instruction.Const));
        return -1;
    }


    private int HandleI32Add(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value1.Value + (int)l_Value2.Value);
        p_Stack.Push(l_Result);
        return -1;
    }

    private int HandleLocalGet(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WasmLocalGet l_Instruction = (WasmLocalGet)p_Instruction;
        WebAssemblyJitValue l_Value = p_Frame.GetLocal(l_Instruction.LocalIndex);
        p_Stack.Push(l_Value);
        return -1;
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

    private int HandleBr(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WasmBr l_Instruction = (WasmBr)p_Instruction;
        return (int)l_Instruction.LabelIndex;
    }

    private int HandleBrIf(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WasmBrIf l_Instruction = (WasmBrIf)p_Instruction;
        WebAssemblyJitValue l_Value = p_Stack.Pop();

        if ((int)l_Value.Value != 0)
        {
            return (int)l_Instruction.LabelIndex;
        }

        return -1;
    }

    private int HandleI32GeU(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value >= (int)l_Value1.Value ? 1 : 0);
        p_Stack.Push(l_Result);
        return -1;
    }

    private int HandleBlock(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WasmBlock l_Instruction = (WasmBlock)p_Instruction;

        int l_CurrentBlockIndex = p_CurrentBlockIndex + 1;

        int l_BreakLabel = ExecuteInstructions(l_Instruction.Body, p_Stack, p_Frame, l_CurrentBlockIndex);

        if (l_BreakLabel == 0)
        {
            // Break is handled by the block
            return -1;
        }
            
        if (l_BreakLabel == l_CurrentBlockIndex)
        {
            // Break is handled by the block
            return -1;
        }

        return l_BreakLabel;
    }

    private int HandleLoop(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WasmLoop l_Instruction = (WasmLoop)p_Instruction;

        int l_CurrentBlockIndex = p_CurrentBlockIndex + 1;

        while (true)
        {
            
            int l_BreakLabel = ExecuteInstructions(l_Instruction.Body, p_Stack, p_Frame, l_CurrentBlockIndex);
            
            // On an loop the break label index controls if the loop should be repeated or not
            // if the break label is 0 then the loop should be repeated
            // if the break label is the current block index then the loop should be repeated
            
            if (l_BreakLabel == 0)
            {
                // Continue the loop
                continue;
            }
            
            if (l_BreakLabel == l_CurrentBlockIndex)
            {
                // Continue the loop
                continue;
            }

            return l_BreakLabel;
        }
        
    }

    private int HandleLocalSet(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WasmLocalSet l_Instruction = (WasmLocalSet)p_Instruction;
        WebAssemblyJitValue l_Value = p_Stack.Pop();
        p_Frame.GetLocal(l_Instruction.LocalIndex).CopyValueFrom(l_Value);
        return -1;
    }

    private int HandleI32RemU(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value % (int)l_Value1.Value);
        p_Stack.Push(l_Result);
        return -1;
    }

    private int HandleI32Eq(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value == (int)l_Value1.Value ? 1 : 0);
        p_Stack.Push(l_Result);
        return -1;
    }

    private int HandleReturn(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        // Exit the function
        return Int32.MaxValue;
    }

    private int HandleIf(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WasmIf l_Instruction = (WasmIf)p_Instruction;

        WebAssemblyJitValue l_Value = p_Stack.Pop();

        bool l_CompareResult = ((int)l_Value.Value != 0);

        if (l_CompareResult)
            return ExecuteInstructions(l_Instruction.IfBody, p_Stack, p_Frame, p_CurrentBlockIndex);
        else
            return ExecuteInstructions(l_Instruction.ElseBody, p_Stack, p_Frame, p_CurrentBlockIndex);
    }

    private int HandleI32LtU(WasmInstruction p_Instruction, Stack<WebAssemblyJitValue> p_Stack, WebAssemblyJitStackFrame p_Frame, int p_CurrentBlockIndex)
    {
        WebAssemblyJitValue l_Value1 = p_Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value < (int)l_Value1.Value ? 1 : 0);
        p_Stack.Push(l_Result);
        return -1;
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

    private delegate int ExecuteInstructionDelegate(WasmInstruction Instruction, Stack<WebAssemblyJitValue> Stack, WebAssemblyJitStackFrame Frame, int CurrentBlockIndex);
}