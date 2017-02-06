namespace Comptech.Backend.Service
{
    public interface IImageDecryptor
    {
        byte[] Decrypt(byte[] encryptedPhoto);
    }
}
