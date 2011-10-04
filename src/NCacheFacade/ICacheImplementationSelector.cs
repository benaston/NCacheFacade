namespace NCacheFacade
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Responsible for defining the interface for concrete 
    /// types to implement cache implementation selection 
    /// logic.
    /// </summary>
    public interface ICacheImplementationSelector
    {
        ICacheImplementation Select(TimeSpan durationToCacheItemFor, 
                                    StorageStyle cacheStorageStyle, 
                                    ExpirationType expirationType);
        Dictionary<string, ICacheImplementation> CacheImplementations { get;  }
    }
}