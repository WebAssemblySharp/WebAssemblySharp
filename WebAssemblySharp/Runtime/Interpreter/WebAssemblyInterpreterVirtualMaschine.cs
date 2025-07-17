using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.Runtime.Memory;
using WebAssemblySharp.Runtime.Utils;

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

    public delegate ValueTask ExecuteInstructionDelegateAsync(WasmInstruction Instruction, WebAssemblyInterpreterExecutionContext Context);

    private WebAssemblyInterpreterValue[] m_Globals;
    private IWebAssemblyMemoryArea[] m_MemoryAreas;
    private Dictionary<WasmOpcode, ExecuteInstructionDelegate> m_InstructionHandlers;

    private void HandleI32Const(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmI32Const l_Instruction = (WasmI32Const)p_Instruction;
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Instruction.Const));
    }


    private void HandleI32Add(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_IntResult = l_Value1.IntValue + l_Value2.IntValue;
        
#if true
        Debug.WriteLine($"{l_Value2.IntValue} + {l_Value1.IntValue} = {l_IntResult}");
#endif
        
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(l_IntResult);
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
        m_InstructionHandlers.Add(WasmOpcode.I32GeS, HandleI32GeS);
        m_InstructionHandlers.Add(WasmOpcode.I32Mul, HandleI32Mul);
        m_InstructionHandlers.Add(WasmOpcode.I32Load, HandleI32Load);
        m_InstructionHandlers.Add(WasmOpcode.MemoryGrow, HandleMemoryGrow);
        m_InstructionHandlers.Add(WasmOpcode.MemorySize, HandleMemorySize);
        m_InstructionHandlers.Add(WasmOpcode.MemoryFill, HandleMemoryFill);
        m_InstructionHandlers.Add(WasmOpcode.I32Ne, HandleI32Ne);
        m_InstructionHandlers.Add(WasmOpcode.I32GtS, HandleI32GtS);
    }

    private void HandleI32GtS(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Result = l_Value2.IntValue > l_Value1.IntValue ? 1 : 0;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} > ${l_Value1.IntValue} = ${l_Result}");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Result));
    }

    private void HandleI32Ne(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Result = l_Value2.IntValue != l_Value1.IntValue ? 1 : 0;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} != ${l_Value1.IntValue} = ${l_Result}");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Result));
    }

    private void HandleMemoryFill(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmMemoryFill l_Instruction = (WasmMemoryFill)p_Instruction;
        
        WebAssemblyInterpreterValue l_Length = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Offset = p_Context.PopFromStack();
        
        Span<byte> l_Memory = m_MemoryAreas[l_Instruction.MemoryIndex].GetMemoryAccess(l_Offset.IntValue, l_Length.IntValue);
        byte l_ByteVal = (byte)(l_Value.IntValue & 0xFF);

#if DEBUG
        Debug.WriteLine($"MemoryFill: {l_Length.IntValue} bytes at {l_Offset.IntValue} with {l_ByteVal}");
#endif
        
        l_Memory.Fill(l_ByteVal);
    }

    private void HandleMemorySize(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmMemorySize l_Instruction = (WasmMemorySize)p_Instruction;
        
        IWebAssemblyMemoryArea l_Area = m_MemoryAreas[l_Instruction.MemoryIndex];
        int l_Pages = l_Area.GetCurrentPages();

#if DEBUG
        Debug.WriteLine($"GetMemorySize: {l_Pages} pages");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Pages));
    }

    private void HandleMemoryGrow(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmMemoryGrow l_Instruction = (WasmMemoryGrow)p_Instruction;
        
        WebAssemblyInterpreterValue l_PagesToAddValue = p_Context.PopFromStack();
        int l_PagesToAdd = l_PagesToAddValue.IntValue;
        
        if (l_PagesToAdd < 0)
        {
            throw new InvalidOperationException("Memory grow must be non-negative");
        }
        
        IWebAssemblyMemoryArea l_InterpreterMemoryArea = m_MemoryAreas[l_Instruction.MemoryIndex];
        int l_CurrentPages = l_InterpreterMemoryArea.GetCurrentPages();
        int l_NewSize = l_InterpreterMemoryArea.GrowMemory(l_PagesToAdd);

#if DEBUG
        Debug.WriteLine($"MemoryGrow: {l_CurrentPages} pages + {l_PagesToAdd} pages = {l_NewSize}");
#endif
        
        if (l_NewSize < 0)
        {
            // Memory grow failed
            p_Context.PushToStack(new WebAssemblyInterpreterValue(l_NewSize));
        }
        else
        {
            // Memory grow succeeded
            p_Context.PushToStack(new WebAssemblyInterpreterValue(l_CurrentPages));
        }
        
    }

    private void HandleI32Store8(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmI32Store8 l_Instruction = (WasmI32Store8)p_Instruction;
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Address = p_Context.PopFromStack();
        Span<byte> l_Memory = m_MemoryAreas[0].GetMemoryAccess(l_Instruction.Offset + l_Address.IntValue, 1);
        byte l_ByteVal = (byte)(l_Value.IntValue & 0xFF);

#if DEBUG
        Debug.WriteLine($"I32Store8: {l_ByteVal} at {l_Instruction.Offset + l_Address.IntValue}");
#endif
        
        l_Memory[0] = l_ByteVal;
    }

    private void HandleI64Store32(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmI64Store32 l_Instruction = (WasmI64Store32)p_Instruction;
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Address = p_Context.PopFromStack();

        Span<byte> l_Memory = m_MemoryAreas[0].GetMemoryAccess(l_Instruction.Offset + l_Address.IntValue, 4);
        
        uint l_Lower32 = (uint)(l_Value.LongValue & 0xFFFFFFFF);
        
#if DEBUG
        Debug.WriteLine($"I64Store32: {l_Lower32} at {l_Instruction.Offset + l_Address.IntValue}");
#endif
        
        l_Memory[0] = (byte)(l_Lower32 & 0xFF);
        l_Memory[1] = (byte)((l_Lower32 >> 8) & 0xFF);
        l_Memory[2] = (byte)((l_Lower32 >> 16) & 0xFF);
        l_Memory[3] = (byte)((l_Lower32 >> 24) & 0xFF);
    }
    
    private void HandleI32Extend8_s(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        int l_Value8 = l_Value.IntValue;
        int l_Value32 = (sbyte)(l_Value8 & 0xFF);

#if DEBUG
        Debug.WriteLine($"Extend8_s: {l_Value8} to {l_Value32}");
#endif
        
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(l_Value32);
        p_Context.PushToStack(l_Result);
    }

    private void HandleUnreachable(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        throw new InvalidOperationException("Unreachable code reached");
    }

    private void HandleI32Load(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmI32Load l_Instruction = (WasmI32Load)p_Instruction;
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        long l_Address = l_Instruction.Offset + l_Value.IntValue;
        Span<byte> l_Span = m_MemoryAreas[0].GetMemoryAccess(l_Address, 4);
        
        uint l_Value32 = (uint)(l_Span[0] | (l_Span[1] << 8) | (l_Span[2] << 16) | (l_Span[3] << 24));

#if DEBUG
        Debug.WriteLine($"I32Load: {l_Value32} at {l_Instruction.Offset + l_Value.IntValue}");
#endif
        
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue((int)l_Value32);
        p_Context.PushToStack(l_Result);
    }
    
    private void HandleI32Load8U(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmI32Load8U l_Instruction = (WasmI32Load8U)p_Instruction;
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
        long l_Address = l_Instruction.Offset + l_Value.IntValue;
        Span<byte> l_Span = m_MemoryAreas[0].GetMemoryAccess(l_Address, 1);
        uint l_Value8 = l_Span[0];

#if DEBUG
        Debug.WriteLine($"I32Load8U: {l_Value8} at {l_Instruction.Offset + l_Value.IntValue}");
#endif
        
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue((int)l_Value8);
        p_Context.PushToStack(l_Result);
    }

    private void HandleI32Sub(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Value = l_Value2.IntValue - l_Value1.IntValue;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} - {l_Value1.IntValue} = {l_Value}");
