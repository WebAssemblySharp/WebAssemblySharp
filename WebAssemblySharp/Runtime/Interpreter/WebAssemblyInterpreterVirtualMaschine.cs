﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.Interpreter;

/**
 * A WebAssembly interpreter virtual machine that executes WebAssembly instructions.
 * This class provides the core execution environment for interpreting WebAssembly bytecode.
 * It handles instruction execution, memory management, stack operations, and optimization
 * of WebAssembly code. The VM supports both synchronous and asynchronous execution models
 * through delegate handlers for instructions, manages execution contexts, and provides
 * facilities for optimizing WebAssembly code paths. It implements various WebAssembly
 * instructions like arithmetic operations, control flow, memory access, and stack manipulation.
 */
public class WebAssemblyInterpreterVirtualMaschine
{
    public delegate void ExecuteInstructionDelegate(WasmInstruction Instruction, WebAssemblyInterpreterExecutionContext Context);

    public delegate Task ExecuteInstructionDelegateAsync(WasmInstruction Instruction, WebAssemblyInterpreterExecutionContext Context);


    private Dictionary<WasmOpcode, ExecuteInstructionDelegate> m_InstructionHandlers;

    private void HandleI32Const(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmI32Const l_Instruction = (WasmI32Const)p_Instruction;
        p_Context.PushToStack(new WebAssemblyInterpreterValue(WasmDataType.I32, l_Instruction.Const));
    }


