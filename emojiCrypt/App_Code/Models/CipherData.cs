using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace emojiCrypt.App_Code.Models
{
    public abstract class ISymmetricCipherData<T>
    {
        private T? Plaintext {  get; set; }
        private T? Ciphertext { get; set; }

        private T? Key { get; set; }
        private T? IV { get; set; }
        public ISymmetricCipher<T, Task<T>> CipherMode;

        public abstract Task<T> GetCiphertext();
        public abstract Task<T> GetPlaintext();


    }

    public abstract class CipherDataString : ISymmetricCipherData<string>
    {
        CipherDataString(ISymmetricCipher<string, Task<string>> Cipher)
        {
            this.CipherMode = Cipher;
        }
        public class CipherDataStringAes : CipherDataString
        {
            private string? Plaintext { get; set; }
            private string? Ciphertext { get; set; }

            private string Key { get; set; }
            private string Iv { get; set; }
            CipherDataStringAes(string Key, string Iv) : base(new AesEcbString())
            {
                this.Key = Key;
                this.Iv = Iv;
            }

            public async override Task<string> GetCiphertext()
            {
                if (this.Plaintext == null)
                {
                    throw new InvalidOperationException("Must define plaintext before you can decrypt");
                }
                if (this.Ciphertext == null)
                {
                    return await base.CipherMode.Encrypt(this.Plaintext, this.Key, this.Iv);
                }
                return this.Ciphertext;
            }

            public async override Task<string> GetPlaintext()
            {
                if (this.Ciphertext == null)
                {
                    throw new InvalidOperationException("Must define ciphertext before you can decrypt");
                }
                if (this.Plaintext == null)
                {
                    return await base.CipherMode.Decrypt(this.Ciphertext, this.Key, this.Iv);
                }
                return this.Plaintext;
            }
        }
    }
}
