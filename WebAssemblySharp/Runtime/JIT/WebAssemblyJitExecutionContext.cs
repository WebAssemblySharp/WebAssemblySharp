using System;
using System.Collections.Generic;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitExecutionContext
{
    private WebAssemblyJitExecutionCallFrame m_CurrentCallFrame;
    private WebAssemblyJitExecutionCallFrame m_UnusedCallFrame;
    
    public WasmFuncType FuncType { get; }

    public WebAssemblyJitStackLocals Locals { get; }
    public Stack<WebAssemblyJitValue> Stack { get; }


    public WebAssemblyJitExecutionContext(WasmFuncType p_FuncType, IEnumerator<WasmInstruction> p_Instuctions, WebAssemblyJitStackLocals p_Locals,
        Stack<WebAssemblyJitValue> p_Stack)
    {
        FuncType = p_FuncType;
        m_CurrentCallFrame = new WebAssemblyJitExecutionCallFrame();
        m_CurrentCallFrame.Instructions = p_Instuctions;
        m_CurrentCallFrame.Parent = null;
        m_CurrentCallFrame.BlockKind = WebAssemblyJitExecutionCallFrameBlockKind.Regular;
        Locals = p_Locals;
        Stack = p_Stack;
    }

    public WasmInstruction GetNextInstruction()
    {
        while (m_CurrentCallFrame != null)
        {
            bool l_MoveNext = m_CurrentCallFrame.Instructions.MoveNext();

            if (!l_MoveNext)
            {
                FinishCallFrame();
                continue;
            }

            return m_CurrentCallFrame.Instructions.Current;
        }

        return null;
    }

    public void MoveCallFrameToBranchIndex(uint p_BranchIndex)
    {
        
        while (m_CurrentCallFrame != null)
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
        m_CurrentCallFrame.Instructions.Reset();
    }

    public void FinishCallFrame()
    {
        m_CurrentCallFrame.Instructions.Dispose();
        WebAssemblyJitExecutionCallFrame l_Parent = m_CurrentCallFrame.Parent;
        
        m_CurrentCallFrame.Parent = m_UnusedCallFrame;
        m_UnusedCallFrame = m_CurrentCallFrame;
        m_UnusedCallFrame.Instructions = null;
        
        m_CurrentCallFrame = l_Parent;
    }

    public bool HasPendingExecutions()
    {
        if (m_CurrentCallFrame == null)
            return false;

        return m_CurrentCallFrame.Instructions.MoveNext();
    }

    public int CurrentBlockIndex
    {
        get { return m_CurrentCallFrame.CurrentBlockIndex; }
    }

    public WebAssemblyJitExecutionCallFrameBlockKind CurrentBlockKind
    {
        get { return m_CurrentCallFrame.BlockKind; }
    }

    public void UpdateCallFrame(IEnumerator<WasmInstruction> p_Executions, int p_CurrentBlockIndex, WebAssemblyJitExecutionCallFrameBlockKind p_Kind)
    {
        WebAssemblyJitExecutionCallFrame l_NextFrame;

        if (m_UnusedCallFrame != null)
        {
            l_NextFrame = m_UnusedCallFrame;
            m_UnusedCallFrame = m_UnusedCallFrame.Parent;
        }
        else
        {
            l_NextFrame = new WebAssemblyJitExecutionCallFrame();
        }
        
        l_NextFrame.Instructions = p_Executions;
        l_NextFrame.Parent = m_CurrentCallFrame;
        l_NextFrame.CurrentBlockIndex = p_CurrentBlockIndex;
        l_NextFrame.BlockKind = p_Kind;
        m_CurrentCallFrame = l_NextFrame;
    }

    public void ExitContext()
    {
        while (m_CurrentCallFrame != null)
        {
            m_CurrentCallFrame.Instructions.Dispose();
            m_CurrentCallFrame = m_CurrentCallFrame.Parent;
        }
    }
}

class WebAssemblyJitExecutionCallFrame
{
    public IEnumerator<WasmInstruction> Instructions;
    public WebAssemblyJitExecutionCallFrame Parent;
    public int CurrentBlockIndex;
    public WebAssemblyJitExecutionCallFrameBlockKind BlockKind;
}

public enum WebAssemblyJitExecutionCallFrameBlockKind
{
    Regular,
    Restart,
    Exit
}