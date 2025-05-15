namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyValueAccess
{
    /*
     * Accessor for the internal memory area.
     *
     * @param p_Index The index of the memory area to access. Defaults to 0.
     * @return The memory area interface for accessing the memory area.
     */
    IWebAssemblyMemoryAreaReadAccess GetInternalMemoryArea(int p_Index = 0);
}