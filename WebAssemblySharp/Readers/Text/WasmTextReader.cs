using System;
using System.Collections.Generic;
using System.Text;
using WebAssemblySharp.Readers.Text.Model;

namespace WebAssemblySharp.Readers.Text;

public class WasmTextReader
{
    private readonly List<WasmReaderElement> m_Elements;
    private ParseState m_ParseState;
    private readonly StringBuilder m_TextBuffer;

    public WasmTextReader()
    {
        m_Elements = new List<WasmReaderElement>();
        m_TextBuffer = new StringBuilder(64);
        m_ParseState = ParseState.Normal;
    }


    public void Read(ReadOnlySpan<char> p_Text)
    {
        var l_TextLength = p_Text.Length;
        var l_Index = -1;

        while (MoveIndex(ref l_Index, l_TextLength))
        {
            var l_CurrentChar = p_Text[l_Index];

            switch (m_ParseState)
            {
                case ParseState.Normal:
                    if (l_CurrentChar == '(')
                    {
                        ParseTextBuffer();
                        // Start of a new element
                        m_Elements.Add(WasmReaderElement.OpenElement);
                    }
                    else if (l_CurrentChar == ')')
                    {
                        ParseTextBuffer();
                        m_Elements.Add(WasmReaderElement.CloseElement);
                    }
                    else if (l_CurrentChar == ';')
                    {
                        ParseTextBuffer();
                        m_ParseState = ParseState.CommentSingleLine;
                    }
                    else if (l_CurrentChar == '"')
                    {
                        ParseTextBuffer();
                        m_ParseState = ParseState.String;
                    }
                    else if (IsWithSpaceOrControlChar(l_CurrentChar))
                    {
                        ParseTextBuffer();
                    }
                    else
                    {
                        // Text
                        m_TextBuffer.Append(l_CurrentChar);
                    }

                    break;
                case ParseState.CommentMultiLine:

                    if (l_CurrentChar == ';')
                    {
                        if (m_TextBuffer.Length == 0)
                            m_ParseState = ParseState.CommentSingleLine; // Single line comment
                        else
                            m_ParseState = ParseState.Normal; // Comment end
                    }

                    break;
                case ParseState.CommentSingleLine:

                    if (IsNewLine(l_CurrentChar))
                        m_ParseState = ParseState.Normal;

                    break;
                case ParseState.String:

                    if (l_CurrentChar == '"')
                    {
                        ParseTextBuffer();
                        m_ParseState = ParseState.Normal;
                    }
                    else
                    {
                        m_TextBuffer.Append(l_CurrentChar);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private bool IsNewLine(char p_CurrentChar)
    {
        return p_CurrentChar == '\n' || p_CurrentChar == '\r';
    }

    public List<WasmReaderElement> Finish()
    {
        ParseTextBuffer();
        return m_Elements;
    }

    private void ParseTextBuffer()
    {
        if (m_TextBuffer.Length == 0) return;

        var l_Text = m_TextBuffer.ToString();
        m_TextBuffer.Clear();

        // Key words
        if (HandleKeyWords(l_Text, "module", WasmReaderElement.ModuleElement))
            return;

        if (HandleKeyWords(l_Text, "func", WasmReaderElement.FunctionElement))
            return;

        if (HandleKeyWords(l_Text, "export", WasmReaderElement.ExportElement))
            return;

        if (HandleKeyWords(l_Text, "param", WasmReaderElement.ParamElement))
            return;

        if (HandleKeyWords(l_Text, "result", WasmReaderElement.ResultElement))
            return;

        if (HandleKeyWords(l_Text, "i32", WasmReaderElement.I32Element))
            return;

        if (HandleKeyWords(l_Text, "i64", WasmReaderElement.I64Element))
            return;

        if (HandleKeyWords(l_Text, "f32", WasmReaderElement.F32Element))
            return;

        if (HandleKeyWords(l_Text, "F64", WasmReaderElement.F64Element))
            return;

        if (HandleKeyWords(l_Text, "local", WasmReaderElement.LocalElement))
            return;

        if (HandleKeyWords(l_Text, "local", WasmReaderElement.LocalElement))
            return;

        if (HandleKeyWords(l_Text, "table", WasmReaderElement.TableElement))
            return;

        if (HandleKeyWords(l_Text, "mem", WasmReaderElement.MemElement))
            return;

        if (HandleKeyWords(l_Text, "global", WasmReaderElement.GlobalElement))
            return;

        if (HandleKeyWords(l_Text, "data", WasmReaderElement.DataElement))
            return;

        m_Elements.Add(new WasmReaderElement(WasmReaderElementKind.Text, l_Text));
    }

    private bool HandleKeyWords(string p_Text, string p_Keyword, WasmReaderElement p_PerfectMatch)
    {
        var l_MatchKind = IsKeywordMatch(p_Text, p_Keyword);

        if (l_MatchKind == MatchKind.Perfect)
        {
            m_Elements.Add(p_PerfectMatch);
            return true;
        }

        if (l_MatchKind == MatchKind.Partial)
        {
            m_Elements.Add(new WasmReaderElement(p_PerfectMatch.Kind, p_Text.Substring(p_Keyword.Length + 1)));
            return true;
        }

        return false;
    }

    private MatchKind IsKeywordMatch(string p_Text, string p_Keyword)
    {
        if (!p_Text.StartsWith(p_Keyword))
            return MatchKind.None;

        if (p_Text.Length == p_Keyword.Length)
            return MatchKind.Perfect;

        if (p_Text[p_Keyword.Length] == '.')
            return MatchKind.Partial;

        return MatchKind.None;
    }

    private bool MoveIndex(ref int p_Index, int p_TextLength)
    {
        p_Index++;

        if (p_Index >= p_TextLength)
            // Reached the end of the text
            return false;

        return true;

    }

    private bool IsWithSpaceOrControlChar(char p_Char)
    {
        return char.IsWhiteSpace(p_Char) || char.IsControl(p_Char);
    }

    private enum MatchKind
    {
        Perfect,
        Partial,
        None
    }

    private enum ParseState
    {
        Normal,
        CommentMultiLine,
        CommentSingleLine,
        String
    }
}