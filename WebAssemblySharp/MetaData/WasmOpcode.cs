namespace WebAssemblySharp.MetaData;

public enum WasmOpcode
{
    // Control instructions
    /// <summary>Trap immediately</summary>
    Unreachable = 0x00,
    /// <summary>No operation</summary>
    Nop = 0x01,
    /// <summary>Begin a block construct with a signature</summary>
    Block = 0x02,
    /// <summary>Begin a loop construct with a signature</summary>
    Loop = 0x03,
    /// <summary>Begin an if construct with a signature</summary>
    If = 0x04,
    /// <summary>Begin else branch of if</summary>
    Else = 0x05,
    /// <summary>End a block, loop, if, or function</summary>
    End = 0x0B,
    /// <summary>Branch to a given label</summary>
    Br = 0x0C,
    /// <summary>Conditional branch to a given label</summary>
    BrIf = 0x0D,
    /// <summary>Branch table control flow construct</summary>
    BrTable = 0x0E,
    /// <summary>Return from function</summary>
    Return = 0x0F,
    /// <summary>Call a function by its index</summary>
    Call = 0x10,
    /// <summary>Call a function indirect through a table</summary>
    CallIndirect = 0x11,

    // Parametric instructions
    /// <summary>Drop the top value from the stack</summary>
    Drop = 0x1A,
    /// <summary>Select one of two values based on condition</summary>
    Select = 0x1B,

    // Variable instructions
    /// <summary>Read a local variable</summary>
    LocalGet = 0x20,
    /// <summary>Set a local variable</summary>
    LocalSet = 0x21,
    /// <summary>Set a local variable and keep the value on the stack</summary>
    LocalTee = 0x22,
    /// <summary>Read a global variable</summary>
    GlobalGet = 0x23,
    /// <summary>Set a global variable</summary>
    GlobalSet = 0x24,

    // Memory instructions
    /// <summary>Load 32-bit integer from memory</summary>
    I32Load = 0x28,
    /// <summary>Load 64-bit integer from memory</summary>
    I64Load = 0x29,
    /// <summary>Load 32-bit float from memory</summary>
    F32Load = 0x2A,
    /// <summary>Load 64-bit float from memory</summary>
    F64Load = 0x2B,
    /// <summary>Load 8-bit signed integer and extend to i32</summary>
    I32Load8S = 0x2C,
    /// <summary>Load 8-bit unsigned integer and extend to i32</summary>
    I32Load8U = 0x2D,
    /// <summary>Load 16-bit signed integer and extend to i32</summary>
    I32Load16S = 0x2E,
    /// <summary>Load 16-bit unsigned integer and extend to i32</summary>
    I32Load16U = 0x2F,
    /// <summary>Load 8-bit signed integer and extend to i64</summary>
    I64Load8S = 0x30,
    /// <summary>Load 8-bit unsigned integer and extend to i64</summary>
    I64Load8U = 0x31,
    /// <summary>Load 16-bit signed integer and extend to i64</summary>
    I64Load16S = 0x32,
    /// <summary>Load 16-bit unsigned integer and extend to i64</summary>
    I64Load16U = 0x33,
    /// <summary>Load 32-bit signed integer and extend to i64</summary>
    I64Load32S = 0x34,
    /// <summary>Load 32-bit unsigned integer and extend to i64</summary>
    I64Load32U = 0x35,
    /// <summary>Store 32-bit integer to memory</summary>
    I32Store = 0x36,
    /// <summary>Store 32-bit float to memory</summary>
    F32Store = 0x38,
    /// <summary>Store 64-bit float to memory</summary>
    F64Store = 0x39,
    /// <summary>Store lowest 8 bits of i32 to memory</summary>
    I32Store8 = 0x3A,
    /// <summary>Store lowest 16 bits of i32 to memory</summary>
    I32Store16 = 0x3B,
    /// <summary>Store lowest 8 bits of i64 to memory</summary>
    I64Store8 = 0x3C,
    /// <summary>Store lowest 16 bits of i64 to memory</summary>
    I64Store16 = 0x3D,
    /// <summary>Store lowest 32 bits of i64 to memory</summary>
    I64Store32 = 0x3E,
    /// <summary>Get the current memory size in pages</summary>
    MemorySize = 0x3F,
    /// <summary>Grow memory by a given delta and return previous size</summary>
    MemoryGrow = 0x40,

