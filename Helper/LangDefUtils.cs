using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lexium.Helper
{
    public class LangDefUtils
    {
        public void AddKeywordToLangDef(string filePath, string newKeyword)
        {
            string content = File.ReadAllText(filePath);
            
            string pattern99 = @"(<ExplicitPatterns>\s*<!\[CDATA\[)(.*?)(\]\]>\s*</ExplicitPatterns>)";
            string pattern = @"(<RegexPatternGroup[^>]*TokenKey=""CustomDefination1""[^>]*Pattern="")(.*?)("")";

            bool result = HelperFunction.IsMultiWord(newKeyword);

            string inputXml = File.ReadAllText(filePath);


            string newPhrase = HelperFunction.AddSPlus(newKeyword);
            string newPhrase999 = "kamik\\s+suka\\s+makan";


            string updatedXml = Regex.Replace(inputXml, pattern, m =>
            {
                var before = m.Groups[1].Value;
                var existing = m.Groups[2].Value;
                var after = m.Groups[3].Value;

                if (!existing.Contains(newPhrase))
                    existing += " | " + newPhrase;

                return before + existing + after;
            });

            File.WriteAllText(filePath, updatedXml);

            var match = Regex.Match(content, pattern, RegexOptions.Singleline);

        }
        public void AddKeywordToLangDef9999(string filePath, string newKeyword)
        {
            string content = File.ReadAllText(filePath);

            //var pattern = @"(<ExplicitPatterns><!\[CDATA\[\s*)(.*?)(\s*\]\]></ExplicitPatterns>)";
            //var pattern = @"<ExplicitPatterns><!\[CDATA\[(.*?)\]\]></ExplicitPatterns>";
            //var pattern = @"<ExplicitPatterns>\s*<!\[CDATA\[(.*?)\]\]>\s*</ExplicitPatterns>";
            
            string pattern = @"(<ExplicitPatterns>\s*<!\[CDATA\[)(.*?)(\]\]>\s*</ExplicitPatterns>)"; 

            bool result = HelperFunction.IsMultiWord(newKeyword);

            var match = Regex.Match(content, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                var existingKeywords = match.Groups[2].Value.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (!existingKeywords.Contains(newKeyword))
                {
                    string updatedKeywords = string.Join(" ", existingKeywords.Concat(new[] { newKeyword }));
                    content = Regex.Replace(content, pattern, $"$1{updatedKeywords}$3", RegexOptions.Singleline);
                    File.WriteAllText(filePath, content);
                }
            }
        }
    }
}
