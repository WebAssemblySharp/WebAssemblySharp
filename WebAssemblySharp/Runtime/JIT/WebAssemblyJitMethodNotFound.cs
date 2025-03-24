﻿using System;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitMethodNotFound: IWebAssemblyMethod
{
    private readonly String m_Name;

    public WebAssemblyJitMethodNotFound(string p_Name)
    {
        m_Name = p_Name;
    }

    public object Invoke(params object[] p_Args)
    {
        throw new InvalidOperationException("Method not found: " + m_Name);
    }
}