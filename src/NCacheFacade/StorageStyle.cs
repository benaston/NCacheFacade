namespace NCacheFacade
{
    /// <summary>
    /// Used to indicate how an item should be/is stored in the cache.
    /// </summary>
    public enum StorageStyle
    {
        Unmodified = 0,
        Compressed,
        Encrypted,
        CompressedAndEncrypted
    }
}