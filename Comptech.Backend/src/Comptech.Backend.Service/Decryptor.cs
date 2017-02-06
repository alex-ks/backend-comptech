using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Comptech.Backend.Service
{
    public static class Decryptor
    {
        public static byte[] Decrypt(IConfiguration configuration, byte[] encryptedPhoto)
        {
            var decryptKey = configuration.GetSection("DecryptKey").Value;
            //calling library function here

        }
    }
}
