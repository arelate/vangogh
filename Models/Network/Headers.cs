namespace Models.Network
{
    public static class Headers
    {
        public const string SetCookie = "Set-Cookie";
        public const string Cookie = "Cookie";
        public const string Accept = "Accept";
        public const string ContentType = "Content-Type";
        public const string UserAgent = "User-Agent";
        public const string Origin = "Origin";
    }

    public static class HeaderDefaultValues
    {
        public const string Accept = "text/html, application/xhtml+xml, image/jxr, */*";
        public const string ContentType = "application/x-www-form-urlencoded";
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.16174";
    }
}
