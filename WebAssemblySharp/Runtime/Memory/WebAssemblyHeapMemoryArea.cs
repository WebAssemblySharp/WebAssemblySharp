using System;

namespace WebAssemblySharp.Runtime.Memory;

public class WebAssemblyHeapMemoryArea: IWebAssemblyMemoryArea
{
    private byte[] m_Memory;
    private int m_CurrentPages;
    private int m_MaxPages;

    public WebAssemblyHeapMemoryArea(int p_CurrentPages): this(p_CurrentPages, Int32.MaxValue)
    {
    }
    
    public WebAssemblyHeapMemoryArea(int p_CurrentPages, int p_MaxPages)
    {
        m_CurrentPages = p_CurrentPages;
        m_MaxPages = p_MaxPages;
        m_Memory = new byte[m_CurrentPages * WebAssemblyConst.WASM_MEMORY_PAGE_SIZE];
    }

    public Span<byte> GetMemoryAccess(long p_Address, int p_Length)
    {
        return m_Memory.AsSpan((int)p_Address, p_Length);
    }

    public int GetSize()
    {
        return m_Memory.Length;
    }

    public int GetCurrentPages()
    {
        return m_CurrentPages;
    }

    public int GetMaximumPages()
    {
        return m_MaxPages;
    }

    public int GrowMemory(int p_Pages)
    {
        if (p_Pages < 0)
        {
            return -1;
        }
        
        if (p_Pages == 0)
        {
            return m_CurrentPages;
        }
        
        int l_TargetPages = m_CurrentPages + p_Pages;
        
        if (l_TargetPages > m_MaxPages)
        {
            return -1;
        }
        
        byte[] l_OldMemory = m_Memory;
        m_CurrentPages = l_TargetPages;
        m_Memory = new byte[m_CurrentPages * WebAssemblyConst.WASM_MEMORY_PAGE_SIZE];
        Array.Copy(l_OldMemory, m_Memory, l_OldMemory.Length);
        return m_CurrentPages;
    }
}