#endif
        
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(l_Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleGlobalGet(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmGlobalGet l_Instruction = ((WasmGlobalGet)p_Instruction);
        p_Context.PushToStack(m_Globals[l_Instruction.GlobalIndex]);
    }

    private void HandleI32DivU(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        uint l_Value = (uint)l_Value2.IntValue / (uint)l_Value1.IntValue;
        
#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} / {l_Value1.IntValue} = {(int)l_Value}");
#endif
        
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue((int)l_Value);
        p_Context.PushToStack(l_Result);
    }
    
    private void HandleI32Mul(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
        
        int l_Value = l_Value2.IntValue * l_Value1.IntValue;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} * {l_Value1.IntValue} = {l_Value}");
#endif
        
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue(l_Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleBr(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmBr l_Instruction = (WasmBr)p_Instruction;
        p_Context.MoveCallFrameToBranchIndex(l_Instruction.LabelIndex);
        
#if DEBUG
        Debug.WriteLine($"Br: {l_Instruction.LabelIndex}");
#endif
        
    }

    private void HandleBrIf(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmBrIf l_Instruction = (WasmBrIf)p_Instruction;
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();

#if DEBUG
        Debug.WriteLine($"BrIf: Go Label {l_Instruction.LabelIndex} if {l_Value.IntValue} != 0 = {l_Value.IntValue != 0}");
#endif
        
        if (l_Value.IntValue != 0)
        {
            p_Context.MoveCallFrameToBranchIndex(l_Instruction.LabelIndex);
        }
    }

    private void HandleI32GeU(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Result = (uint)l_Value2.IntValue >= (uint)l_Value1.IntValue ? 1 : 0;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} >= {l_Value1.IntValue} = {l_Result}");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Result));
    }
    
    private void HandleI32GeS(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Result = l_Value2.IntValue >= l_Value1.IntValue ? 1 : 0;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} >= {l_Value1.IntValue} = {l_Result}");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Result));    
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

        uint l_Value = (uint)l_Value2.IntValue % (uint)l_Value1.IntValue;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} % {l_Value1.IntValue} = {(int)l_Value}");
