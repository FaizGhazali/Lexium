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

            //var pattern = @"(<ExplicitPatterns><!\[CDATA\[\s*)(.*?)(\s*\]\]></ExplicitPatterns>)";
            //var pattern = @"<ExplicitPatterns><!\[CDATA\[(.*?)\]\]></ExplicitPatterns>";
            //var pattern = @"<ExplicitPatterns>\s*<!\[CDATA\[(.*?)\]\]>\s*</ExplicitPatterns>";
            
           string pattern = @"(<ExplicitPatterns>\s*<!\[CDATA\[)(.*?)(\]\]>\s*</ExplicitPatterns>)"; //need to master regex


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
