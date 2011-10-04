namespace NCacheFacade
{
    /// <summary>
    /// Responisble for defining the interface for concrete types
    /// that expose functionality for the generation of a 
    /// cache item "friendly name" - unique human-
    /// comprehensible key for a cache item. 
    /// </summary>
    public interface IFriendlyNameCreationStrategy
    {
        string GenerateFriendlyName(params object[] args);
    }
}