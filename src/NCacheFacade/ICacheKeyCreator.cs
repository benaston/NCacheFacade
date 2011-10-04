namespace NCacheFacade
{
    using System;

    /// <summary>
    ///   Responsible for defining the interface for concrete 
    ///   types that expose functionality for the generation of 
    ///   cache keys, using supplied storage information and a
    ///   friendly name.
    /// </summary>
    public interface ICacheKeyCreator
    {
        Key Create(TimeSpan durationToStore,
                   StorageStyle cacheStorageStyle,
                   ExpirationType cacheExpirationType,
                   string friendlyName);
    }
}