using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitVirtualMaschine : IWebAssemblyJitVirtualMaschine
{
    private delegate void ExecuteInstructionDelegate(WasmInstruction Instruction, WebAssemblyJitExecutionContext Context);
    
    private Dictionary<WasmOpcode, ExecuteInstructionDelegate> m_InstructionHandlers;

    private void HandleI32Const(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WasmI32Const l_Instruction = (WasmI32Const)p_Instruction;
        p_Context.PushToStack(new WebAssemblyJitValue(WasmDataType.I32, l_Instruction.Const));
    }


    private void HandleI32Add(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value1.Value + (int)l_Value2.Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleLocalGet(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WasmLocalGet l_Instruction = (WasmLocalGet)p_Instruction;
        WebAssemblyJitValue l_Value = p_Context.Locals.GetLocal(l_Instruction.LocalIndex);
        p_Context.PushToStack(l_Value);
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

    private void HandleBr(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WasmBr l_Instruction = (WasmBr)p_Instruction;
        p_Context.MoveCallFrameToBranchIndex(l_Instruction.LabelIndex);
    }

    private void HandleBrIf(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WasmBrIf l_Instruction = (WasmBrIf)p_Instruction;
        WebAssemblyJitValue l_Value = p_Context.PopFromStack();

        if ((int)l_Value.Value != 0)
        {
            p_Context.MoveCallFrameToBranchIndex(l_Instruction.LabelIndex);
        }

    }
    

    private void HandleI32GeU(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value >= (int)l_Value1.Value ? 1 : 0);
        p_Context.PushToStack(l_Result);
    }

    private void HandleBlock(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WasmBlock l_Instruction = (WasmBlock)p_Instruction;
        p_Context.UpdateCallFrame(l_Instruction.Body.GetEnumerator(), p_Context.CurrentBlockIndex + 1, WebAssemblyJitExecutionCallFrameBlockKind.Exit);
        
    }

    private void HandleLoop(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WasmLoop l_Instruction = (WasmLoop)p_Instruction;
        p_Context.UpdateCallFrame(l_Instruction.Body.GetEnumerator(), p_Context.CurrentBlockIndex + 1, WebAssemblyJitExecutionCallFrameBlockKind.Restart);
    }

    private void HandleLocalSet(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WasmLocalSet l_Instruction = (WasmLocalSet)p_Instruction;
        WebAssemblyJitValue l_Value = p_Context.PopFromStack();
        p_Context.Locals.GetLocal(l_Instruction.LocalIndex).CopyValueFrom(l_Value);
    }

    private void HandleI32RemU(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value % (int)l_Value1.Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleI32Eq(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value == (int)l_Value1.Value ? 1 : 0);
        p_Context.PushToStack(l_Result);
    }

    private void HandleReturn(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        p_Context.ExitContext();
    }

    private void HandleIf(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WasmIf l_Instruction = (WasmIf)p_Instruction;

        WebAssemblyJitValue l_Value = p_Context.PopFromStack();

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

    private void HandleI32LtU(WasmInstruction p_Instruction, WebAssemblyJitExecutionContext p_Context)
    {
        WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
        WebAssemblyJitValue l_Result = new WebAssemblyJitValue(WasmDataType.I32, (int)l_Value2.Value < (int)l_Value1.Value ? 1 : 0);
        p_Context.PushToStack(l_Result);
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

    

    public WebAssemblyJitExecutionContext CreateContext(WasmFuncType p_FuncType, WasmCode p_Code, object[] p_Args)
    {
        WebAssemblyJitStackLocals l_Locals = CreateStackLocals(p_FuncType, p_Code, p_Args);
        return new WebAssemblyJitExecutionContext(p_FuncType, p_Code.Instructions.GetEnumerator(), l_Locals);
    }
    
    private WebAssemblyJitStackLocals CreateStackLocals(WasmFuncType p_FuncType, WasmCode p_Code, object[] p_Args)
    {
        int l_Length = p_FuncType.Parameters.Length + p_Code.Locals.Length;
        
        WebAssemblyJitValue[] l_Values = ArrayPool<WebAssemblyJitValue>.Shared.Rent(l_Length);;

        for (int i = 0; i < p_Args.Length; i++)
        {
            l_Values[i] = new WebAssemblyJitValue(p_FuncType.Parameters[i], p_Args[i]);
        }

        for (int i = 0; i < p_Code.Locals.Length; i++)
        {
            WasmCodeLocal l_WasmCodeLocal = p_Code.Locals[i];

            l_Values[(int)l_WasmCodeLocal.Number] = new WebAssemblyJitValue(l_WasmCodeLocal.ValueType, null);
        }

        return new WebAssemblyJitStackLocals(l_Values);
    }


    public Task<bool> ExecuteFrame(WebAssemblyJitExecutionContext p_Context, int p_InstructionsToExecute)
    {

        if (p_InstructionsToExecute <= 0)
        {
            return ExecuteFrameWithoutInstructionsLimit(p_Context);
        }

        int l_InstcutionCounter = 0; 
        
        while (true)
        {
            WasmInstruction l_Instruction = p_Context.GetNextInstruction();

            if (l_Instruction == null)
                return Task.FromResult(true);

            ((ExecuteInstructionDelegate)l_Instruction.VmData)(l_Instruction, p_Context);

            l_InstcutionCounter++;
            
            if (l_InstcutionCounter >= p_InstructionsToExecute)
            {
                // Pause the execution
                return Task.FromResult(false);
            }
        }
    }

    private Task<bool> ExecuteFrameWithoutInstructionsLimit(WebAssemblyJitExecutionContext p_Context)
    {
        while (true)
        {
            WasmInstruction l_Instruction = p_Context.GetNextInstruction();

            if (l_Instruction == null)
                return Task.FromResult(true);

            ((ExecuteInstructionDelegate)l_Instruction.VmData)(l_Instruction, p_Context);
        }

    }

    public Span<WebAssemblyJitValue> FinishContext(WebAssemblyJitExecutionContext p_Context)
    {
        if (p_Context.HasPendingExecutions())
            throw new WebAssemblyJitException($"Context can not be finished. There are still not executed instructions.");


        if (p_Context.FuncType.Results.Length != p_Context.StackCount)
        {
            throw new WebAssemblyJitException(
                $"Invalid Stack Size after execute instructions. Expected {p_Context.FuncType.Results.Length} but got {p_Context.StackCount}");
        }

        ArrayPool<WebAssemblyJitValue>.Shared.Return(p_Context.Locals.GetBuffer());
        
        if (p_Context.FuncType.Results.Length == 0)
        {
            return new Span<WebAssemblyJitValue>();
        }

        if (p_Context.FuncType.Results.Length == 1)
        {
            WebAssemblyJitValue l_JitValue = p_Context.PopFromStack();
            return new WebAssemblyJitValue[] { l_JitValue };
        }

        return p_Context.StackToArray();
    }
}