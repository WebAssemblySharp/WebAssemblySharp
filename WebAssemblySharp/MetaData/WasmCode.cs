﻿using System.Collections.Generic;
using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.MetaData;

public class WasmCode
{
    public long CodeSize { get; set; }
    public WasmDataType[] Locals { get; set; }
    public IEnumerable<WasmInstruction> Instructions { get; set; }
}