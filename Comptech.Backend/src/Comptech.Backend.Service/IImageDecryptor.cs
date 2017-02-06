
namespace Comptech.Backend.Service
{
    interface IImageDecryptor
    {
        byte[] Decrypt(byte[] encryptedPhoto);
    }
}