#endif
        
        WebAssemblyInterpreterValue l_Result = new WebAssemblyInterpreterValue((int)l_Value);
        p_Context.PushToStack(l_Result);
    }

    private void HandleI32Eq(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Result = l_Value2.IntValue == l_Value1.IntValue ? 1 : 0;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} == {l_Value1.IntValue} = {l_Result}");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Result));
    }

    private void HandleI32Eqz(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();

        int l_Result = l_Value.IntValue == 0 ? 1 : 0;

#if DEBUG
        Debug.WriteLine($"{l_Value.IntValue} == 0 = {l_Result}");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Result));
    }

    private void HandleReturn(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {

#if DEBUG
        Debug.WriteLine("Return");
#endif
        
        p_Context.ExitContext();
    }

    private void HandleIf(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WasmIf l_Instruction = (WasmIf)p_Instruction;

        WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();

        bool l_CompareResult = (l_Value.IntValue != 0);

#if DEBUG
        Debug.WriteLine($"If: {l_Value.IntValue} != 0 = {l_CompareResult}");
#endif
        
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

        int l_Result = (uint)l_Value2.IntValue < (uint)l_Value1.IntValue ? 1 : 0;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} < {l_Value1.IntValue} = {l_Result}");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Result));
    }

    private void HandleI32LtS(WasmInstruction p_Instruction, WebAssemblyInterpreterExecutionContext p_Context)
    {
        WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
        WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();

        int l_Result = l_Value2.IntValue < l_Value1.IntValue ? 1 : 0;

#if DEBUG
        Debug.WriteLine($"{l_Value2.IntValue} < {l_Value1.IntValue} = {l_Result}");
#endif
        
        p_Context.PushToStack(new WebAssemblyInterpreterValue(l_Result));
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
            switch (p_FuncType.Parameters[i])
            {
                case WasmDataType.Unkown:
                    throw new InvalidOperationException($"Invalid Function Param Type {p_FuncType.Parameters[i]}");
                case WasmDataType.I32:
                    l_Values[i] = new WebAssemblyInterpreterValue((int)p_Args[i], true);
                    break;
                case WasmDataType.I64:
                    l_Values[i] = new WebAssemblyInterpreterValue((long)p_Args[i], true);
                    break;
                case WasmDataType.F32:
                    l_Values[i] = new WebAssemblyInterpreterValue((float)p_Args[i], true);
                    break;
                case WasmDataType.F64:
                    l_Values[i] = new WebAssemblyInterpreterValue((double)p_Args[i], true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        for (int i = 0; i < p_Code.Locals.Length; i++)
        {
            WasmDataType l_CodeLocal = p_Code.Locals[i];

            switch (l_CodeLocal)
            {
                case WasmDataType.Unkown:
                    throw new InvalidOperationException($"Invalid Local Type {l_CodeLocal}");
                case WasmDataType.I32:
                    l_Values[p_Args.Length + i] = new WebAssemblyInterpreterValue(0, true);
                    break;
                case WasmDataType.I64:
                    l_Values[p_Args.Length + i] = new WebAssemblyInterpreterValue(0L, true);
                    break;
                case WasmDataType.F32:
                    l_Values[p_Args.Length + i] = new WebAssemblyInterpreterValue(0.0f, true);
                    break;
                case WasmDataType.F64:
                    l_Values[p_Args.Length + i] = new WebAssemblyInterpreterValue(0.0d, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
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
    public async ValueTask<bool> ExecuteFrame(WebAssemblyInterpreterExecutionContext p_Context, int p_InstructionsToExecute)
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
                await (((ExecuteInstructionDelegateAsync)l_Instruction.VmData)(l_Instruction, p_Context)).ConfigureAwait(false);
            }

            l_InstcutionCounter++;

            if (l_InstcutionCounter >= p_InstructionsToExecute)
            {
                // Pause the execution
                return false;
            }
        }
    }

    private async ValueTask<bool> ExecuteFrameWithoutInstructionsLimit(WebAssemblyInterpreterExecutionContext p_Context)
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
                await (((ExecuteInstructionDelegateAsync)l_Instruction.VmData)(l_Instruction, p_Context)).ConfigureAwait(false);
            }

#if DEBUG
            WebAssemblyInterpreterValue[] l_StackToArray = p_Context.StackToArray();
            if (l_StackToArray.Length != 0)
                Debug.WriteLine($"Stack: {string.Join(";", l_StackToArray)}");
            else
                Debug.WriteLine($"Stack: (empty)");
#endif
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

    public void SetupMemory(IWebAssemblyMemoryArea[] p_Memory)
    {
        if (p_Memory != null)
        {
            m_MemoryAreas = p_Memory;    
        }
        else
        {
            m_MemoryAreas = Array.Empty<IWebAssemblyMemoryArea>();
        }
    }

    public async Task PreloadData(WasmData[] p_Data)
    {
        if (p_Data == null)
            return;

        WasmFuncType l_FuncType = new WasmFuncType();
        l_FuncType.Parameters = Array.Empty<WasmDataType>();
        l_FuncType.Results = new[] { WasmDataType.I32 };
        
        foreach (WasmData l_WasmData in p_Data)
        {
            byte[] l_InitContent = l_WasmData.Data.Data;
            
            if (l_InitContent == null)
                continue;
            
            if (l_InitContent.Length == 0)
                continue;
            
            WebAssemblyInterpreterExecutionContext l_Context =
                new WebAssemblyInterpreterExecutionContext(l_FuncType, l_WasmData.OffsetInstructions.GetEnumerator(), new WebAssemblyInterpreterStackLocals());

            await ExecuteFrameWithoutInstructionsLimit(l_Context);
            
            if (l_Context.StackCount != 1)
            {
                throw new WebAssemblyInterpreterException(
                    $"Invalid Stack Size after execute instructions. Expected 1 but got {l_Context.StackCount}");
            }
            
            WebAssemblyInterpreterValue l_OffsetValue = l_Context.PopFromStack();
            
            Span<byte> l_Memory = m_MemoryAreas[l_WasmData.MemoryIndex].GetMemoryAccess(l_OffsetValue.IntValue, l_InitContent.Length);
            l_InitContent.CopyTo(l_Memory);
        }
        
    }

    public async Task InitGlobals(WasmGlobal[] p_Globals)
    {
        if (p_Globals == null)
        {
            m_Globals = Array.Empty<WebAssemblyInterpreterValue>();
            return;
        }   
        
        m_Globals = new WebAssemblyInterpreterValue[p_Globals.Length];
        
        for (int i = 0; i < p_Globals.Length; i++)
        {
            WasmGlobal l_Global = p_Globals[i];

            WasmFuncType l_FuncType = new WasmFuncType();
            l_FuncType.Parameters = Array.Empty<WasmDataType>();
            l_FuncType.Results = new[] { l_Global.Type };
            
            WebAssemblyInterpreterExecutionContext l_Context =
                new WebAssemblyInterpreterExecutionContext(l_FuncType, l_Global.InitInstructions.GetEnumerator(), new WebAssemblyInterpreterStackLocals());
            
            await ExecuteFrameWithoutInstructionsLimit(l_Context);
            
            if (l_Context.StackCount != 1)
            {
                throw new WebAssemblyInterpreterException(
                    $"Invalid Stack Size after execute instructions. Expected 1 but got {l_Context.StackCount}");
            }
            
            WebAssemblyInterpreterValue l_InitValue = l_Context.PopFromStack();
            
            if (l_InitValue.DataType != l_Global.Type)
            {
                throw new WebAssemblyInterpreterException(
                    $"Invalid Global Value Type. Expected {l_Global.Type} but got {l_InitValue.DataType}");
            }
            
            WebAssemblyInterpreterValue l_Value = new WebAssemblyInterpreterValue(l_InitValue, l_Global.Mutable);
            m_Globals[i] = l_Value;
        }
        
    }
    
    public IWebAssemblyMemoryArea GetMemoryArea(int p_Index)
    {
        return m_MemoryAreas[p_Index];
    }
    
}