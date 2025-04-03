using System;
using System.Collections;
using System.Collections.Generic;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitExecutionContext: IWebAssemblyJitInteropStack
{
    private Stack<WebAssemblyJitValue> m_ValueStack;
    private Stack<WebAssemblyJitExecutionCallFrame> m_CallFrameStack;
    
    public WasmFuncType FuncType { get; }

    public WebAssemblyJitStackLocals Locals { get; }
    
    public WebAssemblyJitExecutionContext(WasmFuncType p_FuncType, IEnumerator<WasmInstruction> p_Instuctions, WebAssemblyJitStackLocals p_Locals)
    {
        FuncType = p_FuncType;
        Locals = p_Locals;
        m_ValueStack = new Stack<WebAssemblyJitValue>(8);
        m_CallFrameStack = new Stack<WebAssemblyJitExecutionCallFrame>(8);
        WebAssemblyJitExecutionCallFrame l_CallFrame = new WebAssemblyJitExecutionCallFrame();
        l_CallFrame.Instructions = p_Instuctions;
        l_CallFrame.CurrentBlockIndex = 0;
        l_CallFrame.BlockKind = WebAssemblyJitExecutionCallFrameBlockKind.Regular;
        m_CallFrameStack.Push(l_CallFrame);
    }

    public WasmInstruction GetNextInstruction()
    {
        while (m_CallFrameStack.Count > 0)
        {
            WebAssemblyJitExecutionCallFrame l_Frame = m_CallFrameStack.Peek();
            bool l_MoveNext = l_Frame.Instructions.MoveNext();

            if (!l_MoveNext)
            {
                FinishCallFrame();
                continue;
            }

            return l_Frame.Instructions.Current;
        }

        return null;
    }

    public void MoveCallFrameToBranchIndex(uint p_BranchIndex)
    {
        
        while (m_CallFrameStack.Count > 0)
        {
            WebAssemblyJitExecutionCallFrameBlockKind l_BlockKind = CurrentBlockKind;
            
            switch (l_BlockKind)
            {
                case WebAssemblyJitExecutionCallFrameBlockKind.Regular:

                    if (p_BranchIndex == 0)
                    {
                        FinishCallFrame();
                        return;
                    }

                    throw new WebAssemblyJitException($"Invalid Branch Index {p_BranchIndex} in Regular Block");

                case WebAssemblyJitExecutionCallFrameBlockKind.Restart:

                    // Mainly the Look Case
                    if (p_BranchIndex == 0)
                    {
                        RestartCallFrame();
                        return;
                    }

                    if (p_BranchIndex == CurrentBlockIndex)
                    {
                        RestartCallFrame();
                        return;
                    }

                    FinishCallFrame();
                    
                    break;
                case WebAssemblyJitExecutionCallFrameBlockKind.Exit:

                    // Mainly the Block Case

                    if (p_BranchIndex == 0)
                    {
                        FinishCallFrame();
                        return;
                    }

                    if (p_BranchIndex == CurrentBlockIndex)
                    {
                        FinishCallFrame();
                        return;
                    }
                    
                    FinishCallFrame();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void RestartCallFrame()
    {
        m_CallFrameStack.Peek().Instructions.Reset();
    }

    public void FinishCallFrame()
    {
        WebAssemblyJitExecutionCallFrame l_CallFrame = m_CallFrameStack.Pop();
        l_CallFrame.Instructions.Dispose();
        
    }

    public bool HasPendingExecutions()
    {
        if (m_CallFrameStack.Count == 0)
            return false;

        return m_CallFrameStack.Peek().Instructions.MoveNext();
    }

    public int CurrentBlockIndex
    {
        get { return m_CallFrameStack.Peek().CurrentBlockIndex; }
    }

    public WebAssemblyJitExecutionCallFrameBlockKind CurrentBlockKind
    {
        get { return m_CallFrameStack.Peek().BlockKind; }
    }

    public void UpdateCallFrame(IEnumerator<WasmInstruction> p_Executions, int p_CurrentBlockIndex, WebAssemblyJitExecutionCallFrameBlockKind p_Kind)
    {
         WebAssemblyJitExecutionCallFrame l_NextFrame = new WebAssemblyJitExecutionCallFrame();
        l_NextFrame.Instructions = p_Executions;
        l_NextFrame.CurrentBlockIndex = p_CurrentBlockIndex;
        l_NextFrame.BlockKind = p_Kind;
        m_CallFrameStack.Push(l_NextFrame);
    }

    public void ExitContext()
    {
        while (m_CallFrameStack.Count > 0)
        {
            WebAssemblyJitExecutionCallFrame l_Frame = m_CallFrameStack.Pop();
            l_Frame.Instructions.Dispose();
        }
    }

    public WebAssemblyJitValue PopFromStack()
    {
        return m_ValueStack.Pop();
    }

    public void PushToStack(WebAssemblyJitValue p_Result)
    {
        m_ValueStack.Push(p_Result);
    }
    
    public int StackCount
    {
        get { return m_ValueStack.Count; }
    }

    public WebAssemblyJitValue[] StackToArray()
    {
        return m_ValueStack.ToArray();
    }
}

struct WebAssemblyJitExecutionCallFrame
{
    public IEnumerator<WasmInstruction> Instructions;
    public int CurrentBlockIndex;
    public WebAssemblyJitExecutionCallFrameBlockKind BlockKind;
}

public enum WebAssemblyJitExecutionCallFrameBlockKind
{
    Regular,
    Restart,
    Exit
}