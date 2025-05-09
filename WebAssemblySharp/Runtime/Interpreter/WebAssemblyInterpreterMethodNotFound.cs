﻿using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.Interpreter;

public class WebAssemblyInterpreterMethodNotFound: IWebAssemblyMethod
{
    private readonly String m_Name;

    public WebAssemblyInterpreterMethodNotFound(string p_Name)
    {
        m_Name = p_Name;
    }

    public Task<object> Invoke(params object[] p_Args)
    {
        throw new InvalidOperationException("Method not found: " + m_Name);
    }

    public WasmFuncType GetMetaData()
    {
        throw new InvalidOperationException("Method not found: " + m_Name);
    }
}