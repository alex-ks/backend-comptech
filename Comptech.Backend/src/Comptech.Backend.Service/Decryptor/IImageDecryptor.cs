
namespace Comptech.Backend.Service.Decryptor
{
    public interface IImageDecryptor
    {
        byte[] Decrypt(byte[] encryptedImage);
    }
}