    private void HandleI32Add(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, (int)l_Value1.Value + (int)l_Value2.Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleLocalGet(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmLocalGet l_Instruction = (WasmLocalGet)p_Instruction;
        WebAssemblyInterpreterValue l_Value = p_Context.Locals.GetLocal(l_Instruction.LocalIndex);
        p_Context.PushToStack(l_Value);
    }

    public void OptimizeCode(IWebAssemblyInterpreterInteropOptimizer p_ExternalOptimizer, WasmMetaData p_WasmMetaData)
    {
        InitInstructionHandlers();

        if (p_WasmMetaData.Code != null)
        {
            foreach (WasmCode l_Code in p_WasmMetaData.Code)
            {
                OptimizeInstructions(p_ExternalOptimizer, l_Code.Instructions);
            }
        }

        if (p_WasmMetaData.Globals != null)
        {
            foreach (WasmGlobal l_Global in p_WasmMetaData.Globals)
            {
                OptimizeInstructions(p_ExternalOptimizer, l_Global.InitInstructions);
            }
        }

        if (p_WasmMetaData.Data != null)
        {
            foreach (WasmData l_Data in p_WasmMetaData.Data)
            {
                OptimizeInstructions(p_ExternalOptimizer, l_Data.OffsetInstructions);
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
        m_InstructionHandlers.Add(WasmOpcode.I32LtS, HandleI32LtS);
        m_InstructionHandlers.Add(WasmOpcode.If, HandleIf);
        m_InstructionHandlers.Add(WasmOpcode.Return, HandleReturn);
        m_InstructionHandlers.Add(WasmOpcode.I32Eq, HandleI32Eq);
        m_InstructionHandlers.Add(WasmOpcode.I32Eqz, HandleI32Eqz);
        m_InstructionHandlers.Add(WasmOpcode.I32RemU, HandleI32RemU);
        m_InstructionHandlers.Add(WasmOpcode.LocalSet, HandleLocalSet);
        m_InstructionHandlers.Add(WasmOpcode.Loop, HandleLoop);
        m_InstructionHandlers.Add(WasmOpcode.Block, HandleBlock);
        m_InstructionHandlers.Add(WasmOpcode.I32GeU, HandleI32GeU);
        m_InstructionHandlers.Add(WasmOpcode.BrIf, HandleBrIf);
        m_InstructionHandlers.Add(WasmOpcode.Br, HandleBr);
        m_InstructionHandlers.Add(WasmOpcode.I32DivU, HandleI32DivU);
        m_InstructionHandlers.Add(WasmOpcode.GlobalGet, HandleGlobalGet);
        m_InstructionHandlers.Add(WasmOpcode.I32Sub, HandleI32Sub);
        m_InstructionHandlers.Add(WasmOpcode.I32Load8U, HandleI32Load8U);
        m_InstructionHandlers.Add(WasmOpcode.Unreachable, HandleUnreachable);
        m_InstructionHandlers.Add(WasmOpcode.I32Extend8_s, HandleI32Extend8_s);
        m_InstructionHandlers.Add(WasmOpcode.I64Store32, HandleI64Store32);
        m_InstructionHandlers.Add(WasmOpcode.I32Store8, HandleI32Store8);
    }

    private void HandleI32Store8(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Address = p_Context.PopFromStack();

        Span<byte> l_Memory = p_Context.GetMemoryAccess((int)l_Address.Value, 1);
        byte l_ByteVal = (byte)(((int)l_Value.Value) & 0xFF);
        l_Memory[0] = l_ByteVal;
    }

    private void HandleI64Store32(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Address = p_Context.PopFromStack();

        Span<byte> l_Memory = p_Context.GetMemoryAccess((int)l_Address.Value, 4);

        uint l_Lower32 = (uint)((long)l_Value.Value & 0xFFFFFFFF);
        l_Memory[0] = (byte)(l_Lower32 & 0xFF);
        l_Memory[1] = (byte)((l_Lower32 >> 8) & 0xFF);
        l_Memory[2] = (byte)((l_Lower32 >> 16) & 0xFF);
        l_Memory[3] = (byte)((l_Lower32 >> 24) & 0xFF);
    }

    private void HandleI32Extend8_s(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        int l_Value8 = (int)l_Value.Value;
        int l_Value32 = (sbyte)(l_Value8 & 0xFF);
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, l_Value32);
        p_Context.PushToStack(l_Result);
    }

    private void HandleUnreachable(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        throw new InvalidOperationException("Unreachable code reached");
    }

    private void HandleI32Load8U(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        int l_Address = (int)l_Value.Value;
        Span<byte> l_Span = p_Context.GetMemoryAccess(l_Address, 1);
        int l_Value8 = l_Span[0];
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, l_Value8);
        p_Context.PushToStack(l_Result);
    }

    private void HandleI32Sub(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Value = (int)l_Value2.Value - (int)l_Value1.Value;
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, l_Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleGlobalGet(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        throw new NotImplementedException();
    }

    private void HandleI32DivU(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Value = (int)l_Value2.Value / (int)l_Value1.Value;
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, l_Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleBr(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmBr l_Instruction = (WasmBr)p_Instruction;
        p_Context.MoveCallFrameToBranchIndex(l_Instruction.LabelIndex);
    }

    private void HandleBrIf(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmBrIf l_Instruction = (WasmBrIf)p_Instruction;
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();

        if ((int)l_Value.Value != 0)
        {
            p_Context.MoveCallFrameToBranchIndex(l_Instruction.LabelIndex);
        }
    }

    private void HandleI32GeU(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, (int)l_Value2.Value >= (int)l_Value1.Value ? 1 : 0);
        p_Context.PushToStack(l_Result);
    }

    private void HandleBlock(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmBlock l_Instruction = (WasmBlock)p_Instruction;
        p_Context.UpdateCallFrame(l_Instruction.Body.GetEnumerator(), p_Context.CurrentBlockIndex + 1,
            WebAssemblyJitExecutionCallFrameBlockKind.Exit);
    }

    private void HandleLoop(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmLoop l_Instruction = (WasmLoop)p_Instruction;
        p_Context.UpdateCallFrame(l_Instruction.Body.GetEnumerator(), p_Context.CurrentBlockIndex + 1,
            WebAssemblyJitExecutionCallFrameBlockKind.Restart);
    }

    private void HandleLocalSet(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmLocalSet l_Instruction = (WasmLocalSet)p_Instruction;
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        p_Context.Locals.GetLocal(l_Instruction.LocalIndex).CopyValueFrom(l_Value);
    }

    private void HandleI32RemU(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, (int)l_Value2.Value % (int)l_Value1.Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleI32Eq(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, (int)l_Value2.Value == (int)l_Value1.Value ? 1 : 0);
        p_Context.PushToStack(l_Result);
    }

    private void HandleI32Eqz(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, (int)l_Value.Value == 0 ? 1 : 0);
        p_Context.PushToStack(l_Result);
    }

    private void HandleReturn(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        p_Context.ExitContext();
    }

    private void HandleIf(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmIf l_Instruction = (WasmIf)p_Instruction;

        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();

        bool l_CompareResult = ((int)l_Value.Value != 0);

        if (l_CompareResult)
        {
            if (l_Instruction.IfBody != null)
                p_Context.UpdateCallFrame(l_Instruction.IfBody.GetEnumerator(), p_Context.CurrentBlockIndex, p_Context.CurrentBlockKind);
        }
        else
        {
            if (l_Instruction.ElseBody != null)
                p_Context.UpdateCallFrame(l_Instruction.ElseBody.GetEnumerator(), p_Context.CurrentBlockIndex, p_Context.CurrentBlockKind);
        }
    }

    private void HandleI32LtU(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, (int)l_Value2.Value < (int)l_Value1.Value ? 1 : 0);
        p_Context.PushToStack(l_Result);
    }

