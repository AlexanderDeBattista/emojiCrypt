using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace emojiCrypt.App_Code.Models
{
    public abstract class ISymmetricCipherData<T, K>
    {

        public ISymmetricCipher<K, Task<K>> CipherMode;

        public abstract Task<T> GetCiphertext();
        public abstract Task<T> GetPlaintext();

        public abstract void SetCiphertext(T ciphertext);
        public abstract void SetPlaintext(T plaintext);



    }

    public abstract class CipherDataString : ISymmetricCipherData<string, string>
    {
        public CipherDataString(ISymmetricCipher<string, Task<string>> Cipher)
        {
            this.CipherMode = Cipher;
        }
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
            this.Ciphertext ??= await base.CipherMode.Encrypt(this.Plaintext, this.Key, this.Iv);

            return this.Ciphertext;
        }

        public async override Task<string> GetPlaintext()
        {
            if (this.Ciphertext == null)
            {
                throw new InvalidOperationException("Must define ciphertext before you can decrypt");
            }
            this.Plaintext ??= await base.CipherMode.Decrypt(this.Ciphertext, this.Key, this.Iv);
            return this.Plaintext;
        }

        public override void SetCiphertext(string Ciphertext)
        {
            this.Ciphertext = Ciphertext;
        }

        public override void SetPlaintext(string plaintext)
        {
            throw new NotImplementedException();
        }
    }

    public class CipherDataAesEmoji : ISymmetricCipherData<EmojiArrayBase64String, string>
    {
        private string? Plaintext { get; set; }
        private string? Ciphertext { get; set; }

        private string Key { get; set; }
        private string Iv { get; set; }
        public CipherDataAesEmoji(string Key, string Iv)
        {
            this.CipherMode = new AesEcbString();
            this.Key = Key;
            this.Iv = Iv;
        }

        public async override Task<EmojiArrayBase64String> GetCiphertext()
        {
            if (this.Plaintext == null)
            {
                throw new InvalidOperationException("Must define plaintext before you can decrypt");
            }
            this.Ciphertext = await base.CipherMode.Encrypt(this.Plaintext, this.Key, this.Iv);
   
            return new EmojiArrayBase64String(this.Ciphertext);
        }

        public async override Task<EmojiArrayBase64String> GetPlaintext()
        {
            if (this.Ciphertext == null)
            {
                throw new InvalidOperationException("Must define ciphertext before you can decrypt");
            }
            this.Plaintext = await base.CipherMode.Decrypt(this.Ciphertext, this.Key, this.Iv);
            return new EmojiArrayBase64String(this.Plaintext);
        }

        public override void SetCiphertext(EmojiArrayBase64String Ciphertext)
        {
            this.Ciphertext = Ciphertext.GetEncodedString();
        }

        public override void SetPlaintext(EmojiArrayBase64String Plaintext)
        {
            this.Plaintext = Plaintext.GetEncodedString();
        }
    }
}
