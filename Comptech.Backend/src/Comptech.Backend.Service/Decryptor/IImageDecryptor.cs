using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Decryptor
{
    public interface IImageDecryptor
    {
        byte[] Decrypt(byte[] encryptedImage);
    }
}
