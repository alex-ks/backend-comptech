using Microsoft.Extensions.Logging;
using System.Text;
using Xunit;

namespace Comptech.Backend.Service.Test
{
    public class ImageDecryptorTest
    {
        /// <summary>
        /// Quite synthetic and stupid test for Decrypt
        /// </summary>
        [Fact]
        public void TestDecrypt()
        {
            ImageDecryptor decryptor = new ImageDecryptor(new LoggerFactory());
            string encryptedPhoto = "some photo";
            var decryptedPhoto = decryptor.Decrypt(Encoding.Unicode.GetBytes(encryptedPhoto));
            Assert.NotEqual(encryptedPhoto, Encoding.Unicode.GetString(decryptedPhoto));
        }
    }
}