    // Constants
    /// <summary>Push a 32-bit integer constant</summary>
    I32Const = 0x41,
    /// <summary>Push a 64-bit integer constant</summary>
    I64Const = 0x42,
    /// <summary>Push a 32-bit float constant</summary>
    F32Const = 0x43,
    /// <summary>Push a 64-bit float constant</summary>
    F64Const = 0x44,

    // Comparison operators
    /// <summary>i32 equal to zero (returns i32)</summary>
    I32Eqz = 0x45,
    /// <summary>i32 equal (returns i32)</summary>
    I32Eq = 0x46,
    /// <summary>i32 not equal (returns i32)</summary>
    I32Ne = 0x47,
    /// <summary>i32 signed less than (returns i32)</summary>
    I32LtS = 0x48,
    /// <summary>i32 unsigned less than (returns i32)</summary>
    I32LtU = 0x49,
    /// <summary>i32 signed greater than (returns i32)</summary>
    I32GtS = 0x4A,
    /// <summary>i32 unsigned greater than (returns i32)</summary>
    I32GtU = 0x4B,
    /// <summary>i32 signed less than or equal (returns i32)</summary>
    I32LeS = 0x4C,
    /// <summary>i32 unsigned less than or equal (returns i32)</summary>
    I32LeU = 0x4D,
    /// <summary>i32 signed greater than or equal (returns i32)</summary>
    I32GeS = 0x4E,
    /// <summary>i32 unsigned greater than or equal (returns i32)</summary>
    I32GeU = 0x4F,
    /// <summary>i64 equal to zero (returns i32)</summary>
    I64Eqz = 0x50,
    /// <summary>i64 equal (returns i32)</summary>
    I64Eq = 0x51,
    /// <summary>i64 not equal (returns i32)</summary>
    I64Ne = 0x52,
    /// <summary>i64 signed less than (returns i32)</summary>
    I64LtS = 0x53,
    /// <summary>i64 unsigned less than (returns i32)</summary>
    I64LtU = 0x54,
    /// <summary>i64 signed greater than (returns i32)</summary>
    I64GtS = 0x55,
    /// <summary>i64 unsigned greater than (returns i32)</summary>
    I64GtU = 0x56,
    /// <summary>i64 signed less than or equal (returns i32)</summary>
    I64LeS = 0x57,
    /// <summary>i64 unsigned less than or equal (returns i32)</summary>
    I64LeU = 0x58,
    /// <summary>i64 signed greater than or equal (returns i32)</summary>
    I64GeS = 0x59,
    /// <summary>i64 unsigned greater than or equal (returns i32)</summary>
    I64GeU = 0x5A,
    /// <summary>f32 equal (returns i32)</summary>
    F32Eq = 0x5B,
    /// <summary>f32 not equal (returns i32)</summary>
    F32Ne = 0x5C,
    /// <summary>f32 less than (returns i32)</summary>
    F32Lt = 0x5D,
    /// <summary>f32 greater than (returns i32)</summary>
    F32Gt = 0x5E,
    /// <summary>f32 less than or equal (returns i32)</summary>
    F32Le = 0x5F,
    /// <summary>f32 greater than or equal (returns i32)</summary>
    F32Ge = 0x60,
    /// <summary>f64 equal (returns i32)</summary>
    F64Eq = 0x61,
    /// <summary>f64 not equal (returns i32)</summary>
    F64Ne = 0x62,
    /// <summary>f64 less than (returns i32)</summary>
    F64Lt = 0x63,
    /// <summary>f64 greater than (returns i32)</summary>
    F64Gt = 0x64,
    /// <summary>f64 less than or equal (returns i32)</summary>
    F64Le = 0x65,
    /// <summary>f64 greater than or equal (returns i32)</summary>
    F64Ge = 0x66,

