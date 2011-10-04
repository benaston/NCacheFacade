namespace NCacheFacade
{
    /// <summary>
    /// Defines the interface for cache implementations.
    /// </summary>    
    public interface ICacheImplementation
    {
        /// <summary>
        /// Indicates whether this CacheImplementation may be used.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <returns>The key assigned the cache item.</returns>
        bool Add(Key key, object value);

        //bool Insert(CacheKey key, object value);

        int Count { get; }

        object Get(Key key);

        bool Supports(ExpirationType expirationType);

        void Remove(Key key);

        void RemoveAll();
    }
}