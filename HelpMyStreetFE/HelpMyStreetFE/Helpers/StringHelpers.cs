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
        private static readonly Regex LineBreakRegex = new Regex(@"(\n|\r){1,2}");

        public static string ToHtmlSafeStringWithLineBreaks(this string inputString)
        {
            return LineBreakRegex.Replace(HttpUtility.HtmlEncode(inputString), "<br />");
        }
    }
}