    // Numeric operators
    /// <summary>Count leading zeros in i32</summary>
    I32Clz = 0x67,
    /// <summary>Count trailing zeros in i32</summary>
    I32Ctz = 0x68,
    /// <summary>Count number of 1 bits in i32</summary>
    I32Popcnt = 0x69,
    /// <summary>i32 addition</summary>
    I32Add = 0x6A,
    /// <summary>i32 subtraction</summary>
    I32Sub = 0x6B,
    /// <summary>i32 multiplication</summary>
    I32Mul = 0x6C,
    /// <summary>i32 signed division (can trap on division by zero or overflow)</summary>
    I32DivS = 0x6D,
    /// <summary>i32 unsigned division (can trap on division by zero)</summary>
    I32DivU = 0x6E,
    /// <summary>i32 signed remainder (can trap on division by zero)</summary>
    I32RemS = 0x6F,
    /// <summary>i32 unsigned remainder (can trap on division by zero)</summary>
    I32RemU = 0x70,
    /// <summary>i32 bitwise AND</summary>
    I32And = 0x71,
    /// <summary>i32 bitwise OR</summary>
    I32Or = 0x72,
    /// <summary>i32 bitwise XOR</summary>
    I32Xor = 0x73,
    /// <summary>i32 bitwise shift left</summary>
    I32Shl = 0x74,
    /// <summary>i32 signed shift right</summary>
    I32ShrS = 0x75,
    /// <summary>i32 unsigned shift right</summary>
    I32ShrU = 0x76,
    /// <summary>i32 bitwise rotate left</summary>
    I32Rotl = 0x77,
    /// <summary>i32 bitwise rotate right</summary>
    I32Rotr = 0x78,
    /// <summary>Count leading zeros in i64</summary>
    I64Clz = 0x79,
    /// <summary>Count trailing zeros in i64</summary>
    I64Ctz = 0x7A,
    /// <summary>Count number of 1 bits in i64</summary>
    I64Popcnt = 0x7B,
    /// <summary>i64 addition</summary>
    I64Add = 0x7C,
    /// <summary>i64 subtraction</summary>
    I64Sub = 0x7D,
    /// <summary>i64 multiplication</summary>
    I64Mul = 0x7E,
    /// <summary>i64 signed division (can trap on division by zero or overflow)</summary>
    I64DivS = 0x7F,
    /// <summary>i64 unsigned division (can trap on division by zero)</summary>
    I64DivU = 0x80,
    /// <summary>i64 signed remainder (can trap on division by zero)</summary>
    I64RemS = 0x81,
    /// <summary>i64 unsigned remainder (can trap on division by zero)</summary>
    I64RemU = 0x82,
    /// <summary>i64 bitwise AND</summary>
    I64And = 0x83,
    /// <summary>i64 bitwise OR</summary>
    I64Or = 0x84,
    /// <summary>i64 bitwise XOR</summary>
    I64Xor = 0x85,
    /// <summary>i64 bitwise shift left</summary>
    I64Shl = 0x86,
    /// <summary>i64 signed shift right</summary>
    I64ShrS = 0x87,
    /// <summary>i64 unsigned shift right</summary>
    I64ShrU = 0x88,
    /// <summary>i64 bitwise rotate left</summary>
    I64Rotl = 0x89,
    /// <summary>i64 bitwise rotate right</summary>
    I64Rotr = 0x8A,
    /// <summary>f32 absolute value</summary>
    F32Abs = 0x8B,
    /// <summary>f32 negation</summary>
    F32Neg = 0x8C,
    /// <summary>f32 ceiling operation</summary>
    F32Ceil = 0x8D,
    /// <summary>f32 floor operation</summary>
    F32Floor = 0x8E,
    /// <summary>f32 truncate to integer</summary>
    F32Trunc = 0x8F,
    /// <summary>f32 round to nearest integer</summary>
    F32Nearest = 0x90,
    /// <summary>f32 square root</summary>
    F32Sqrt = 0x91,
    /// <summary>f32 addition</summary>
    F32Add = 0x92,
    /// <summary>f32 subtraction</summary>
    F32Sub = 0x93,
    /// <summary>f32 multiplication</summary>
    F32Mul = 0x94,
    /// <summary>f32 division</summary>
    F32Div = 0x95,
    /// <summary>f32 minimum (returns NaN if either input is NaN)</summary>
    F32Min = 0x96,
    /// <summary>f32 maximum (returns NaN if either input is NaN)</summary>
    F32Max = 0x97,
    /// <summary>f32 copy sign from second operand</summary>
    F32Copysign = 0x98,
    /// <summary>f64 absolute value</summary>
    F64Abs = 0x99,
    /// <summary>f64 negation</summary>
    F64Neg = 0x9A,
    /// <summary>f64 ceiling operation</summary>
    F64Ceil = 0x9B,
    /// <summary>f64 floor operation</summary>
    F64Floor = 0x9C,
    /// <summary>f64 truncate to integer</summary>
    F64Trunc = 0x9D,
    /// <summary>f64 round to nearest integer</summary>
    F64Nearest = 0x9E,
    /// <summary>f64 square root</summary>
    F64Sqrt = 0x9F,
    /// <summary>f64 addition</summary>
    F64Add = 0xA0,
    /// <summary>f64 subtraction</summary>
    F64Sub = 0xA1,
    /// <summary>f64 multiplication</summary>
    F64Mul = 0xA2,
    /// <summary>f64 division</summary>
    F64Div = 0xA3,
    /// <summary>f64 minimum (returns NaN if either input is NaN)</summary>
    F64Min = 0xA4,
    /// <summary>f64 maximum (returns NaN if either input is NaN)</summary>
    F64Max = 0xA5,
    /// <summary>f64 copy sign from second operand</summary>
    F64Copysign = 0xA6,

