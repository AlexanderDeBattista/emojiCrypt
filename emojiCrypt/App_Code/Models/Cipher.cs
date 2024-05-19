using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace emojiCrypt.App_Code.Models
{
    public abstract class ICipher
    {
    }

    public abstract class ISymmetricCipher<T, K> : ICipher
    {
        public abstract K Encrypt(T value, T key, T iv);
        public abstract K Decrypt(T value, T key, T iv);
    }


    public class AesEcb : ISymmetricCipher<byte[], Task<byte[]>>
    {
        private Aes AesObject {  get; set; }
        public AesEcb() {
            AesObject = Aes.Create();
        }

        public async override Task<byte[]> Encrypt(byte[] Value, byte[] Key, byte[] Iv)
        {
            Task<byte[]> IvHash = this.HashValToBlocksize(Iv);
            Task<byte[]> KeyHash = this.HashValToBlocksize(Key);

            AesObject.IV = await IvHash;
            AesObject.Key = await KeyHash;
            return await Task.Run(() => AesObject.EncryptEcb(Value, PaddingMode.PKCS7));
        }

        public async override Task<byte[]> Decrypt(byte[] Value, byte[] Key, byte[] Iv)
        {
            Task<byte[]> IvHash = this.HashValToBlocksize(Iv);
            Task<byte[]> KeyHash = this.HashValToBlocksize(Key);

            AesObject.IV = await IvHash;
            AesObject.Key = await KeyHash;
            return await Task.Run(() => AesObject.DecryptEcb(Value, PaddingMode.PKCS7));
        }

        private async Task<byte[]> HashValToBlocksize(byte[] Iv)
        {
            return await Task.Run(() => SHA256.HashData(Iv).Take(AesObject.BlockSize / 8).ToArray());//Update to Keccak once supported
        }

    }
    public class StringToByteCipher: ISymmetricCipher<string, Task<string>> 
    {
        private ISymmetricCipher<byte[], Task<byte[]>> SymmetricCipher { get; set; }
        public StringToByteCipher(ISymmetricCipher<byte[], Task<byte[]>> Cipher)
        {
            SymmetricCipher = Cipher;
        }
        public async override Task<string> Encrypt(string Value, string Key, string Iv) {

            byte[] encVal = await SymmetricCipher.Encrypt(
                Encoding.UTF8.GetBytes(Value),
                Encoding.UTF8.GetBytes(Key),
                Encoding.UTF8.GetBytes(Iv)
            );
            return Convert.ToBase64String(encVal);
        }

        public async override Task<string> Decrypt(string Value, string Key, string Iv)
        {

            byte[] encVal = await SymmetricCipher.Decrypt(
                Convert.FromBase64String(Value),
                Encoding.UTF8.GetBytes(Key),
                Encoding.UTF8.GetBytes(Iv)
            );
            return Encoding.UTF8.GetString(encVal);
        }

    }

    public class AesEcbString : StringToByteCipher
    {
        public AesEcbString():base(new AesEcb())
        {}
    }
}


