
namespace Comptech.Backend.Service
{
    interface IDecryptor
    {
        byte[] Decrypt(byte[] encryptedPhoto);
    }
}
