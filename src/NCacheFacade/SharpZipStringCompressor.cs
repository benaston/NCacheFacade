namespace NCacheFacade
{
    using System;
    using System.IO;
    using System.Text;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

    /// <summary>
    /// Responsible for exposing Zip compression functionality for the compression of strings.
    /// http://archdipesh.blogspot.com/2008/01/how-to-compressuncompress-string-in-net.html
    /// </summary>
    public class SharpZipStringCompressor : IStringCompressor
    {        
        public string Compress(string uncompressedString)
        {
            var stringAsBytes = Encoding.UTF8.GetBytes(uncompressedString);
            var ms = new MemoryStream();
            var outputStream = new DeflaterOutputStream(ms);
            outputStream.Write(stringAsBytes, 0, stringAsBytes.Length);
            outputStream.Close();
            var compressedData = ms.ToArray();

            return Convert.ToBase64String(compressedData, 0, compressedData.Length);
        }

        public string Decompress(string compressedString)
        {
            var uncompressedString = string.Empty;
            var totalLength = 0;
            var inputAsBytes = Convert.FromBase64String(compressedString);;
            var writeData = new byte[4096];
            var inputStream = new InflaterInputStream(new MemoryStream(inputAsBytes));
            
            for(;;)
            {
                var size = inputStream.Read(writeData, 0, writeData.Length);
            
                if (size > 0)
                {
                    totalLength += size;
                    uncompressedString += Encoding.UTF8.GetString(writeData, 0, size);
                }
                else
                {
                    break;
                }
            }
            
            inputStream.Close();
            
            return uncompressedString;
        }
    }
}