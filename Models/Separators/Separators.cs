namespace Models.Separators
{
    public static class Separators
    {
        public static class Common
        {
            public const string Space = " ";
            public const string Equality = "=";
            public const string Dot = ".";
            public const string Comma = ",";
            public const string SemiColon = ";";
            public const string NewLine = "\n";
            public const string Dash = "-";
            public const string Ampersand = "&";
            public const string QuestionMark = "?";
            public const string LeftSquareBracket = "[";
            public const string RightSquareBracket = "]";
            public const string ForwardSlash = "/";
            public const string LeftCurlyBracket = "{";
            public const string RightCurlyBracket = "}";
            public const string MoreThan = ">";
            public const string LessThan = "<";
        }

        // query parameters
        public const string UriPart = Common.ForwardSlash;
        public const string QueryString = Common.QuestionMark;
        public const string QueryStringParameters = Common.Ampersand;
        // key value
        public const string KeyValue = Common.Equality;
        // GameDetails downloads
        public const string GameDetailsDownloadsStart = Common.LeftSquareBracket + Common.LeftSquareBracket;
        public const string GameDetailsDownloadsEnd = Common.RightSquareBracket + Common.RightSquareBracket;
        // template
        public const string TemplatePrefix = Common.LeftCurlyBracket;
        public const string TemplateSuffix = Common.RightCurlyBracket;
        // headers
        public const string Headers = Common.NewLine;
    }
}
