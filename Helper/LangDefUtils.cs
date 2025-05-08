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
        public void AddKeywordToLangDef(string filePath, string newKeyword,string tokenKey)
        {
            string content = File.ReadAllText(filePath);

            

            
            string pattern = $@"(<RegexPatternGroup[^>]*TokenKey=""{Regex.Escape(tokenKey)}""[^>]*Pattern="")(.*?)("")";
            string pattern9999 = @"(<RegexPatternGroup[^>]*TokenKey=""[tokenKeyHere]""[^>]*Pattern="")(.*?)("")";

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
        
    }
}
