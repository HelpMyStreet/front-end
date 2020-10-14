using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HelpMyStreetFE.Helpers
{
    public static class StringHelpers
    {
        private static readonly Regex LineBreakRegex = new Regex(@"(\n|\r)+");

        public static string ToHtmlSafeStringWithLineBreaks(this string inputString)
        {
            return LineBreakRegex.Replace(HttpUtility.HtmlEncode(inputString), "<br />");
        }

        public static string ToTitleCase(this string inputString)
        {
            return string.Join(' ', inputString.Split(' ').Where(word => word.Length > 0).Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }
    }
}
