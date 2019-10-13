namespace Models.Uris
{
    public static class Uris
    {
        public static class Protocols
        {
            public const string HttpPrefix = "http";
            public const string HttpsPrefix = "https";
            public const string PrefixSeparator = "://";
            public const string Http = HttpPrefix + PrefixSeparator;
            public const string Https = HttpsPrefix + PrefixSeparator;
        }

        public static class Roots
        {
            const string GOGcom = "gog.com";
            public const string Website = Protocols.Https + "www." + GOGcom;
            public const string Login = Protocols.Https + "login." + GOGcom;
            public const string Auth = Protocols.Https + "auth." + GOGcom;
            public const string Api = Protocols.Https + "api." + GOGcom;
            public const string CDN = Protocols.Https + "cdn." + GOGcom;
            public const string GoogleRecaptcha = "https://www.google.com/recaptcha";
        }

        public static class Endpoints
        {
            public static class Authentication
            {
                public static string UserData = Roots.Website + "/userData" + Extensions.Extensions.JSON;
                public static string Auth = Roots.Auth + "/auth";
                public static string Login = Roots.Login + "/login";
                public static string Logout = Roots.Login + "/logout";
                public static string LoginCheck = Roots.Login + "/login_check";
                public static string TwoStep = Login + "/two_step";
                public static string OnLoginSuccess = Roots.Website + "/on_login_success";
            }

            public static class Api
            {
                public static string ProductTemplate = Roots.Api + "/products/{0}";
            }

            public static class Account
            {
                public static string AccountRoot = Roots.Website + "/account";
                public static string GetFilteredProducts = AccountRoot + "/getFilteredProducts";
                public static string GameDetails = AccountRoot + "/gameDetails";
                public static string GameDetailsRequestTemplate = GameDetails + "/{0}" + Extensions.Extensions.JSON;
                public static string Wishlist = AccountRoot + "/wishlist";
            }

            public static class Games
            {
                public static string AjaxFiltered = Roots.Website + "/games/ajax/filtered";
            }

            public static class GameProductData
            {
                public static string ProductTemplate = Roots.Website + "{0}";
            }

            public static class Images
            {
                public static string FullUriTemplate = Protocols.HttpPrefix + "{0}" + Extensions.Extensions.PNG;
            }

            public static class Screenshots
            {
                public static string FullUriTemplate = Protocols.HttpPrefix + "{0}";
            }

            public static class ProductFiles
            {
                public static string ManualUrlDownlink = Roots.Website + "/downlink";
                public static string ManualUrlRequestTemplate = Roots.Website + "{0}";
                public static string ManualUrlCDNSecure = Roots.CDN + "/secure";
            }
        }
    }
}
