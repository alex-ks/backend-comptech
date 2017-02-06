using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Comptech.Backend.Service
{
    /// <summary>
    /// Decryptor class to decrypt encrypted photo, passed as byte array
    /// </summary>
    /// <example>
    /// Decryptor decrypt = ... //Instantiate Decryptor manually or using Di
    /// byte[] decryptedPhoto = decrypt.Decrypt(encryptedPhoto)
    /// </example>
    public class Decryptor : IDecryptor
    {
        //I hope that string will be marshalled to const char *
        [DllImport("libEncrypt.so")]
        private static extern void decrypt(string name_file_in, string name_file_out);

        private readonly ILogger<Decryptor> logger;

        public Decryptor(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<Decryptor>();
        }

        /// <summary>
        /// Decrypts encrypted photo
        /// </summary>
        /// <param name="encryptedPhoto">Encrypted photo as byte array</param>
        /// <returns>byte[] - Decrypted photo as byte array</returns>
        public byte[] Decrypt(byte[] encryptedPhoto)
        {
            FileStream fsEncrypted, fsDecrypted;
            byte[] decBytePhoto;
            using (fsEncrypted = new FileStream("tmp.enc", FileMode.OpenOrCreate, FileAccess.Write))
            {
                fsEncrypted.Write(encryptedPhoto, 0, encryptedPhoto.Length);
                fsEncrypted.Flush();
            }
            FileInfo decFileInfo = new FileInfo("tmp.dec");
            fsDecrypted = decFileInfo.Create();
            logger.LogInformation("Decrypting...");
            decrypt("tmp.enc", "tmp.dec");
            using (fsDecrypted = new FileStream("tmp.dec", FileMode.Create))
            {
                decBytePhoto = new byte[decFileInfo.Length];
                fsDecrypted.Read(decBytePhoto, 0, decBytePhoto.Length);
                fsDecrypted.Flush();
            }
            logger.LogInformation("Cleaning up after decryption...");
            try
            {
                File.Delete("tmp.enc");
                File.Delete("tmp.dec");
            }
            catch(IOException e)
            {
                logger.LogCritical($"Deleting files tmp.enc and tmp.dec failed: {e.Message}. Stack trace:\n{e.StackTrace}");
            }
            return decBytePhoto;
        }
    }
}
