using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiscordCompagnon
{
    internal interface ITextChanger
    {
        public ImageSource Icon { get; }
        string Name { get; }

        string ChangeText(string text);
    }

    internal class TextModifierViewModel : ReactiveObject
    {
        public TextModifierViewModel()
        {
            InputText = "";
            PreviewText = "";
            this.WhenAnyValue(o => o.LastModifier, o => o.InputText)
                .Do(((ITextChanger? modifier, string text) change) =>
                {
                    PreviewText = change.modifier?.ChangeText(change.text) ?? "";
                })
                .Subscribe();

            TextChangers = new ITextChanger[]
            {
                new TitleModifier(),
                new CapitalizeWordModifier(),
                new FullLowerModifier(),
                new FullUpperModifier(),
                new SpongebobModifier(),
                new SmallModifier(),
                new WideModifier(),
                new ScriptModifier(),
                new FancyModifier(),
                new CircleModifier(),
                new EmojiModifier(),
                new CursedModifier(),
            };
        }

        /// <summary>
        /// Text written by the user
        /// </summary>
        [Reactive]
        public string InputText { get; set; }

        /// <summary>
        /// Text viewed when passing the mouse over a button
        /// </summary>
        [Reactive]
        public string PreviewText { get; private set; }

        public ITextChanger[] TextChangers { get; }

        [Reactive]
        private ITextChanger? LastModifier { get; set; }

        public void ImportText()
        {
            if (Clipboard.ContainsText())
                InputText = Clipboard.GetText(TextDataFormat.UnicodeText);
        }

        public void Preview(ITextChanger? textChanger)
        {
            LastModifier = textChanger;
        }

        private static ImageSource FindImage(string resourceName) => new BitmapImage(new Uri(@"pack://application:,,," + resourceName));

        private class CapitalizeWordModifier : ITextChanger
        {
            public ImageSource Icon => FindImage("/capitalizeWords.png");

            public string Name => "Capitalize each word";

            public string ChangeText(string text)
            {
                if (text.Length == 0)
                    return "";
                var sb = new StringBuilder();
                foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.Append(word.Take(1).Select(character => char.ToUpper(character)).ToArray());
                    sb.Append(word.Skip(1).Select(character => char.ToLower(character)).ToArray());
                    sb.Append(' ');
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
        }

        private abstract class CharMapper : ITextChanger
        {
            protected CharMapper(string name, string[] mapSet, string resourceName)
            {
                Name = name;
                MapSet = mapSet;
                Icon = FindImage(resourceName);
            }

            public ImageSource Icon { get; }
            public string[] MapSet { get; }
            public string Name { get; }
            protected static string SourceSet { get; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            public virtual string ChangeText(string text)
            {
                var sb = new StringBuilder();
                foreach (var character in text)
                {
                    var index = SourceSet.IndexOf(character);
                    if (index != -1)
                        sb.Append(MapSet[index]);
                    else
                        sb.Append(character);
                }
                return sb.ToString();
            }
        }

        private class CircleModifier : CharMapper
        {
            public CircleModifier() : base("Circles", new[] { "🅐", "🅑", "🅒", "🅓", "🅔", "🅕", "🅖", "🅗", "🅘", "🅙", "🅚", "🅛", "🅜", "🅝", "🅞", "🅟", "🅠", "🅡", "🅢", "🅣", "🅤", "🅥", "🅦", "🅧", "🅨", "🅩", "🅐", "🅑", "🅒", "🅓", "🅔", "🅕", "🅖", "🅗", "🅘", "🅙", "🅚", "🅛", "🅜", "🅝", "🅞", "🅟", "🅠", "🅡", "🅢", "🅣", "🅤", "🅥", "🅦", "🅧", "🅨", "🅩" }, "/circles.png")
            {
            }
        }

        private class CursedModifier : ITextChanger
        {
            public CursedModifier()
            {
                Diacritics = new[]
                {
                    "◌҃",
                    "◌̑",
                    "◌̱",
                    "◌̷",
                    "◌̄",
                    "◌̊",
                    "◌̧ ̧",
                    "◌̏",
                    "◌̵",
                    "◌̃",
                    "◌͝",
                    "◌͞",
                    "◌͠",
                }.Select(diacritic =>
                {
                    var bytes = Encoding.Unicode.GetBytes(diacritic);
                    return Encoding.Unicode.GetString(bytes.Skip(2).ToArray());
                }).ToArray();
                Random = new Random();
            }

            public ImageSource Icon => FindImage("/cursedText.png");
            public string Name => "Cursed";
            private string[] Diacritics { get; }
            private Random Random { get; }

            public string ChangeText(string text)
            {
                var sb = new StringBuilder();
                foreach (var character in text)
                {
                    sb.Append(character);
                    for (int i = 0; i < 5; ++i)
                    {
                        sb.Append(Diacritics[Random.Next(Diacritics.Length)]);
                    }
                }
                return sb.ToString();
            }
        }

        private class EmojiModifier : CharMapper
        {
            public EmojiModifier() : base("Emojis", new[] { "🇦", "​​​​​🇧​​", "​​​🇨​​​​​", "🇩", "​​​​​🇪​​​​​", "🇫​​​​​", "🇬​​​​​", "🇭​​​​​", "🇮​​​​​", "🇯", "​​​​​🇰", "​​​​​🇱​​​​​", "🇲", "​​​​​🇳​​​​​", "🇴", "​​​​​🇵", "​​​​​🇶​​​​​", "🇷", "​​​​​🇸", "​​​​​🇹", "​​​​​🇺", "​​​​​🇻", "​​​​​🇼", "​​​​​🇽", "​​​​​🇾", "​​​​​🇿", "🇦​​​​​", "​​​​​🇧", "​​​​​🇨", "​​​​​🇩", "​​​​​🇪", "​​​​​🇫", "​​​​​🇬", "​​​​​🇭", "​​​​​🇮", "​​​​​🇯", "​​​​​🇰", "​​​​​🇱", "​​​​​🇲", "​​​​​🇳", "​​​​​🇴", "​​​​​🇵", "​​​​​🇶", "​​​​​🇷", "​​​​​🇸", "​​​​​🇹", "​​​​​🇺", "​​​​​🇻", "​​​​​🇼", "​​​​​🇽", "​​​​​🇾", "​​​​​🇿" }, "/emojiText.png")
            {
            }
        }

        private class FancyModifier : CharMapper
        {
            public FancyModifier() : base("Fancy text", new[] { "𝕒", "𝕓", "𝕔", "𝕕", "𝕖", "𝕗", "𝕘", "𝕙", "𝕚", "𝕛", "𝕜", "𝕝", "𝕞", "𝕟", "𝕠", "𝕡", "𝕢", "𝕣", "𝕤", "𝕥", "𝕦", "𝕧", "𝕨", "𝕩", "𝕪", "𝕫", "𝔸", "𝔹", "ℂ", "𝔻", "𝔼", "𝔽", "𝔾", "ℍ", "𝕀", "𝕁", "𝕂", "𝕃", "𝕄", "ℕ", "𝕆", "ℙ", "ℚ", "ℝ", "𝕊", "𝕋", "𝕌", "𝕍", "𝕎", "𝕏", "𝕐", "ℤ" }, "/fancyText.png")
            {
            }
        }

        private class FullLowerModifier : ITextChanger
        {
            public ImageSource Icon => FindImage("/fullLow.png");

            public string Name => "Full lowercase";

            public string ChangeText(string text)
            {
                return new string(text.Select(character => char.ToLower(character)).ToArray());
            }
        }

        private class FullUpperModifier : ITextChanger
        {
            public ImageSource Icon => FindImage("/fullUpper.png");

            public string Name => "Full uppercase";

            public string ChangeText(string text)
            {
                return new string(text.Select(character => char.ToUpper(character)).ToArray());
            }
        }

        private class ScriptModifier : CharMapper
        {
            public ScriptModifier() : base("Script", new[] { "𝓪", "𝓫", "𝓬", "𝓭", "𝓮", "𝓯", "𝓰", "𝓱", "𝓲", "𝓳", "𝓴", "𝓵", "𝓶", "𝓷", "𝓸", "𝓹", "𝓺", "𝓻", "𝓼", "𝓽", "𝓾", "𝓿", "𝔀", "𝔁", "𝔂", "𝔃", "𝓐", "𝓑", "𝓒", "𝓓", "𝓔", "𝓕", "𝓖", "𝓗", "𝓘", "𝓙", "𝓚", "𝓛", "𝓜", "𝓝", "𝓞", "𝓟", "𝓠", "𝓡", "𝓢", "𝓣", "𝓤", "𝓥", "𝓦", "𝓧", "𝓨", "𝓩" }, "/scriptText.png")
            {
            }
        }

        private class SmallModifier : CharMapper
        {
            public SmallModifier() : base("Small text", new[] { "ᵃ", "ᵇ", "ᶜ", "ᵈ", "ᵉ", "ᶠ", "ᵍ", "ʰ", "ⁱ", "ʲ", "ᵏ", "ˡ", "ᵐ", "ⁿ", "ᵒ", "ᵖ", "q", "ʳ", "ˢ", "ᵗ", "ᵘ", "ᵛ", "ʷ", "ˣ", "ʸ", "ᶻ", "ᴀ", "ʙ", "ᴄ", "ᴅ", "ᴇ", "ꜰ", "ɢ", "ʜ", "ɪ", "ᴊ", "ᴋ", "ʟ", "ᴍ", "ɴ", "ᴏ", "ᴘ", "q", "ʀ", "s", "ᴛ", "ᴜ", "ᴠ", "ᴡ", "x", "ʏ", "ᴢ" }, "/smallText.png")
            {
            }
        }

        private class SpongebobModifier : ITextChanger
        {
            public ImageSource Icon => FindImage("/spongebob.png");

            public string Name => "Spongebob text";

            public string ChangeText(string text)
            {
                var nextUpper = true;
                return new string(text.Select(character =>
                {
                    if (nextUpper = !nextUpper)
                        return char.ToLower(character);
                    else
                        return char.ToUpper(character);
                }).ToArray());
            }
        }

        private class TitleModifier : ITextChanger
        {
            public ImageSource Icon => FindImage("/titleText.png");

            public string Name => "Title text";

            public string ChangeText(string text)
            {
                var sb = new StringBuilder();
                sb.Append(text.Take(1).Select(character => char.ToUpper(character)).ToArray());
                sb.Append(text.Skip(1).Select(character => char.ToLower(character)).ToArray());
                return sb.ToString();
            }
        }

        private class WideModifier : CharMapper
        {
            public WideModifier() : base("Wide text", new[] { "ａ", "ｂ", "ｃ", "ｄ", "ｅ", "ｆ", "ｇ", "ｈ", "ｉ", "ｊ", "ｋ", "ｌ", "ｍ", "ｎ", "ｏ", "ｐ", "ｑ", "ｒ", "ｓ", "ｔ", "ｕ", "ｖ", "ｗ", "ｘ", "ｙ", "ｚ", "Ａ", "Ｂ", "Ｃ", "Ｄ", "Ｅ", "Ｆ", "Ｇ", "Ｈ", "Ｉ", "Ｊ", "Ｋ", "Ｌ", "Ｍ", "Ｎ", "Ｏ", "Ｐ", "Ｑ", "Ｒ", "Ｓ", "Ｔ", "Ｕ", "Ｖ", "Ｗ", "Ｘ", "Ｙ", "Ｚ" }, "/wideText.png")
            {
            }
        }
    }
}