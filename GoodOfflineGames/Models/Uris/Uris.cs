using Interfaces.ProductTypes;

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
            public const string Website = "www." + GOGcom;
            public const string Login = Protocols.Https + "login." + GOGcom;
            public const string Auth = Protocols.Https + "auth." + GOGcom;
            public const string Api = Protocols.Https + "api." + GOGcom;
        }

        public static class Paths
        {
            public static class Authentication
            {
                public const string Auth = Roots.Auth + "/auth";
                public const string Login = Roots.Login + "/login";
                public const string Logout = Roots.Login + "/logout";
                public const string LoginCheck = Roots.Login + "/login_check";
                public const string TwoStep = Login + "/two_step";
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

            public static class GameProductData
            {
                public const string ProductTemplate = Protocols.Http + Roots.Website + "{0}";
            }

            public static class Images
            {
                public const string FullUriTemplate = Protocols.HttpPrefix + "{0}.png";
            }

            public static class Screenshots
            {
                public const string FullUriTemplate = Protocols.HttpPrefix + "{0}";
            }

            public static string GetUpdateUri(ProductTypes productType)
            {
                switch (productType)
                {
                    case ProductTypes.Product:
                        return Games.AjaxFiltered;
                    case ProductTypes.AccountProduct:
                        return Account.GetFilteredProducts;
                    case ProductTypes.Screenshot: // screenshots use the same page as game product data
                    case ProductTypes.GameProductData:
                        return GameProductData.ProductTemplate;
                    case ProductTypes.ApiProduct:
                        return Api.ProductTemplate;
                    case ProductTypes.GameDetails:
                        return Account.GameDetailsTemplate;
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }
    }
}
