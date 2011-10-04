namespace NCacheFacade
{
    /// <summary>
    ///   Defines interface for CachingStrategy types.
    ///   CachingStrategy types maintain the logic for 
    ///   placement of items in different caches and 
    ///   control the pre- and post- processing of 
    ///   cache items.
    /// </summary>
    /// <remarks>
    ///   Realtime updates of strategy would be cool.
    ///   Possibly use a filewatcher, or implement a plugin model,
    ///   or dynamic class loading.
    /// </remarks>
    public interface ICachingStrategy
    {
        //bool Insert(CacheKey key, object value);

        int CountAll { get; }

        bool Add(Key key, object value);

        object Get(Key key);

        void Remove(Key cacheKey);

        void RemoveAll();
    }
}