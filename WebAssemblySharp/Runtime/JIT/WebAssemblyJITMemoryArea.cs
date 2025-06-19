using System;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITMemoryArea: IWebAssemblyMemoryArea
{
    private readonly Func<byte[]> m_MemoryAccess;
    private readonly Action<byte[]> m_MemoryUpdate;
    private int m_MaxPages;

    public WebAssemblyJITMemoryArea(Func<byte[]> p_MemoryAccess, Action<byte[]> p_MemoryUpdate, int p_MaxPages)
    {
        m_MemoryAccess = p_MemoryAccess;
        m_MemoryUpdate = p_MemoryUpdate;
        m_MaxPages = p_MaxPages;
    }
    
    public Span<byte> GetMemoryAccess(long p_Address, int p_Length)
    {
        return m_MemoryAccess().AsSpan((int)p_Address, p_Length);
    }

    public int GetSize()
    {
        return m_MemoryAccess().Length;
    }

    public int GetCurrentPages()
    {
        return GetSize() / WebAssemblyConst.WASM_MEMORY_PAGE_SIZE;
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

        int l_CurrentPages = GetCurrentPages();

        if (p_Pages == 0)
        {
            return l_CurrentPages;
        }
        
        int l_TargetPages = l_CurrentPages + p_Pages;
        
        if (l_TargetPages > m_MaxPages)
        {
            return -1;
        }
        
        byte[] l_OldMemory = m_MemoryAccess();
        byte[] l_NewMemory = new byte[l_TargetPages * WebAssemblyConst.WASM_MEMORY_PAGE_SIZE];
        Array.Copy(l_OldMemory, l_NewMemory, l_OldMemory.Length);
        m_MemoryUpdate(l_NewMemory);
        return GetCurrentPages();
    }
}