using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Decryptor
{
    public class FakeImageDecryptor : IImageDecryptor
    {
        public byte[] Decrypt(byte[] encryptedImage)
        {
            return encryptedImage;
        }
    }
}
