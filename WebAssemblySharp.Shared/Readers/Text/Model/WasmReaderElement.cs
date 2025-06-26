using System;

namespace WebAssemblySharp.Readers.Text.Model;

public class WasmReaderElement
{
    public static readonly WasmReaderElement OpenElement = new WasmReaderElement(WasmReaderElementKind.OpenElement, null);
    public static readonly WasmReaderElement CloseElement = new WasmReaderElement(WasmReaderElementKind.CloseElement, null);
    public static readonly WasmReaderElement ModuleElement   = new WasmReaderElement(WasmReaderElementKind.Module, null);
    public static readonly WasmReaderElement FunctionElement = new WasmReaderElement(WasmReaderElementKind.Function, null);
    public static readonly WasmReaderElement ExportElement   = new WasmReaderElement(WasmReaderElementKind.Export, null);
    public static readonly WasmReaderElement ParamElement    = new WasmReaderElement(WasmReaderElementKind.Param, null);
    public static readonly WasmReaderElement ResultElement   = new WasmReaderElement(WasmReaderElementKind.Result, null);
    public static readonly WasmReaderElement I32Element      = new WasmReaderElement(WasmReaderElementKind.I32, null);
    public static readonly WasmReaderElement I64Element      = new WasmReaderElement(WasmReaderElementKind.I64, null);
    public static readonly WasmReaderElement F32Element      = new WasmReaderElement(WasmReaderElementKind.F32, null);
    public static readonly WasmReaderElement F64Element      = new WasmReaderElement(WasmReaderElementKind.F64, null);
    public static readonly WasmReaderElement LocalElement      = new WasmReaderElement(WasmReaderElementKind.Local, null);
    public static readonly WasmReaderElement TableElement = new WasmReaderElement(WasmReaderElementKind.Table, null);
    public static readonly WasmReaderElement MemElement = new WasmReaderElement(WasmReaderElementKind.Mem, null);
    public static readonly WasmReaderElement GlobalElement = new WasmReaderElement(WasmReaderElementKind.Global, null);
    public static readonly WasmReaderElement DataElement = new WasmReaderElement(WasmReaderElementKind.Data, null);
    
    
    public WasmReaderElementKind Kind { get; private set; }
    public String Data { get; private set; }

    public WasmReaderElement(WasmReaderElementKind p_Kind, string p_Data)
    {
        
        Kind = p_Kind;
        Data = p_Data;
    }
    
    public override string ToString()
    {
        if (String.IsNullOrWhiteSpace(Data))
        {
            return Kind.ToString();
        }
        
        return $"{Kind} {Data}";
    }
    
    public String ToText()
    {
        switch (Kind)
        {
            case WasmReaderElementKind.OpenElement:
                return "(";
            case WasmReaderElementKind.CloseElement:
                return ")";
            case WasmReaderElementKind.Module:
                return "module";
            case WasmReaderElementKind.Function:
                return "func";
            case WasmReaderElementKind.Export:
                return "export";
            case WasmReaderElementKind.Param:
                return "param";
            case WasmReaderElementKind.Result:
                return "result";
            case WasmReaderElementKind.I32:
                return "i32";
            case WasmReaderElementKind.I64:
                return "i64";
            case WasmReaderElementKind.F32:
                return "f32";
            case WasmReaderElementKind.F64:
                return "f64";
            case WasmReaderElementKind.Local:
                return "local";
            case WasmReaderElementKind.Table:
                return "table";
            case WasmReaderElementKind.Mem:
                return "mem";
            case WasmReaderElementKind.Global:
                return "global";
            case WasmReaderElementKind.Data:
                return "data";
            case WasmReaderElementKind.Text:
                return "\"" + Data + "\"";
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
}