    private void HandleI32LtS(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(WasmDataType.I32, (int)l_Value2.Value < (int)l_Value1.Value ? 1 : 0);
        p_Context.PushToStack(l_Result);
    }

    private void OptimizeInstructions(IWebAssemblyInterpreterInteropOptimizer p_ExternalOptimizer, IEnumerable<WasmInstruction> p_Instructions)
    {
        foreach (WasmInstruction l_Instruction in p_Instructions)
        {
            if (p_ExternalOptimizer.OptimizeInstruction(l_Instruction))
                continue;

            l_Instruction.VmData = m_InstructionHandlers[l_Instruction.Opcode];

            if (l_Instruction is WasmBlockInstruction)
            {
                WasmBlockInstruction l_BlockInstruction = (WasmBlockInstruction)l_Instruction;
                OptimizeInstructions(p_ExternalOptimizer, l_BlockInstruction.GetAllInstructions());
            }

            if (l_Instruction is WasmIf)
            {
                // Remove empty if and else bodies
                WasmIf l_WasmIf = (WasmIf)l_Instruction;

                if (!l_WasmIf.IfBody.Any())
                {
                    l_WasmIf.IfBody = null;
                }

                if (!l_WasmIf.ElseBody.Any())
                {
                    l_WasmIf.ElseBody = null;
                }
            }
        }
    }


    public WebAssemblyInterpreterExecutionContext CreateContext(WasmFuncType p_FuncType, WasmCode p_Code, object[] p_Args)
    {
        WebAssemblyInterpreterStackLocals l_Locals = CreateStackLocals(p_FuncType, p_Code, p_Args);
        return new WebAssemblyInterpreterExecutionContext(p_FuncType, p_Code.Instructions.GetEnumerator(), l_Locals);
    }

    private WebAssemblyInterpreterStackLocals CreateStackLocals(WasmFuncType p_FuncType, WasmCode p_Code, object[] p_Args)
    {
        int l_Length = p_FuncType.Parameters.Length + p_Code.Locals.Length;

        WebAssemblyInterpreterValue[] l_Values = new WebAssemblyInterpreterValue[l_Length];

        for (int i = 0; i < p_Args.Length; i++)
        {
            l_Values[i] = new WebAssemblyInterpreterValue(p_FuncType.Parameters[i], p_Args[i]);
        }

        for (int i = 0; i < p_Code.Locals.Length; i++)
        {
            WasmCodeLocal l_WasmCodeLocal = p_Code.Locals[i];

            l_Values[(int)l_WasmCodeLocal.Number] = new WebAssemblyInterpreterValue(l_WasmCodeLocal.ValueType, null);
        }

        return new WebAssemblyInterpreterStackLocals(l_Values);
    }

