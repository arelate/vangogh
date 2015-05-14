using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    public static class Separators
    {
        // query parameters
        public const string QueryString = "?";
        public const string QueryStringParameters = "&";
        // common
        public const string KeyValueSeparator = "=";
        // html helper
        public const string HtmlOpenTag = "<";
        public const string HtmlExplicitCloseTag = "/>";
        public const string HtmlImplicitCloseTag = ">";
        public const char Attributes = ' ';
        public const char AttributeValueQuote = '"';
    }
}
