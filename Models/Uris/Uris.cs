namespace Models.Uris
{
    public static class Uris
    {
        public static class Protocols
        {
            public const string HttpPrefix = "http:";
            public const string HttpsPrefix = "https:";
            public const string PrefixSeparator = "//";
            public const string Http = HttpPrefix + PrefixSeparator;
            public const string Https = HttpsPrefix + PrefixSeparator;
        }

        public static class Roots
        {
            private const string GOGcom = "gog.com";
            public const string Website = Protocols.Https + "www." + GOGcom;
            public const string Login = Protocols.Https + "login." + GOGcom;
            public const string Auth = Protocols.Https + "auth." + GOGcom;
            public const string Api = Protocols.Https + "api." + GOGcom;
            public const string CDN = Protocols.Https + "cdn." + GOGcom;
        }

        public static class Extensions
        {
            public static class Validation
            {
                public const string ValidationExtension = ".xml";
            }
        }

        public static class Paths
        {
            public static class Authentication
            {
                public const string UserData = Roots.Website + "/userData.json";
                public const string Auth = Roots.Auth + "/auth";
                public const string Login = Roots.Login + "/login";
                public const string Logout = Roots.Login + "/logout";
                public const string LoginCheck = Roots.Login + "/login_check";
                public const string TwoStep = Login + "/two_step";
                public const string OnLoginSuccess =  Roots.Website + "/on_login_success";
            }

            public static class Api
            {
                public const string ProductTemplate = Roots.Api + "/products/{0}";
            }

            public static class Account
            {
                private const string AccountRoot = Roots.Website + "/account";
                public const string GetFilteredProducts = AccountRoot + "/getFilteredProducts";
                public const string GameDetails = AccountRoot + "/gameDetails";
                public const string GameDetailsRequestTemplate = GameDetails + "/{0}.json";
                public const string Wishlist = AccountRoot + "/wishlist";
            }

            public static class Games
            {
                public const string AjaxFiltered = Roots.Website + "/games/ajax/filtered";
            }

            public static class GameProductData
            {
                public const string ProductTemplate = Roots.Website + "{0}";
            }

            public static class Images
            {
                public const string FullUriTemplate = Protocols.HttpPrefix + "{0}.png";
            }

            public static class Screenshots
            {
                public const string FullUriTemplate = Protocols.HttpPrefix + "{0}";
            }

            public static class ProductFiles
            {
                public const string ManualUrlDownlink = Roots.Website + "/downlink";
                public const string ManualUrlRequestTemplate = Roots.Website + "{0}";
                public const string ManualUrlCDNSecure = Roots.CDN + "/secure";
            }
        }
    }
}
