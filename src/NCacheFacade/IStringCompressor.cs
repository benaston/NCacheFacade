namespace NCacheFacade
{
    /// <summary>
    /// Responsible for defining the interface for concrete types 
    /// exposing string compression functionality.
    /// </summary>
    public interface IStringCompressor
    {
        string Compress(string text);
        string Decompress(string text);
    }
}
