using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    public static class Urls
    {
        public const string HttpProtocol = "http:";
        public const string HttpsProtocol = "https:";
        // Roots
        private const string Root = HttpsProtocol + "//www.gog.com";
        private const string Login = HttpsProtocol + "//login.gog.com";
        private const string Auth = HttpsProtocol + "//auth.gog.com";
        //public const string ImagesTemplate = HttpProtocol + "//images-{0}.gog.com";
        // Authentication flow
        public const string Authenticate = Auth + "/auth";
        public const string LoginCheck = Login + "/login_check";
        public const string LoginRedirect = Root + "/on_login_success";
        // Account
        private const string Account = Root + "/account";
        public const string AccountGetFilteredProducts = Account + "/getFilteredProducts";
        public const string AccountGameDetailsTemplate = Account + "/gameDetails/{0}.json";
        // Games
        public const string GamesAjaxFiltered = Root + "/games/ajax/filtered";
        // Game page
        public const string GamePage = Root + "/game/";
        // Wishlist
        public const string AccountWishlist = Account + "/wishlist";
    }

    public static class SortBy
    {
        public const string DatePurchased = "date_purchased";
        public const string Title = "title";
        public const string Bestselling = "bestselling";
        public const string DateAdded = "date";
        public const string Rating = "rating";
    }

    public static class QueryParameters
    {
        public static Dictionary<string, string> AccountGetFilteredProducts = new Dictionary<string, string>()
        {
            { "hasHiddenProducts", "false" },
            { "hiddenFlag", "0" },
            { "isUpdated", "0" },
            { "mediaType", "1" },
            { "page", "1" },
            { "sortBy", SortBy.DatePurchased }
        };

        public static Dictionary<string, string> GamesAjaxFiltered = new Dictionary<string, string>()
        {
            { "mediaType", "game" },
            { "page", "1" },
            { "sort", SortBy.DateAdded }
        };

        public static Dictionary<string, string> Authenticate = new Dictionary<string, string>()
        {
            { "client_id", "46755278331571209" },
            { "layout", "default" },
            { "redirect_uri", Urls.LoginRedirect},
            { "response_type", "code" }
        };

        public static Dictionary<string, string> LoginAuthenticate = new Dictionary<string, string>()
        {
            { "login[username]", "" },
            { "login[password]", "" },
            { "login[_token]", "" }
        };

    }
}
