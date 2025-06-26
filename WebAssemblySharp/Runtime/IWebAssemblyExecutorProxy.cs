using System;

namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyExecutorProxy
{
    
    /*
     * Sets the proxy type for the executor.
     *
     * @param p_ProxyType The type of the proxy to set
     */
    void SetProxyType(Type p_ProxyType);
    
    /*
     * Converts the executor to a specific interface type.
     *
     * @typeparam T The interface type to convert to
     * @return If the Interface is not found it will return null.
     */
    T AsInterface<T>();
}