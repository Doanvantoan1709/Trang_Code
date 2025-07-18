using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonHelper.Validate
{
    public class ValidateInput
    {
        /// Kiểm tra input mã độc XSS
        public static bool CheckXSSInput(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            string pattern = @"(<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>)|((javascript:|data:|vbscript:)[^\s]*)|(<[^>]+on\w+\s*=)|(&#\d+;)|(&#x[0-9a-fA-F]+;)|(<iframe\b)|(<object\b)|(<embed\b)|(<link\b)";

            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }


    }
}
