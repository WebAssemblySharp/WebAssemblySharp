using System;
using System.Collections.Generic;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitVirtualMaschine : IWebAssemblyJitVirtualMaschine
{
    private Dictionary<WasmOpcode, ExecuteInstructionDelegate> m_InstructionHandlers;


    private Stack<WebAssemblyJitValue> CreateStack()
    {
        return new Stack<WebAssemblyJitValue>(16);
    }


    private void HandleI32Const(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WasmI32Const l_Instruction = (WasmI32Const)p_Instruction;
        p_Context.Stack.Push(new WebAssemblyJitValue(WasmDataType.I32, l_Instruction.Const));
    }


    private void HandleI32Add(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value1.Value + (int)l_Value2.Value);
        p_Context.Stack.Push(l_Result);
    }

    private void HandleLocalGet(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WasmLocalGet l_Instruction = (WasmLocalGet)p_Instruction;
        WebAssemblyJitValue l_Value = p_Context.Frame.GetLocal(l_Instruction.LocalIndex);
        p_Context.Stack.Push(l_Value);
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

    private void HandleBr(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WasmBr l_Instruction = (WasmBr)p_Instruction;
        p_Context.MoveCallFrameToBranchIndex(l_Instruction.LabelIndex);
    }

    private void HandleBrIf(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WasmBrIf l_Instruction = (WasmBrIf)p_Instruction;
        WebAssemblyJitValue l_Value = p_Context.Stack.Pop();

        if ((int)l_Value.Value != 0)
        {
            p_Context.MoveCallFrameToBranchIndex(l_Instruction.LabelIndex);
        }

    }
    

    private void HandleI32GeU(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value >= (int)l_Value1.Value ? 1 : 0);
        p_Context.Stack.Push(l_Result);
    }

    private void HandleBlock(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WasmBlock l_Instruction = (WasmBlock)p_Instruction;
        p_Context.UpdateCallFrame(l_Instruction.Body.GetEnumerator(), p_Context.CurrentBlockIndex + 1, WebAssemblyJitExecutionCallFrameBlockKind.Exit);
        
    }

    private void HandleLoop(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WasmLoop l_Instruction = (WasmLoop)p_Instruction;
        p_Context.UpdateCallFrame(l_Instruction.Body.GetEnumerator(), p_Context.CurrentBlockIndex + 1, WebAssemblyJitExecutionCallFrameBlockKind.Restart);
    }

    private void HandleLocalSet(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WasmLocalSet l_Instruction = (WasmLocalSet)p_Instruction;
        WebAssemblyJitValue l_Value = p_Context.Stack.Pop();
        p_Context.Frame.GetLocal(l_Instruction.LocalIndex).CopyValueFrom(l_Value);
    }

    private void HandleI32RemU(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value % (int)l_Value1.Value);
        p_Context.Stack.Push(l_Result);
    }

    private void HandleI32Eq(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value == (int)l_Value1.Value ? 1 : 0);
        p_Context.Stack.Push(l_Result);
    }

    private void HandleReturn(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        p_Context.ExitContext();
    }

    private void HandleIf(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WasmIf l_Instruction = (WasmIf)p_Instruction;

        WebAssemblyJitValue l_Value = p_Context.Stack.Pop();

        bool l_CompareResult = ((int)l_Value.Value != 0);

        if (l_CompareResult)
            p_Context.UpdateCallFrame(l_Instruction.IfBody.GetEnumerator(), p_Context.CurrentBlockIndex, p_Context.CurrentBlockKind);
        else
            p_Context.UpdateCallFrame(l_Instruction.ElseBody.GetEnumerator(), p_Context.CurrentBlockIndex, p_Context.CurrentBlockKind);
    }

    private void HandleI32LtU(WasmInstruction p_Instruction, ref WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Value2 = p_Context.Stack.Pop();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value < (int)l_Value1.Value ? 1 : 0);
        p_Context.Stack.Push(l_Result);
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

    private delegate void ExecuteInstructionDelegate(WasmInstruction Instruction, ref WebAssemblyJitExecutionContext Context);

    public WebAssemblyJitExecutionContext CreateContext(WasmFuncType p_FuncType, WasmCode p_Code, WebAssemblyJitStackFrame p_Frame)
    {
        Stack<WebAssemblyJitValue> l_Stack = CreateStack();
        return new WebAssemblyJitExecutionContext(p_FuncType, p_Code.Instructions.GetEnumerator(), p_Frame, l_Stack);
    }


    public bool ExecuteFrame(ref WebAssemblyJitExecutionContext p_Context, int p_InstructionsToExecute)
    {

        if (p_InstructionsToExecute <= 0)
        {
            return ExecuteFrameWithoutInstcutionLimit(ref p_Context);
        }

        int l_InstcutionCounter = 0; 
        
        while (true)
        {
            WasmInstruction l_Instruction = p_Context.GetNextInstruction();

            if (l_Instruction == null)
                return true;

            ((ExecuteInstructionDelegate)l_Instruction.VmData)(l_Instruction, ref p_Context);

            l_InstcutionCounter++;
            
            if (l_InstcutionCounter >= p_InstructionsToExecute)
            {
                // Pause the execution
                return false;
            }
        }
    }

    private bool ExecuteFrameWithoutInstcutionLimit(ref WebAssemblyJitExecutionContext p_Context)
    {
        while (true)
        {
            WasmInstruction l_Instruction = p_Context.GetNextInstruction();

            if (l_Instruction == null)
                return true;

            ((ExecuteInstructionDelegate)l_Instruction.VmData)(l_Instruction, ref p_Context);
        }

    }

    public Span<WebAssemblyJitValue> FinishContext(ref WebAssemblyJitExecutionContext p_Context)
    {
        if (p_Context.HasPendingExecutions())
            throw new WebAssemblyJitException($"Context can not be finished. There are still not executed instructions.");


        if (p_Context.FuncType.Results.Length != p_Context.Stack.Count)
        {
            throw new WebAssemblyJitException(
                $"Invalid Stack Size after execute instructions. Expected {p_Context.FuncType.Results.Length} but got {p_Context.Stack.Count}");
        }

        if (p_Context.FuncType.Results.Length == 0)
        {
            return new Span<WebAssemblyJitValue>();
        }

        if (p_Context.FuncType.Results.Length == 1)
        {
            return p_Context.Stack.ToArray();
        }

        return p_Context.Stack.ToArray();
    }
}