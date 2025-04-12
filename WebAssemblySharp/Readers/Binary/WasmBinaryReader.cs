using System;
using System.Collections.Generic;
using System.Linq;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.Readers.Binary.MetaData;

namespace WebAssemblySharp.Readers.Binary;

/*
 * Reads and parses WebAssembly binary format (.wasm files) into a structured WasmMetaData object.
 *
 * This class implements a state machine that processes WebAssembly binary data in chunks,
 * maintaining internal state between calls. It supports incremental reading of WASM binary
 * sections including:
 * - Type definitions (function signatures)
 * - Import declarations
 * - Function tables
 * - Memory definitions
 * - Global variables
 * - Export declarations
 * - Code sections with instructions
 * - Data segments
 *
 * The reader handles LEB128 variable-length encoding for integers and properly
 * processes WebAssembly control flow instructions and function bodies.
 */
public class WasmBinaryReader
{
    private static uint CONST_WASM_MAGIC = 0x6d736100;

    private WasmMetaData m_MetaData;
    private long m_NoLimit = -1;

    // Global Reader States
    private ReaderPosition m_ReaderPosition;
    private byte[] m_Buffer;
    private int m_BufferIndex;
    private long m_SectionSize = -1;

    // Section Reader States
    private long m_BytesToSkip = -1;
    private WasmFuncType m_CurrentFuncType;
    private WasmDataInComplete m_CurrentData;
    private WasmMemory m_CurrentMemory;
    private WasmImport m_CurrentImport;
    private WasmExport m_CurrentExport;
    private WasmGlobalInComplete m_CurrentGlobal;
    private WasmCodeInComplete m_CurrentCode;
    private WasmInstruction m_CurrentInstruction;
    private Stack<WasmBlockInstruction> m_InstructionStack;

    public WasmBinaryReader()
    {
        m_ReaderPosition = ReaderPosition.Start;
        m_Buffer = new byte[255];
        m_BufferIndex = 0;
        m_MetaData = new WasmMetaData();
        m_InstructionStack = new Stack<WasmBlockInstruction>(16);
    }

