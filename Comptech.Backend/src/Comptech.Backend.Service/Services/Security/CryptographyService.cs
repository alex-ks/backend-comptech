using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Services.Security
{
    public interface ICryptographyService
    {
        byte[] Decrypt(byte[] encryptedImage);
    }

    public class CryptographyService : ICryptographyService
    {
        public byte[] Decrypt(byte[] encryptedImage)
        {
            throw new NotImplementedException();
        }
    }
}
