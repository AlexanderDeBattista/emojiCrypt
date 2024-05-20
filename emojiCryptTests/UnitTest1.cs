using emojiCrypt.App_Code.Models;
using System.Drawing;
using System.Text;

namespace emojiCryptTests
{
    [TestClass]
    public class CipherAesEcbStringTest
    {
        public async Task EncDecSameResult(int ptxtSize)
        {
            AesEcbString aes = new AesEcbString();
            string ptxt = new string('a', ptxtSize);
            string key = "testKey";
            string iv = "testIv";
            string ctxt = await aes.Encrypt(ptxt, key, iv);

            string newPtxt = await aes.Decrypt(ctxt, key, iv);
            Assert.AreEqual(ptxt, newPtxt);
        }
        [TestMethod]
        public async Task EncDecSameResultSmallPtxt()
        {
            await EncDecSameResult(1);
        }

        [TestMethod]
        public async Task EncDecSameResultLargePtxt()
        {
            await EncDecSameResult(1000);
        }
    }


    public abstract class ICipherDataTest<T,K>
    {
            
    }

    [TestClass]
    public class EmojiEncryptionTest
    {
   
        public async Task CheckSameEmojisAfterEncAndDecB64(int sizeOfString)
        {
            string ptxt = new string('a', sizeOfString);
            string key = "hello";
            string iv = "yo";
            EmojiArrayBase64String EmojiString = new EmojiArrayBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(ptxt)));
            CipherDataAesEmoji aesEmojiData = new CipherDataAesEmoji(key, iv);
            CipherDataAesEmoji aesEmojiDataDecrypt = new CipherDataAesEmoji(key, iv);
            aesEmojiData.SetPlaintext(EmojiString);

            EmojiArrayBase64String ctxt = await aesEmojiData.GetCiphertext();

            aesEmojiDataDecrypt.SetCiphertext(ctxt);

            EmojiArrayBase64String NewPtxt = await aesEmojiDataDecrypt.GetPlaintext();

            Console.WriteLine(ctxt.GetEmojiString());
            Console.WriteLine(ctxt.GetEncodedString());

            Assert.AreEqual(EmojiString.GetEmojiString(), NewPtxt.GetEmojiString());
        }

        [TestMethod]
        public async Task SameAfterEncDecB64Small()
        {
            await CheckSameEmojisAfterEncAndDecB64(10);
        }
        [TestMethod]
        public async Task SameAfterEncDecB64Large()
        {
            await CheckSameEmojisAfterEncAndDecB64(1000);
        }

        public async Task SameAfterEncDecString(string ptxt)
        {
            string key = "hello";
            string iv = "yo";
            EmojiArrayString EmojiString = new EmojiArrayString(ptxt);
            CipherDataAesEmoji aesEmojiData = new CipherDataAesEmoji(key, iv);
            CipherDataAesEmoji aesEmojiDataDecrypt = new CipherDataAesEmoji(key, iv);
            aesEmojiData.SetPlaintext(EmojiString);

            EmojiArrayString ctxt = new EmojiArrayString(await aesEmojiData.GetCiphertext());

            aesEmojiDataDecrypt.SetCiphertext(ctxt);

            EmojiArrayString NewPtxt = new EmojiArrayString(await aesEmojiDataDecrypt.GetPlaintext());

            Console.WriteLine(EmojiString.GetEmojiString());
            Console.WriteLine(NewPtxt.GetEmojiString());

            Console.WriteLine(ctxt.GetEncodedString());

            Console.WriteLine(EmojiString.RawString);
            Console.WriteLine(NewPtxt.RawString);

            Assert.AreEqual(EmojiString.GetEmojiString(), NewPtxt.GetEmojiString());
        }

        [TestMethod]
        public async Task SameAfterEncDecStringSmall()
        {
            await SameAfterEncDecString("a");
        }
        [TestMethod]
        public async Task SameAfterEncDecStringLarge()
        {
            await SameAfterEncDecString("I LOVE EMOJIIIIIIS (Am bigger than one block?)");
        }
    }
}