    /**
     * Executes WebAssembly instructions within the specified execution context.
     *
     * This method forms the core execution loop of the WebAssembly interpreter, processing
     * instructions one at a time from the given context. It supports cooperative multitasking
     * by allowing execution to be limited to a specific number of instructions.
     *
     * @param p_Context The execution context containing the instruction stream and stack state.
     * @param p_InstructionsToExecute The maximum number of instructions to execute in this frame.
     *        If zero or negative, executes without an instruction limit until completion.
     *
     * @returns A task that resolves to:
     *          - true: if execution completed normally (reached end of instructions or returned)
     *          - false: if execution was paused due to reaching the instruction limit
     *
     */
    public async Task<bool> ExecuteFrame(WebAssemblyInterpreterExecutionContext p_Context, int p_InstructionsToExecute)
    {
        if (p_InstructionsToExecute <= 0)
        {
            return await ExecuteFrameWithoutInstructionsLimit(p_Context);
        }

        int l_InstcutionCounter = 0;

        while (true)
        {
            WasmInstruction l_Instruction = p_Context.GetNextInstruction();

            if (l_Instruction == null)
                return true;


            if (l_Instruction.VmData is ExecuteInstructionDelegate)
            {
                ((ExecuteInstructionDelegate)l_Instruction.VmData)(l_Instruction, p_Context);
            }
            else
            {
                await (((ExecuteInstructionDelegateAsync)l_Instruction.VmData)(l_Instruction, p_Context));
            }

            l_InstcutionCounter++;

            if (l_InstcutionCounter >= p_InstructionsToExecute)
            {
                // Pause the execution
                return false;
            }
        }
    }

    private async Task<bool> ExecuteFrameWithoutInstructionsLimit(WebAssemblyInterpreterExecutionContext p_Context)
    {
        while (true)
        {
            WasmInstruction l_Instruction = p_Context.GetNextInstruction();

            if (l_Instruction == null)
                return true;

            if (l_Instruction.VmData is ExecuteInstructionDelegate)
            {
                ((ExecuteInstructionDelegate)l_Instruction.VmData)(l_Instruction, p_Context);
            }
            else
            {
                await (((ExecuteInstructionDelegateAsync)l_Instruction.VmData)(l_Instruction, p_Context));
            }
        }
    }

    public Span<WebAssemblyInterpreterValue> FinishContext(WebAssemblyInterpreterExecutionContext p_Context)
    {
        if (p_Context.HasPendingExecutions())
            throw new WebAssemblyInterpreterException($"Context can not be finished. There are still not executed instructions.");


        if (p_Context.FuncType.Results.Length != p_Context.StackCount)
        {
            throw new WebAssemblyInterpreterException(
                $"Invalid Stack Size after execute instructions. Expected {p_Context.FuncType.Results.Length} but got {p_Context.StackCount}");
        }

        if (p_Context.FuncType.Results.Length == 0)
        {
            return new Span<WebAssemblyInterpreterValue>();
        }

        if (p_Context.FuncType.Results.Length == 1)
        {
            WebAssemblyInterpreterValue l_InterpreterValue = p_Context.PopFromStack();
            return new WebAssemblyInterpreterValue[] { l_InterpreterValue };
        }

        return p_Context.StackToArray();
    }

    public void SetupMemory(WasmMemory[] p_Memory)
    {
    }

    public void PreloadData(WasmData[] p_Data)
    {
    }

    public void InitGlobals(WasmGlobal[] p_Globals)
    {
    }
}