    // Conversions
    /// <summary>Wrap i64 to i32 (truncate)</summary>
    I32WrapI64 = 0xA7,
    /// <summary>Truncate f32 to i32 (signed)</summary>
    I32TruncF32S = 0xA8,
    /// <summary>Truncate f32 to i32 (unsigned)</summary>
    I32TruncF32U = 0xA9,
    /// <summary>Truncate f64 to i32 (signed)</summary>
    I32TruncF64S = 0xAA,
    /// <summary>Truncate f64 to i32 (unsigned)</summary>
    I32TruncF64U = 0xAB,
    /// <summary>Extend i32 to i64 (signed)</summary>
    I64ExtendI32S = 0xAC,
    /// <summary>Extend i32 to i64 (unsigned)</summary>
    I64ExtendI32U = 0xAD,
    /// <summary>Truncate f32 to i64 (signed)</summary>
    I64TruncF32S = 0xAE,
    /// <summary>Truncate f32 to i64 (unsigned)</summary>
    I64TruncF32U = 0xAF,
    /// <summary>Truncate f64 to i64 (signed)</summary>
    I64TruncF64S = 0xB0,
    /// <summary>Truncate f64 to i64 (unsigned)</summary>
    I64TruncF64U = 0xB1,
    /// <summary>Convert i32 to f32 (signed)</summary>
    F32ConvertI32S = 0xB2,
    /// <summary>Convert i32 to f32 (unsigned)</summary>
    F32ConvertI32U = 0xB3,
    /// <summary>Convert i64 to f32 (signed)</summary>
    F32ConvertI64S = 0xB4,
    /// <summary>Convert i64 to f32 (unsigned)</summary>
    F32ConvertI64U = 0xB5,
    /// <summary>Demote f64 to f32</summary>
    F32DemoteF64 = 0xB6,
    /// <summary>Convert i32 to f64 (signed)</summary>
    F64ConvertI32S = 0xB7,
    /// <summary>Convert i32 to f64 (unsigned)</summary>
    F64ConvertI32U = 0xB8,
    /// <summary>Convert i64 to f64 (signed)</summary>
    F64ConvertI64S = 0xB9,
    /// <summary>Convert i64 to f64 (unsigned)</summary>
    F64ConvertI64U = 0xBA,
    /// <summary>Promote f32 to f64</summary>
    F64PromoteF32 = 0xBB,

    // Reinterpretations
    /// <summary>Reinterpret f32 bits as i32</summary>
    I32ReinterpretF32 = 0xBC,
    /// <summary>Reinterpret f64 bits as i64</summary>
    I64ReinterpretF64 = 0xBD,
    /// <summary>Reinterpret i32 bits as f32</summary>
    F32ReinterpretI32 = 0xBE,
    /// <summary>Reinterpret i64 bits as f64</summary>
    F64ReinterpretI64 = 0xBF,

    // Sign extension operations
    /// <summary>Extend 8-bit signed integer to i32</summary>
    I32Extend8_s = 0xC0,
    /// <summary>Extend 16-bit signed integer to i32</summary>
    I32Extend16_s = 0xC1,
    /// <summary>Extend 8-bit signed integer to i64</summary>
    I64Extend8_s = 0xC2,
    /// <summary>Extend 16-bit signed integer to i64</summary>
    I64Extend16_s = 0xC3,
    /// <summary>Extend 32-bit signed integer to i64</summary>
    I64Extend32_s = 0xC4,
    
