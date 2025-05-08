using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lexium.Helper
{
    public class HelperFunction
    {
        public static bool IsMultiWord(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Normalize whitespace and split by space
            var wordCount = input
                .Trim()
                .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            return wordCount > 1;
        }
        public static string AddSPlus(string input)
        {
            // Replace every space with \s+
            return Regex.Replace(input, @"\s+", @"\s+");
        }

        public static string GetTokenKeyFromPhrase(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return "phrase0";

            // Split by whitespace, remove empty entries
            int wordCount = phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;

            return $"phrase{wordCount}";
        }
    }
}
