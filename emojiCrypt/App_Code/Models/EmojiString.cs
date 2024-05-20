using System.Text;

namespace emojiCrypt.App_Code.Models
{
    public abstract class IEmojiArray<T, K>
    {
        public abstract T MapToEmoji(K value);
        public abstract K MapFromEmoji(T value);

        public abstract K GetEncodedString();
        public abstract T GetEmojiString();


    }

    public class EmojiArrayBase64String : IEmojiArray<string, string>
    {

        private string EmojiString;
        private string OriginalString;
        private static string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        private static List<string> EmojiList = new List<string> {
            "😀", "😁", "😂", "🤣", "😃", "😄", "😅", "😆", "😉", "😊",
            "😋", "😎", "😍", "😘", "😗", "😙", "😚", "☺️", "🙂", "🤗",
            "🤩", "🤔", "🤨", "😐", "😑", "😶", "🙄", "😏", "😣", "😥",
            "😮", "🤐", "😯", "😪", "😫", "😴", "😌", "😛", "😜", "😝",
            "🤤", "😒", "😓", "😔", "😕", "🙃", "🤑", "😲", "☹️", "🙁",
            "😖", "😞", "😟", "😤", "😢", "😭", "😦", "😧", "😨", "😩",
            "🤯", "😬", "😰", "😱", "🥵", "🥶", "😳", "🤪", "😷"
        };

        private static Dictionary<string, string> Base64ToEmojiMapping = Base64Chars.Zip(EmojiList, (ch, emoji) => new { ch, emoji}).ToDictionary(x => x.ch.ToString(), x => x.emoji);
        private static Dictionary<string, string> EmojiToBase64Mapping = Base64Chars.Zip(EmojiList, (ch, emoji) => new { ch, emoji}).ToDictionary(x => x.emoji, x => x.ch.ToString());
        public EmojiArrayBase64String(string Base64String)
        {
            this.EmojiString = "";
            this.OriginalString = Base64String;
            foreach (char ch in Base64String)
            {
                EmojiString += this.MapToEmoji(ch.ToString());
            }
        }

        public override string GetEmojiString()
        {
            return this.EmojiString;
        }

        public override string GetEncodedString()
        {
            return this.OriginalString;
        }



        public override string MapToEmoji(string character)
        {
            if (!Base64ToEmojiMapping.ContainsKey(character)) {
                throw new ArgumentException("Character is not in base64");
            }
            return Base64ToEmojiMapping[character];
        }

        public override string MapFromEmoji(string emoji)
        {
            if (!EmojiToBase64Mapping.ContainsKey(emoji))
            {
                throw new ArgumentException("Not valid emoji!");
            }
            return EmojiToBase64Mapping[emoji];
        }
    }

    public class EmojiArrayString : EmojiArrayBase64String
    {
        public string RawString { get; set; }
        public EmojiArrayString(string input) : base(Convert.ToBase64String(Encoding.ASCII.GetBytes(input)))
        { 
            this.RawString = input;
        }

        public EmojiArrayString(EmojiArrayBase64String emojiArrayString) : base(emojiArrayString.GetEncodedString())
        {
            this.RawString = Encoding.ASCII.GetString(Convert.FromBase64String(emojiArrayString.GetEncodedString()));
        }

    }
}
