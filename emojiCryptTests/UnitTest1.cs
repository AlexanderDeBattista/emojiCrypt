using emojiCrypt.App_Code.Models;

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
}