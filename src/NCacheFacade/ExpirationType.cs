namespace NCacheFacade
{
    /// <summary>
    /// Used to indicate what caching options are available
    /// on a given cache.
    /// NOTE: BA; Memcached does not expose a sliding 
    /// expiration option).
    /// </summary>
    public enum ExpirationType
    {
        Absolute = 0,
        Sliding 
    }
}