namespace NCacheFacade
{
    /// <summary>
    /// Responsioble for defining the interface for concrete types 
    /// that expose encryption functionality.
    /// </summary>
    public interface IStringEncryptor
    {
        string Encrypt(string plainText);
        string Decrypt(string cypher);
    }
}
