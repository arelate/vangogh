using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG.SharedModels
{
    public static class Urls
    {
        public const string HttpProtocol = "http:";
        public const string HttpsProtocol = "https:";
        // Roots
        private const string SiteRoot = "//www.gog.com";
        public const string HttpsRoot = HttpsProtocol + SiteRoot;
        public const string HttpRoot = HttpProtocol + SiteRoot;
        private const string LoginHost = HttpsProtocol + "//login.gog.com";
        private const string Auth = HttpsProtocol + "//auth.gog.com";
        //public const string ImagesTemplate = HttpProtocol + "//images-{0}.gog.com";
        // Authentication flow
        public const string Authenticate = Auth + "/auth";
        public const string Login = LoginHost + "/login";
        public const string Logout = LoginHost + "/logout";
        public const string LoginCheck = LoginHost + "/login_check";
        public const string LoginRedirect = HttpsRoot + "/on_login_success";
        // Account
        private const string Account = HttpsRoot + "/account";
        public const string AccountGetFilteredProducts = Account + "/getFilteredProducts";
        public const string AccountGameDetailsTemplate = Account + "/gameDetails/{0}.json";
        // Games
        public const string GamesAjaxFiltered = HttpsRoot + "/games/ajax/filtered";
        // Game product data
        public const string GameProductDataPageTemplate = HttpRoot + "{0}";
        // Wishlist
        public const string Wishlist = Account + "/wishlist";
    }
}
