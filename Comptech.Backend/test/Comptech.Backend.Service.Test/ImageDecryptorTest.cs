using Comptech.Backend.Service.Decryptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Test
{
    public class ImageDecryptorTest : IImageDecryptor
    {
        public byte[] Decrypt(byte[] encryptedImage)
        {
            return encryptedImage;
        }
        
    }
}