    public void Read(ReadOnlySpan<byte> p_Data)
    {
        int l_Index = 0;

        while (l_Index < p_Data.Length)
        {
            switch (m_ReaderPosition)
            {
                case ReaderPosition.Start:
                    ReadStart(p_Data, ref l_Index);
                    break;
                case ReaderPosition.Section:
                    ReadSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecCustom:
                    ReadCustomSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecType:
                    ReadTypeSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecImport:
                    ReadImportSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecFunction:
                    ReadFunctionSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecTable:
                    ReadTableSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecMemory:
                    ReadMemorySection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecGlobal:
                    ReadGlobalSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecExport:
                    ReadExportSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecStart:
                    ReadSecSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecElement:
                    ReadElementSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecCode:
                    ReadCodeSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SecData:
                    ReadDataSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.SkipSection:
                    SkipSection(p_Data, ref l_Index);
                    break;
                case ReaderPosition.ReadType:
                    ReadType(p_Data, ref l_Index);
                    break;
                case ReaderPosition.ReadImport:
                    ReadImport(p_Data, ref l_Index);
                    break;
                case ReaderPosition.ReadExport:
                    ReadExport(p_Data, ref l_Index);
                    break;
                case ReaderPosition.ReadCode:
                    ReadCode(p_Data, ref l_Index);
                    break;
                case ReaderPosition.ReadInstruction:
                    ReadInstruction(p_Data, ref l_Index);
                    break;
                case ReaderPosition.ReadMemory:
                    ReadMemory(p_Data, ref l_Index);
                    break;
                case ReaderPosition.ReadGlobal:
                    ReadGlobal(p_Data, ref l_Index);
                    break;
                case ReaderPosition.ReadData:
                    ReadData(p_Data, ref l_Index);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ReadInstruction(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        InternalInstructionReader l_Reader = new InternalInstructionReader(this, p_Data, ref p_Index);
        
        while (true)
        {
            if (m_CurrentInstruction == null)
            {
                WasmOpcode? l_Opcode = ReadOpCode(l_Reader);

                if (l_Opcode == null)
                    return;
            
                if (l_Opcode == WasmOpcode.End) {
                    
                    if (m_InstructionStack.Count > 0) {
                        WasmBlockInstruction l_BlockInstruction = m_InstructionStack.Pop();
                        l_BlockInstruction.Finished();
                        continue;
                    }
                    else
                    {
                        if (m_CurrentCode != null)
                            m_ReaderPosition = ReaderPosition.ReadCode;
                        else if (m_CurrentGlobal != null)
                            m_ReaderPosition = ReaderPosition.ReadGlobal;
                        else if (m_CurrentData != null)
                            m_ReaderPosition = ReaderPosition.ReadData;
                        else
                            throw new WasmBinaryReaderException("Invalid end instruction. No Jump back point");
                        
                        return;    
                    }
                }

                if (l_Opcode == WasmOpcode.Else)
                {
                    if (m_InstructionStack.Count > 0) {
                        m_InstructionStack.Peek().HandleBlockOpCode(l_Opcode.Value);
                        continue;
                    }
                    else
                    {
                        throw new WasmBinaryReaderException("Invalid else instruction");
                    }    
                }
            
                m_CurrentInstruction = WasmInstructionFactory.CreateInstruction(l_Opcode.Value);
            }
            
            if (!m_CurrentInstruction.ReadInstruction(l_Reader))
                return;

            
            if (m_InstructionStack.Count > 0)
            {
                m_InstructionStack.Peek().AddInstruction(m_CurrentInstruction);
            }
            else
            {
                if (m_CurrentCode != null)
                    m_CurrentCode.ProcessedInstructions.Add(m_CurrentInstruction);
                else if (m_CurrentGlobal != null)
                    m_CurrentGlobal.ProcessedInstructions.Add(m_CurrentInstruction);
                else
                    m_CurrentData.ProcessedInstructions.Add(m_CurrentInstruction);
            }

            if (m_CurrentInstruction is WasmBlockInstruction)
            {
                m_InstructionStack.Push((WasmBlockInstruction)m_CurrentInstruction);
            }
            
            m_CurrentInstruction = null;
        }
    }

    private WasmOpcode? ReadOpCode(InternalInstructionReader p_Reader)
    {
        ReadOnlySpan<byte> l_Bytes = p_Reader.ReadReadBytes(1);
        
        if (l_Bytes.IsEmpty)
            return null;

        byte l_ByteValue = l_Bytes[0];
        WasmOpcode l_Opcode = (WasmOpcode)l_ByteValue;
        return l_Opcode;
    }

    private void ReadCode(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_CurrentCode == null)
        {
            ulong? l_CodeSize = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

            if (l_CodeSize == null)
                return;

            m_CurrentCode = new WasmCodeInComplete();
            m_CurrentCode.CodeSize = (long)l_CodeSize.Value;
            m_CurrentCode.CodeSizeRemaining = m_CurrentCode.CodeSize;
        }

        if (m_CurrentCode.ProcessedLocals == null)
        {
            ulong? l_LocalSize = ReadLEB128UInt(p_Data, ref p_Index, ref m_CurrentCode.CodeSizeRemaining);

            if (l_LocalSize == null)
                return;

            m_CurrentCode.ProcessedLocals = new WasmCodeLocalInComplete[l_LocalSize.Value];
        }


        if (m_CurrentCode.ProcessedLocals.Length > 0)
        {
            for (int i = 0; i < m_CurrentCode.ProcessedLocals.Length; i++)
            {
                if (m_CurrentCode.ProcessedLocals[i] == null)
                {
                    ulong? l_LocalCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_CurrentCode.CodeSizeRemaining);

                    if (l_LocalCount == null)
                        return;

                    m_CurrentCode.ProcessedLocals[i] = new WasmCodeLocalInComplete();
                    m_CurrentCode.ProcessedLocals[i].Number = (long)l_LocalCount.Value;
                }

                if (m_CurrentCode.ProcessedLocals[i].ValueType == WasmDataType.Unkown)
                {
                    WasmDataType? l_DataType =
                        ReadWasmDataType(p_Data, ref p_Index, ref m_CurrentCode.CodeSizeRemaining);

                    if (l_DataType == null)
                        return;

                    m_CurrentCode.ProcessedLocals[i].ValueType = l_DataType.Value;
                }
            }
            
            
        }
        
        if (m_CurrentCode.CodeSizeRemaining > 0)
        {
            // Read Instructions
            m_ReaderPosition = ReaderPosition.ReadInstruction;
            return;
        }

        for (int i = 0; i < m_MetaData.Code.Length; i++)
        {
            if (m_MetaData.Code[i] == null)
            {
                m_SectionSize = m_SectionSize - m_CurrentCode.CodeSize;
                m_MetaData.Code[i] = m_CurrentCode.ToCompleted();
                m_CurrentCode = null;
                m_ReaderPosition = ReaderPosition.SecCode;
                return;
            }
        }

        throw new WasmBinaryReaderException("Invalid code count");
    }

    private void ReadGlobal(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_CurrentGlobal == null)
        {
            WasmDataType? l_DataType = ReadWasmDataType(p_Data, ref  p_Index, ref m_SectionSize);
            
            if (l_DataType == null)
                return;
            
            m_CurrentGlobal = new WasmGlobalInComplete();
            m_CurrentGlobal.Type = l_DataType.Value;
            
        }

        if (m_CurrentGlobal.MutableReaded == null)
        {
            
            ReadOnlySpan<byte> l_IsMutable = ReaReadBytes(p_Data, ref p_Index, 1, ref m_SectionSize);
            
            if (l_IsMutable.IsEmpty)
                return;

            m_CurrentGlobal.MutableReaded = l_IsMutable[0] != 0;
        }
        
        if (m_CurrentGlobal.ProcessedInstructions.Count == 0)
        {
            // Read Instructions
            m_ReaderPosition = ReaderPosition.ReadInstruction;
            return;
        }

        for (int i = 0; i < m_MetaData.Globals.Length; i++)
        {
            if (m_MetaData.Globals[i] == null)
            {
                m_MetaData.Globals[i] = m_CurrentGlobal.ToCompleted();
                m_CurrentGlobal = null;
                m_ReaderPosition = ReaderPosition.SecGlobal;
                return;
            }
        }

        throw new WasmBinaryReaderException("Invalid code count");
    }
    