    // Extended instructions (prefix 0xFC)
    /// <summary>Truncate f32 to i32 (signed) with saturation</summary>
    I32TruncSatF32S = 0xFC00,
    /// <summary>Truncate f32 to i32 (unsigned) with saturation</summary>
    I32TruncSatF32U = 0xFC01,
    /// <summary>Truncate f64 to i32 (signed) with saturation</summary>
    I32TruncSatF64S = 0xFC02,
    /// <summary>Truncate f64 to i32 (unsigned) with saturation</summary>
    I32TruncSatF64U = 0xFC03,
    /// <summary>Truncate f32 to i64 (signed) with saturation</summary>
    I64TruncSatF32S = 0xFC04,
    /// <summary>Truncate f32 to i64 (unsigned) with saturation</summary>
    I64TruncSatF32U = 0xFC05,
    /// <summary>Truncate f64 to i64 (signed) with saturation</summary>
    I64TruncSatF64S = 0xFC06,
    /// <summary>Truncate f64 to i64 (unsigned) with saturation</summary>
    I64TruncSatF64U = 0xFC07,
    /// <summary>Initialize memory segment</summary>
    MemoryInit = 0xFC08,
    /// <summary>Drop a data segment</summary>
    DataDrop = 0xFC09,
    /// <summary>Copy memory from one region to another</summary>
    MemoryCopy = 0xFC0A,
    /// <summary>Fill memory with a specified value</summary>
    MemoryFill = 0xFC0B,
    /// <summary>Initialize a table segment</summary>
    TableInit = 0xFC0C,
    /// <summary>Drop an element segment</summary>
    ElemDrop = 0xFC0D,
    /// <summary>Copy elements from one table to another</summary>
    TableCopy = 0xFC0E,
    /// <summary>Grow a table by a specified number of elements</summary>
    TableGrow = 0xFC0F,
    /// <summary>Get the current size of a table</summary>
    TableSize = 0xFC10,
    /// <summary>Fill a table with a specified value</summary>
    TableFill = 0xFC11,
    
   
    
    
    
    /**
     * namespace WebAssemby.Readers.Model;

public enum WasmOpcode
{
    // Previous opcodes remain unchanged...
    
    
    // Vector instructions (prefix 0xFD)
    V128Load = 0xFD00,
    V128Load8x8S = 0xFD01,
    V128Load8x8U = 0xFD02,
    V128Load16x4S = 0xFD03,
    V128Load16x4U = 0xFD04,
    V128Load32x2S = 0xFD05,
    V128Load32x2U = 0xFD06,
    V128Load8Splat = 0xFD07,
    V128Load16Splat = 0xFD08,
    V128Load32Splat = 0xFD09,
    V128Load64Splat = 0xFD0A,
    V128Store = 0xFD0B,

    // Thread instructions (prefix 0xFE)
    MemoryAtomicNotify = 0xFE00,
    MemoryAtomicWait32 = 0xFE01,
    MemoryAtomicWait64 = 0xFE02,
    AtomicFence = 0xFE03,
    I32AtomicLoad = 0xFE10,
    I64AtomicLoad = 0xFE11,
    I32AtomicLoad8U = 0xFE12,
    I32AtomicLoad16U = 0xFE13,
    I64AtomicLoad8U = 0xFE14,
    I64AtomicLoad16U = 0xFE15,
    I64AtomicLoad32U = 0xFE16,
    I32AtomicStore = 0xFE17,
    I64AtomicStore = 0xFE18,
    I32AtomicStore8U = 0xFE19,
    I32AtomicStore16U = 0xFE1A,
    I64AtomicStore8U = 0xFE1B,
    I64AtomicStore16U = 0xFE1C,
    I64AtomicStore32U = 0xFE1D,

    // Reference types (prefix 0xD0-0xD2)
    RefNull = 0xD0,
    RefIsNull = 0xD1,
    RefFunc = 0xD2,

    // Exception handling (prefix 0x06-0x0A)
    Try = 0x06,
    Catch = 0x07,
    Throw = 0x08,
    Rethrow = 0x09,
    BrOnExn = 0x0A
}
     */
    
}