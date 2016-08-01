namespace Models.Uris
{
    public static class Uris
    {
        public static class Protocols
        {
            public const string Http = "http:";
            public const string Https = "https:";
        }

        public static class Roots
        {
            private const string GOG = "gog.com";
            public const string Website = "//www." + GOG;
            public const string Login = Protocols.Https + "//login." + GOG;
            public const string Auth = Protocols.Https + "//auth." + GOG;
            public const string Api = Protocols.Https + "//api." + GOG;
        }

        public static class Paths
        {
            public static class Authentication
            {
                public const string Auth = Roots.Auth + "/auth";
                public const string Login = Roots.Login + "/login";
                public const string Logout = Roots.Login + "/logout";
                public const string LoginCheck = Roots.Login + "/login_check";
                public const string TwoStep = Roots.Login + "/two_step";
                public const string OnLoginSuccess = Protocols.Https + Roots.Website + "/on_login_success";
            }

            public static class Api
            {
                public const string ProductTemplate = Roots.Api + "/products/{0}";
            }

            public static class Account
            {
                private const string AccountRoot = Protocols.Https + Roots.Website + "/account";
                public const string GetFilteredProducts = AccountRoot + "/getFilteredProducts";
                public const string GameDetailsTemplate = AccountRoot + "/gameDetails/{0}.json";
                public const string Wishlist = AccountRoot + "/wishlist";
            }

            public static class Games
            {
                public const string AjaxFiltered = Protocols.Https + Roots.Website + "/games/ajax/filtered";
            }

            public static class ProductData
            {
                public const string ProductTemplate = Protocols.Http + Roots.Website + "{0}";
            }
        }
    }
}
