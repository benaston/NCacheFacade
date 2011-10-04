namespace NCacheFacade
{
    /// <summary>
    /// Responsible for defining the interface for concrete types 
    /// exposing cache item encoding according to a specified 
    /// storage style (compressed, encrypted etc).
    /// </summary>
    public interface ICacheItemEncoder
    {
        object Encode<T>(T value, StorageStyle cacheStorageStyle) where T : class;
        T Decode<T>(object value, StorageStyle cacheStorageStyle) where T : class;
    }
}