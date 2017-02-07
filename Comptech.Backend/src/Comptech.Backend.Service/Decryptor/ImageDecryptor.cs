using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Comptech.Backend.Service.Decryptor
{
    /// <summary>
    /// ImageDecryptor class to decrypt encrypted photo, passed as byte array
    /// </summary>
    /// <example>
    /// ImageDecryptor decrypt = ... //Instantiate ImageDecryptor manually or using Di
    /// byte[] decryptedPhoto = decrypt.Decrypt(encryptedPhoto)
    /// </example>
    public class ImageDecryptor : IImageDecryptor
    {
        //I hope that string will be marshalled to const char *
        [DllImport("libEncrypt.so")]
        private static extern void decrypt(string name_file_in, string name_file_out);

        private readonly ILogger<ImageDecryptor> logger;

        public ImageDecryptor(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<ImageDecryptor>();
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

            var guid = Guid.NewGuid();
            var tmpEncName = string.Format($"tmp{guid}.enc");
            var tmpDecName = string.Format($"tmp{guid}.dec");

            using (fsEncrypted = new FileStream(tmpEncName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fsEncrypted.Write(encryptedPhoto, 0, encryptedPhoto.Length);
                fsEncrypted.Flush();
            }
            var decFileInfo = new FileInfo(tmpDecName);
            fsDecrypted = decFileInfo.Create();
            logger.LogInformation("Decrypting...");
            decrypt(tmpEncName, tmpDecName);
            using (fsDecrypted = new FileStream(tmpDecName, FileMode.Create, FileAccess.Read))
            {
                decBytePhoto = new byte[decFileInfo.Length];
                fsDecrypted.Read(decBytePhoto, 0, decBytePhoto.Length);
                fsDecrypted.Flush();
            }
            logger.LogInformation("Cleaning up after decryption...");
            try
            {
                File.Delete(tmpEncName);
                File.Delete(tmpDecName);
            }
            catch (IOException e)
            {
                logger.LogCritical("Deleting files tmp.enc and tmp.dec failed: {0}. Stack trace:\n{1}", e.Message, e.StackTrace);
            }
            return decBytePhoto;
        }
    }
}
