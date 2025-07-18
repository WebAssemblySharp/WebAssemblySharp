﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITAssembly
{
    public IDictionary ExportedMethodes { get; private set; }
    public IWebAssemblyMemoryArea[] MemoryAreas { get; private set; }
    public object Instance { get; private set; }

    public WebAssemblyJITAssembly(IDictionary p_ExportedMethodes, object p_Instance, IWebAssemblyMemoryArea[] p_MemoryAreas)
    {
        ExportedMethodes = p_ExportedMethodes;
        Instance = p_Instance;
        MemoryAreas = p_MemoryAreas;
    }
    
}