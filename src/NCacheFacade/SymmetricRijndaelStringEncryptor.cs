namespace NCacheFacade
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Responsible for providing symmetric encryption functionality 
    /// based upon the AES Rijndael algorithm.    
    /// </summary>
    public class SymmetricRijndaelStringEncryptor : IStringEncryptor
    {
        private const string _encryptionKey = "MTIzNDU2Nzg5MDEyMzQ1Njc4OTAxMjM0NTY3ODkwMTI="; //decodes to 32bytes
        private const string _initialisationVector = "MTIzNDU2Nzg5MDEyMzQ1Ng=="; //decodes to 16bytes

        /// <summary>
        /// http://stackoverflow.com/questions/1629828/how-to-encrypt-a-string-in-net        
        /// </summary>
        public string Encrypt(string plainText)
        {
            // Instantiate a new RijndaelManaged object to perform string symmetric encryption
            var rijndaelCipher = new RijndaelManaged();

            // Set key and IV - NOT NEEDED? http://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged.aspx
            rijndaelCipher.Key = Convert.FromBase64String(_encryptionKey);
            rijndaelCipher.IV = Convert.FromBase64String(_initialisationVector);

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            var memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our RijndaelManaged object
            ICryptoTransform rijndaelEncryptor = rijndaelCipher.CreateEncryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            var cryptoStream = new CryptoStream(memoryStream, rijndaelEncryptor, CryptoStreamMode.Write);

            // Convert the plainText string into a byte array
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            // Encrypt the input plaintext string
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);

            // Complete the encryption process
            cryptoStream.FlushFinalBlock();

            // Convert the encrypted data from a MemoryStream to a byte array
            byte[] cypherBytes = memoryStream.ToArray();

            // Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();

            // Convert the encrypted byte array to a base64 encoded string
            string cypherText = Convert.ToBase64String(cypherBytes, 0, cypherBytes.Length);

            // Return the encrypted data as a string
            return cypherText;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/1629828/how-to-encrypt-a-string-in-net
        /// </summary>
        public string Decrypt(string cypher)
        {
            // Instantiate a new RijndaelManaged object to perform string symmetric encryption
            var rijndaelCipher = new RijndaelManaged();

            // Set key and IV
            rijndaelCipher.Key = Convert.FromBase64String(_encryptionKey);
            rijndaelCipher.IV = Convert.FromBase64String(_initialisationVector);

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            var memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our RijndaelManaged object
            ICryptoTransform rijndaelDecryptor = rijndaelCipher.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            var cryptoStream = new CryptoStream(memoryStream, rijndaelDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            try
            {
                // Convert the cyphertext string into a byte array
                byte[] cypherBytes = Convert.FromBase64String(cypher);

                // Decrypt the input cyphertext string
                cryptoStream.Write(cypherBytes, 0, cypherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the encrypted byte array to a base64 encoded string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the encrypted data as a string
            return plainText;
        }
    }
}