    private void ReadData(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_CurrentData == null)
        {
            
            ulong? l_MemoryIndex = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

            if (l_MemoryIndex == null)
                return;

            m_CurrentData = new WasmDataInComplete();
            m_CurrentData.MemoryIndex = (uint)l_MemoryIndex.Value;
            
            if (m_CurrentData.MemoryIndex != 0)
            {
                throw new WasmBinaryReaderException("Invalid memory index " + m_CurrentData.MemoryIndex + " should be 0 by specs");
            }
            
        }
        
        if (m_CurrentData.ProcessedInstructions.Count == 0)
        {
            // Read Instructions
            m_ReaderPosition = ReaderPosition.ReadInstruction;
            return;
        }

        if (m_CurrentData.Data == null)
        {
            ulong? l_DataLength = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);
            
            if (l_DataLength == null)
                return;

            m_CurrentData.Data = new WasmBinaryDataInComplete((long)l_DataLength);
        }

        if (m_CurrentData.Data is WasmBinaryDataInComplete)
        {
            WasmBinaryData l_Data = ReadBinaryData((WasmBinaryDataInComplete)m_CurrentData.Data, p_Data, ref p_Index, ref m_SectionSize);
            
            if (l_Data == null)
                return;
            
            m_CurrentData.Data = l_Data;
        }

        for (int i = 0; i < m_MetaData.Data.Length; i++)
        {
            if (m_MetaData.Data[i] == null)
            {
                m_MetaData.Data[i] = m_CurrentData.ToCompleted();
                m_CurrentData = null;
                m_ReaderPosition = ReaderPosition.SecData;
                return;
            }
        }
        
        throw new WasmBinaryReaderException("Invalid memory count");         
        
    }
    
    private void ReadMemory(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_CurrentMemory == null)
        {
            ReadOnlySpan<byte> l_HasMax = ReaReadBytes(p_Data, ref p_Index, 1, ref m_SectionSize);

            if (l_HasMax.IsEmpty)
                return;

            m_CurrentMemory = new WasmMemory();
            
            if (l_HasMax[0] == 0)
            {
                // Only Min
                m_CurrentMemory.Min = -1;
                m_CurrentMemory.Max = Int64.MaxValue;
            }
            else
            {
                // Min and Max
                m_CurrentMemory.Min = -1;
                m_CurrentMemory.Max = -1;
            }
            
        }
        
        if (m_CurrentMemory.Min == -1)
        {
            ulong? l_Min = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);
                
            if (l_Min == null)
                return;
                
            m_CurrentMemory.Min = (long)l_Min.Value;
        }
            
        if (m_CurrentMemory.Max == -1)
        {
            ulong? l_Max = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);
                
            if (l_Max == null)
                return;
                
            m_CurrentMemory.Max = (long)l_Max.Value;
        }
            
        for (int i = 0; i < m_MetaData.Memory.Length; i++)
        {
            if (m_MetaData.Memory[i] == null)
            {
                m_MetaData.Memory[i] = m_CurrentMemory;
                m_CurrentMemory = null;
                m_ReaderPosition = ReaderPosition.SecMemory;
                return;
            }
        }
        
        throw new WasmBinaryReaderException("Invalid memory count");
    }
    
    private void ReadImport(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_CurrentImport == null)
        {
            ulong? l_ModuleLength = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

            if (l_ModuleLength == null)
                return;

            m_CurrentImport = new WasmUnkownImport();
            m_CurrentImport.Module = new WasmStringInComplete((long)l_ModuleLength.Value);
        }
        
        if (m_CurrentImport.Module is WasmStringInComplete)
        {
            WasmStringInComplete l_Module = (WasmStringInComplete)m_CurrentImport.Module;
            WasmString l_FinalString = ReadString(l_Module, p_Data, ref p_Index, ref m_SectionSize);

            if (l_FinalString == null)
                return;

            m_CurrentImport.Module = l_FinalString;
        }
        
        if (m_CurrentImport.Name == null)
        {
            ulong? l_NameLength = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

            if (l_NameLength == null)
                return;

            m_CurrentImport.Name = new WasmStringInComplete((long)l_NameLength.Value);
        }
        
        if (m_CurrentImport.Name is WasmStringInComplete)
        {
            WasmStringInComplete l_Name = (WasmStringInComplete)m_CurrentImport.Name;
            WasmString l_FinalString = ReadString(l_Name, p_Data, ref p_Index, ref m_SectionSize);

            if (l_FinalString == null)
                return;

            m_CurrentImport.Name = l_FinalString;
        }
        
        if (m_CurrentImport.Kind == WasmExternalKind.Unknown)
        {
            ReadOnlySpan<byte> l_Bytes = ReaReadBytes(p_Data, ref p_Index, 1, ref m_SectionSize);

            if (l_Bytes.IsEmpty)
                return;

            WasmExternalKind l_Kind = (WasmExternalKind)l_Bytes[0];

            WasmImport l_Unkown = m_CurrentImport;

            switch (l_Kind)
            {
                case WasmExternalKind.Function:
                    m_CurrentImport = new WasmImportFunction() { FunctionIndex = -1 };
                    break;
                case WasmExternalKind.Table:
                    break;
                case WasmExternalKind.Memory:
                    m_CurrentImport = new WasmImportMemory() {Min = -1, Max = -2};
                    break;
                case WasmExternalKind.Global:
                    break;
                default:
                    // QQQ handle other external kinds
                    throw new WasmBinaryReaderException("Invalid import kind " + m_CurrentImport.Kind + " not implemnted");
            }
            
            m_CurrentImport.Name = l_Unkown.Name;
            m_CurrentImport.Module = l_Unkown.Module;
        }
        
        if (m_CurrentImport is WasmImportFunction)
        {
            WasmImportFunction l_ImportFunction = (WasmImportFunction)m_CurrentImport;
            
            if (l_ImportFunction.FunctionIndex == -1)
            {
                ulong? l_Index = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_Index == null)
                    return;

                l_ImportFunction.FunctionIndex = (long)l_Index.Value;
            }
        }
        else if (m_CurrentImport is WasmImportMemory)
        {
            WasmImportMemory l_ImportMemory = (WasmImportMemory)m_CurrentImport;

            if (l_ImportMemory.Max == -2)
            {
                ReadOnlySpan<byte> l_HasMax = ReaReadBytes(p_Data, ref p_Index, 1, ref m_SectionSize);

                if (l_HasMax.IsEmpty)
                    return;

                if (l_HasMax[0] == 0)
                {
                    l_ImportMemory.Max = Int64.MaxValue;
                }
                else
                {
                    l_ImportMemory.Max = -1;
                }
            }
            
            if (l_ImportMemory.Min == -1)
            {
                ulong? l_Min = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);
                
                if (l_Min == null)
                    return;
                
                l_ImportMemory.Min = (long)l_Min.Value;        
            }
            
            if (l_ImportMemory.Max == -1)
            {
                ulong? l_Max = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);
                
                if (l_Max == null)
                    return;
                
                l_ImportMemory.Max = (long)l_Max.Value;   
            }
        }
        else
        {
            // QQQ handle other external kinds
            throw new WasmBinaryReaderException("Invalid import kind " + m_CurrentImport.Kind + " not implemnted");    
        }
        
        for (int i = 0; i < m_MetaData.Import.Length; i++)
        {
            if (m_MetaData.Import[i] == null)
            {
                m_MetaData.Import[i] = m_CurrentImport;
                m_CurrentImport = null;
                m_ReaderPosition = ReaderPosition.SecImport;
                return;
            }
        }

        throw new WasmBinaryReaderException("Invalid import count");
        
    }
    
    private void ReadExport(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_CurrentExport == null)
        {
            ulong? l_NameLength = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

            if (l_NameLength == null)
                return;

            m_CurrentExport = new WasmExport();
            m_CurrentExport.Name = new WasmStringInComplete((long)l_NameLength.Value);
            m_CurrentExport.Kind = WasmExternalKind.Unknown;
            m_CurrentExport.Index = -1;
        }

        if (m_CurrentExport.Name is WasmStringInComplete)
        {
            WasmStringInComplete l_Name = (WasmStringInComplete)m_CurrentExport.Name;
            WasmString l_FinalString = ReadString(l_Name, p_Data, ref p_Index, ref m_SectionSize);

            if (l_FinalString == null)
                return;

            m_CurrentExport.Name = l_FinalString;
        }

        if (m_CurrentExport.Kind == WasmExternalKind.Unknown)
        {
            ReadOnlySpan<byte> l_Bytes = ReaReadBytes(p_Data, ref p_Index, 1, ref m_SectionSize);

            if (l_Bytes.IsEmpty)
                return;

            m_CurrentExport.Kind = (WasmExternalKind)l_Bytes[0];
        }

        if (m_CurrentExport.Index == -1)
        {
            ulong? l_Index = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

            if (l_Index == null)
                return;

            m_CurrentExport.Index = (long)l_Index.Value;
        }

        for (int i = 0; i < m_MetaData.Export.Length; i++)
        {
            if (m_MetaData.Export[i] == null)
            {
                m_MetaData.Export[i] = m_CurrentExport;
                m_CurrentExport = null;
                m_ReaderPosition = ReaderPosition.SecExport;
                return;
            }
        }

        throw new WasmBinaryReaderException("Invalid export count");
    }

    private WasmString ReadString(WasmStringInComplete p_StringData, ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        return ReadString(p_StringData, p_Data, ref p_Index, ref m_NoLimit);
    }

    private WasmString ReadString(WasmStringInComplete p_StringData, ReadOnlySpan<byte> p_Data, ref int p_Index,
        ref long p_SectionSize)
    {
        long l_MaxRequestedBytes =
            Math.Min(Math.Min(p_Data.Length - p_Index, p_StringData.BytesRemaining), m_Buffer.Length);

        ReadOnlySpan<byte> l_Bytes = ReaReadBytes(p_Data, ref p_Index, (int)l_MaxRequestedBytes, ref p_SectionSize);

        if (l_Bytes.IsEmpty)
            return null;

        p_StringData.Append(l_Bytes);

        if (p_StringData.BytesRemaining == 0)
        {
            return p_StringData.ToCompleted();
        }

        return null;
    }

    private WasmBinaryData ReadBinaryData(WasmBinaryDataInComplete p_BinaryData, ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        return ReadBinaryData(p_BinaryData, p_Data, ref p_Index, ref m_NoLimit);
    }
    
    private WasmBinaryData ReadBinaryData(WasmBinaryDataInComplete p_BinaryData, ReadOnlySpan<byte> p_Data, ref int p_Index,
        ref long p_SectionSize)
    {
        long l_MaxRequestedBytes =
            Math.Min(Math.Min(p_Data.Length - p_Index, p_BinaryData.BytesRemaining), m_Buffer.Length);

        ReadOnlySpan<byte> l_Bytes = ReaReadBytes(p_Data, ref p_Index, (int)l_MaxRequestedBytes, ref p_SectionSize);

        if (l_Bytes.IsEmpty)
            return null;

        p_BinaryData.Append(l_Bytes);

        if (p_BinaryData.BytesRemaining == 0)
        {
            return p_BinaryData.ToCompleted();
        }

        return null;
    }
    

    private void ReadType(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_CurrentFuncType == null)
        {
            ReadOnlySpan<byte> l_TypeTag = ReaReadBytes(p_Data, ref p_Index, 1, ref m_SectionSize);

            if (l_TypeTag.IsEmpty)
                return;

            if (l_TypeTag[0] != 0x60)
            {
                throw new WasmBinaryReaderException("Invalid type tag " + l_TypeTag[0]);
            }

            m_CurrentFuncType = new WasmFuncType();
        }

        if (m_CurrentFuncType.Parameters == null)
        {
            ulong? l_ParamCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

            if (l_ParamCount == null)
                return;

            ulong l_ParamCountValue = l_ParamCount.Value;
            m_CurrentFuncType.Parameters = new WasmDataType[l_ParamCountValue];
        }

        if (m_CurrentFuncType.Parameters.Length > 0)
        {
            for (int i = 0; i < m_CurrentFuncType.Parameters.Length; i++)
            {
                if (m_CurrentFuncType.Parameters[i] == WasmDataType.Unkown)
                {
                    WasmDataType? l_DataType = ReadWasmDataType(p_Data, ref p_Index, ref m_SectionSize);

                    if (l_DataType == null)
                        return;

                    m_CurrentFuncType.Parameters[i] = l_DataType.Value;
                }
            }
        }

        if (m_CurrentFuncType.Results == null)
        {
            ulong? l_ResultCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

            if (l_ResultCount == null)
                return;

            ulong l_ResultCountValue = l_ResultCount.Value;
            m_CurrentFuncType.Results = new WasmDataType[l_ResultCountValue];
        }

        if (m_CurrentFuncType.Results.Length > 0)
        {
            for (int i = 0; i < m_CurrentFuncType.Results.Length; i++)
            {
                if (m_CurrentFuncType.Results[i] == WasmDataType.Unkown)
                {
                    WasmDataType? l_DataType = ReadWasmDataType(p_Data, ref p_Index, ref m_SectionSize);

                    if (l_DataType == null)
                        return;

                    m_CurrentFuncType.Results[i] = l_DataType.Value;
                }
            }
        }

        for (int i = 0; i < m_MetaData.FunctionType.Length; i++)
        {
            if (m_MetaData.FunctionType[i] == null)
            {
                m_MetaData.FunctionType[i] = m_CurrentFuncType;
                m_CurrentFuncType = null;
                m_ReaderPosition = ReaderPosition.SecType;
                return;
            }
        }

        throw new WasmBinaryReaderException("Invalid function type count");
    }

    private WasmDataType? ReadWasmDataType(ReadOnlySpan<byte> p_Data, ref int p_Index, ref long p_Limit)
    {
        ReadOnlySpan<byte> l_Bytes = ReadReadBytes(p_Data, ref p_Index, 1, ref p_Limit, true);

        if (l_Bytes.IsEmpty)
            return null;

        byte l_ByteValue = l_Bytes[0];
        WasmDataType l_Value = (WasmDataType)l_ByteValue;

        if (l_Value == WasmDataType.Unkown)
        {
            throw new WasmBinaryReaderException("Invalid data type " + l_ByteValue);
        }

        return l_Value;
    }

    private void SkipSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_BytesToSkip < 0)
        {
            throw new WasmBinaryReaderException("Invalid state for skipping section. Bytes to skip is not set");
        }

        SkipBytes(p_Data, ref p_Index, ref m_BytesToSkip);

        if (m_BytesToSkip == 0)
        {
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }
    }

    private void SkipBytes(ReadOnlySpan<byte> p_Data, ref int p_Index, ref long p_BytesToSkip)
    {
        if (m_BufferIndex > 0)
        {
            if (p_BytesToSkip < m_BufferIndex)
            {
                // Skip the remaining bytes
                m_BufferIndex = m_BufferIndex - (int)p_BytesToSkip;
                p_BytesToSkip = 0;
            }
            else
            {
                // Skip the buffer
                p_BytesToSkip -= m_BufferIndex;
                m_BufferIndex = 0;
            }
        }

        if (p_BytesToSkip > 0)
        {
            int l_BytesInBuffer = p_Data.Length - p_Index;

            if (l_BytesInBuffer >= p_BytesToSkip)
            {
                // Skip the remaining bytes
                p_Index += (int)p_BytesToSkip;
                p_BytesToSkip = 0;
            }
            else
            {
                // Skip the buffer
                p_BytesToSkip -= l_BytesInBuffer;
                p_Index = p_Data.Length;
            }
        }
    }
    
    private bool IsSectionSizeValid(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (m_SectionSize < 0)
        {
            ulong? l_Size = ReadLEB128UInt(p_Data, ref p_Index);

            if (l_Size == null)
                return false;

            m_SectionSize = (long)l_Size.Value;
        }

        // If Section Contains not code got back to section selection
        if (m_SectionSize == 0)
        {
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
            return false;
        }

        // Section size is valid
        return true;
    }
    
    private void ReadCodeSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;

        if (m_SectionSize > 0)
        {
            if (m_MetaData.Code == null)
            {
                ulong? l_CodeCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_CodeCount == null)
                    return;

                m_MetaData.Code = new WasmCode[l_CodeCount.Value];
            }

            if (m_MetaData.Code.Length == 0)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
                return;
            }

            if (m_MetaData.Code[m_MetaData.Code.Length - 1] == null)
            {
                m_ReaderPosition = ReaderPosition.ReadCode;
            }
            else
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
            }
        }
        else
        {
            // Section is empty
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }
    }

    private void ReadElementSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;

        throw new WasmBinaryReaderException("Elemet section not implemented");
    }

    private void ReadSecSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;
        
        throw new WasmBinaryReaderException("Sec section not implemented");
    }

    private void ReadDataSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;
        
        if (m_SectionSize > 0)
        {
            if (m_MetaData.Data == null)
            {
                ulong? l_DataCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_DataCount == null)
                    return;

                m_MetaData.Data = new WasmData[l_DataCount.Value];
            }

            if (m_MetaData.Data.Length == 0)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
                return;
            }

            if (m_MetaData.Data[m_MetaData.Data.Length - 1] == null)
            {
                m_ReaderPosition = ReaderPosition.ReadData;
            }
            else
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid data section size");
                }

                m_SectionSize = -1;
            }
        }
        else
        {
            // Section is empty
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }
    }
    
    private void ReadExportSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;

        if (m_SectionSize > 0)
        {
            if (m_MetaData.Export == null)
            {
                ulong? l_ExportCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_ExportCount == null)
                    return;

                m_MetaData.Export = new WasmExport[l_ExportCount.Value];
            }

            if (m_MetaData.Export.Length == 0)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
                return;
            }

            if (m_MetaData.Export[m_MetaData.Export.Length - 1] == null)
            {
                m_ReaderPosition = ReaderPosition.ReadExport;
            }
            else
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
            }
        }
        else
        {
            // Section is empty
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }
    }

    private void ReadGlobalSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;
        
        
        if (m_SectionSize > 0)
        {
            if (m_MetaData.Globals == null)
            {
                ulong? l_GlobalCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_GlobalCount == null)
                    return;

                m_MetaData.Globals = new WasmGlobal[l_GlobalCount.Value];
            }

            if (m_MetaData.Globals.Length == 0)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
                return;
            }

            if (m_MetaData.Globals[m_MetaData.Globals.Length - 1] == null)
            {
                m_ReaderPosition = ReaderPosition.ReadGlobal;
            }
            else
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
            }
        }
        else
        {
            // Section is empty
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }
        
    }

    private void ReadMemorySection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;
        
        if (m_SectionSize > 0)
        {
            if (m_MetaData.Memory == null)
            {
                ulong? l_MemoryCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_MemoryCount == null)
                    return;

                m_MetaData.Memory = new WasmMemory[l_MemoryCount.Value];
            }

            if (m_MetaData.Memory.Length == 0)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
                return;
            }

            if (m_MetaData.Memory[m_MetaData.Memory.Length - 1] == null)
            {
                m_ReaderPosition = ReaderPosition.ReadMemory;
            }
            else
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
            }
        }
        else
        {
            // Section is empty
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }
    }

    private void ReadTableSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;
        
        throw new WasmBinaryReaderException("Table section not implemented");
    }

    private void ReadFunctionSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;

        if (m_SectionSize > 0)
        {
            if (m_MetaData.FuncIndex == null)
            {
                ulong? l_IndexCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_IndexCount == null)
                    return;

                m_MetaData.FuncIndex = new long[l_IndexCount.Value];
                Array.Fill(m_MetaData.FuncIndex, -1);
            }

            if (m_MetaData.FuncIndex.Length == 0)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid function section size");
                }

                m_SectionSize = -1;
                return;
            }

            for (int i = 0; i < m_MetaData.FuncIndex.Length; i++)
            {
                if (m_MetaData.FuncIndex[i] == -1)
                {
                    ulong? l_Index = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                    if (l_Index == null)
                        return;

                    m_MetaData.FuncIndex[i] = (long)l_Index.Value;
                }
            }

            if (m_MetaData.FuncIndex[m_MetaData.FuncIndex.Length - 1] != -1)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid function section size");
                }

                m_SectionSize = -1;
            }
        }
        else
        {
            // Section is empty
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }
    }

    private void ReadImportSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;

        if (m_SectionSize > 0)
        {
            
            if (m_MetaData.Import == null)
            {
                ulong? l_ImportCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_ImportCount == null)
                    return;

                m_MetaData.Import = new WasmImport[l_ImportCount.Value];
            }

            if (m_MetaData.Import.Length == 0)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
                return;
            }

            if (m_MetaData.Import[m_MetaData.Import.Length - 1] == null)
            {
                m_ReaderPosition = ReaderPosition.ReadImport;
            }
            else
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
            }
            
        }
        else
        {
            // Section is empty
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }

    }

    private void ReadTypeSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;

        if (m_SectionSize > 0)
        {
            if (m_MetaData.FunctionType == null)
            {
                ulong? l_TypeCount = ReadLEB128UInt(p_Data, ref p_Index, ref m_SectionSize);

                if (l_TypeCount == null)
                    return;

                m_MetaData.FunctionType = new WasmFuncType[l_TypeCount.Value];
            }

            if (m_MetaData.FunctionType.Length == 0)
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
                return;
            }

            if (m_MetaData.FunctionType[m_MetaData.FunctionType.Length - 1] == null)
            {
                m_ReaderPosition = ReaderPosition.ReadType;
            }
            else
            {
                m_ReaderPosition = ReaderPosition.Section;

                if (m_SectionSize > 0)
                {
                    throw new WasmBinaryReaderException("Invalid type section size");
                }

                m_SectionSize = -1;
            }
        }
        else
        {
            // Section is empty
            m_ReaderPosition = ReaderPosition.Section;
            m_SectionSize = -1;
        }
    }


    private void ReadCustomSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        if (!IsSectionSizeValid(p_Data, ref p_Index))
            return;

        m_ReaderPosition = ReaderPosition.SkipSection;
        m_BytesToSkip = m_SectionSize;
        m_SectionSize = -1;
    }

    private void ReadSection(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        ReadOnlySpan<byte> l_SectionId = ReaReadBytes(p_Data, ref p_Index, 1);

        if (l_SectionId.IsEmpty)
            return;

        byte l_SectionIdValue = l_SectionId[0];

        if (l_SectionIdValue == 0)
        {
            m_ReaderPosition = ReaderPosition.SecCustom;
        }
        else if (l_SectionIdValue == 1)
        {
            m_ReaderPosition = ReaderPosition.SecType;
        }
        else if (l_SectionIdValue == 2)
        {
            m_ReaderPosition = ReaderPosition.SecImport;
        }
        else if (l_SectionIdValue == 3)
        {
            m_ReaderPosition = ReaderPosition.SecFunction;
        }
        else if (l_SectionIdValue == 4)
        {
            m_ReaderPosition = ReaderPosition.SecTable;
        }
        else if (l_SectionIdValue == 5)
        {
            m_ReaderPosition = ReaderPosition.SecMemory;
        }
        else if (l_SectionIdValue == 6)
        {
            m_ReaderPosition = ReaderPosition.SecGlobal;
        }
        else if (l_SectionIdValue == 7)
        {
            m_ReaderPosition = ReaderPosition.SecExport;
        }
        else if (l_SectionIdValue == 8)
        {
            m_ReaderPosition = ReaderPosition.SecStart;
        }
        else if (l_SectionIdValue == 9)
        {
            m_ReaderPosition = ReaderPosition.SecElement;
        }
        else if (l_SectionIdValue == 10)
        {
            m_ReaderPosition = ReaderPosition.SecCode;
        }
        else if (l_SectionIdValue == 11)
        {
            m_ReaderPosition = ReaderPosition.SecData;
        }
        else
        {
            throw new WasmBinaryReaderException("Invalid section id " + l_SectionIdValue);
        }

        m_SectionSize = -1;
    }

    private long? ReadLEB128Int(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        long l_Limit = -1;
        return ReadLEB128Int(p_Data, ref p_Index, ref l_Limit);
    }

    private long? ReadLEB128Int(ReadOnlySpan<byte> p_Data, ref int p_Index, ref long p_Limit)
    {
        long l_Result = 0;
        int l_Shift = 0;
        int l_RequestedBytes = 0;

        byte l_ByteValue;
        
        while (true)
        {
            l_RequestedBytes++;
            ReadOnlySpan<byte> l_NextByte = ReadReadBytes(p_Data, ref p_Index, l_RequestedBytes, ref p_Limit, false);

            if (l_NextByte.IsEmpty)
            {
                return null;
            }

            l_ByteValue = l_NextByte[l_RequestedBytes - 1];
            l_Result |= (long)(l_ByteValue & 0x7F) << l_Shift;

            if ((l_ByteValue & 0x80) == 0)
                break;

            l_Shift += 7;
        }
        
        if (l_Shift < 64 && (l_ByteValue & 0x40) != 0)
        {
            l_Result |= ~0L << l_Shift;
        }

        // Reset buffer
        ResetBuffer(ref p_Limit, l_RequestedBytes);
        return l_Result;
    }
    
    private ulong? ReadLEB128UInt(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        long l_Limit = -1;
        return ReadLEB128UInt(p_Data, ref p_Index, ref l_Limit);
    }

    private ulong? ReadLEB128UInt(ReadOnlySpan<byte> p_Data, ref int p_Index, ref long p_Limit)
    {
        ulong l_Result = 0;
        int l_Shift = 0;
        int l_RequestedBytes = 0;

        while (true)
        {
            l_RequestedBytes++;
            ReadOnlySpan<byte> l_NextByte = ReadReadBytes(p_Data, ref p_Index, l_RequestedBytes, ref p_Limit, false);

            if (l_NextByte.IsEmpty)
            {
                return null;
            }

            byte l_ByteValue = l_NextByte[l_RequestedBytes - 1];
            l_Result |= (ulong)(l_ByteValue & 0x7F) << l_Shift;

            if ((l_ByteValue & 0x80) == 0)
                break;

            l_Shift += 7;
        }

        // Reset buffer
        ResetBuffer(ref p_Limit, l_RequestedBytes);
        return l_Result;
    }

    private void ReadStart(ReadOnlySpan<byte> p_Data, ref int p_Index)
    {
        ReadOnlySpan<byte> l_Bytes = ReaReadBytes(p_Data, ref p_Index, 8);

        if (l_Bytes.IsEmpty)
            return;

        uint l_MagicValue = BitConverter.ToUInt32(l_Bytes.Slice(0, 4));

        if (l_MagicValue != CONST_WASM_MAGIC)
        {
            throw new WasmBinaryReaderException("Invalid magic value " + l_MagicValue);
        }

        m_MetaData.Version = BitConverter.ToInt32(l_Bytes.Slice(4, 4));
        m_ReaderPosition = ReaderPosition.Section;
    }

    private ReadOnlySpan<byte> ReadReadBytes(ReadOnlySpan<byte> p_Data, ref int p_Index,
        int p_RequestedBytes, ref long p_Limit, bool p_ResetBuffer)
    {
        while (true)
        {
            if (m_BufferIndex == p_RequestedBytes)
            {
                if (p_ResetBuffer)
                {
                    ResetBuffer(ref p_Limit, p_RequestedBytes);
                }

                return new ReadOnlySpan<byte>(m_Buffer, 0, p_RequestedBytes);
            }
            else if (m_BufferIndex > p_RequestedBytes)
            {
                throw new WasmBinaryReaderException("Buffer overflow. Should not happen");
            }

            if (p_Index >= p_Data.Length)
                // Reached the end of the current chunk 
                return new ReadOnlySpan<byte>();


            m_Buffer[m_BufferIndex] = p_Data[p_Index];
            m_BufferIndex++;
            p_Index++;

            if (m_BufferIndex >= m_Buffer.Length)
            {
                throw new WasmBinaryReaderException(
                    $"Buffer overflow. Internal Read Buffer is too small ({m_Buffer.Length})");
            }
        }
    }

    private void ResetBuffer(ref long p_Limit, int p_RequestedBytes)
    {
        if (p_Limit >= 0)
        {
            p_Limit -= p_RequestedBytes;

            if (p_Limit < 0)
            {
                throw new WasmBinaryReaderException("Buffer overflow");
            }
        }

        m_BufferIndex = 0;
    }

    private ReadOnlySpan<byte> ReaReadBytes(ReadOnlySpan<byte> p_Data, ref int p_Index, int p_RequestedBytes,
        ref long p_Limit)
    {
        return ReadReadBytes(p_Data, ref p_Index, p_RequestedBytes, ref p_Limit, true);
    }

    private ReadOnlySpan<byte> ReaReadBytes(ReadOnlySpan<byte> p_Data, ref int p_Index, int p_RequestedBytes)
    {
        return ReadReadBytes(p_Data, ref p_Index, p_RequestedBytes, ref m_NoLimit, true);
    }

    public WasmMetaData Finish()
    {
        if (m_ReaderPosition == ReaderPosition.Start)
        {
            throw new InvalidOperationException("Invalid reader state");
        }
        
        if (m_BufferIndex > 0)
        {
            throw new WasmBinaryReaderException("Invalid state. Buffer is not empty");
        }

        return m_MetaData;
    }

    private enum ReaderPosition
    {
        Start,
        Section,
        SecCustom,
        SecType,
        SecImport,
        SecFunction,
        SecTable,
        SecMemory,
        SecGlobal,
        SecExport,
        SecStart,
        SecElement,
        SecCode,
        SecData,
        SkipSection,
        ReadType,
        ReadImport,
        ReadExport,
        ReadCode,
        ReadInstruction,
        ReadMemory,
        ReadGlobal,
        ReadData
    }

    private ref struct InternalInstructionReader : IWasmBinaryInstructionReader
    {
        private WasmBinaryReader m_Parent;
        private ReadOnlySpan<byte> m_Data;
        private ref int m_Index;

        public InternalInstructionReader(WasmBinaryReader p_Parent, ReadOnlySpan<byte> p_Data, ref int p_Index)
        {
            m_Parent = p_Parent;
            m_Data = p_Data;
            m_Index = ref p_Index;
        }

        public ulong? ReadLEB128UInt()
        {
            if (m_Parent.m_CurrentCode != null) 
                return m_Parent.ReadLEB128UInt(m_Data, ref m_Index, ref m_Parent.m_CurrentCode.CodeSizeRemaining);
            
            return m_Parent.ReadLEB128UInt(m_Data, ref m_Index, ref m_Parent.m_SectionSize);
        }

        public long? ReadLEB128Int()
        {
            if (m_Parent.m_CurrentCode != null)
                return m_Parent.ReadLEB128Int(m_Data, ref m_Index, ref m_Parent.m_CurrentCode.CodeSizeRemaining);
            
            return m_Parent.ReadLEB128Int(m_Data, ref m_Index, ref m_Parent.m_SectionSize);
        }

        public ReadOnlySpan<byte> ReadReadBytes(int p_Length)
        {
            if (m_Parent.m_CurrentCode != null)
                return m_Parent.ReadReadBytes(m_Data, ref m_Index, p_Length, ref m_Parent.m_CurrentCode.CodeSizeRemaining, true);
            
            return m_Parent.ReadReadBytes(m_Data, ref m_Index, p_Length, ref m_Parent.m_SectionSize, true);
        }
    }
}