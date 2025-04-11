using System;

namespace WebAssemblySharp.Runtime.Interpreter;

public class WebAssemblyInterpreterRamMemoryArea: IWebAssemblyInterpreterMemoryArea
{
    private byte[] m_Memory;
    private int m_CurrentPages;
    private int m_MaxPages;

    public WebAssemblyInterpreterRamMemoryArea(int p_CurrentPages, int p_MaxPages)
    {
        m_CurrentPages = p_CurrentPages;
        m_MaxPages = p_MaxPages;
        m_Memory = new byte[m_CurrentPages * WebAssemblyConst.WASM_MEMORY_PAGE_SIZE];
    }

    public Span<byte> GetMemoryAccess(int p_Address, int p_Length)
    {
        return m_Memory.AsSpan(p_Address, p_Length);
    }
}