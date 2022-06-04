using System.Text.RegularExpressions;

namespace YourGameClient.Extensions
{
    public static class StringExtensions
    {
        static readonly Regex _4 = new(".{4}(?!$)");
        public static string ToHyphenedPer4(this string src) => _4.Replace(src, "$0-");

        public static string ToHyphened(this string src) => src.Length < 6 ? src
            : (src.Length % 4) switch {
                1 => $"{src[..3]}-{src[3..6]}-{src[6..].ToHyphenedPer4()}",
                2 => $"{src[..3]}-{src[3..].ToHyphenedPer4()}",
                _ => src.ToHyphenedPer4()
            };
    }
}
