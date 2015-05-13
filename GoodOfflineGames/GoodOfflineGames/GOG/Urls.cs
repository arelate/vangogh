using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    public static class Urls
    {
        private const string DefaultProtocol = "https:";
        private const string Root = DefaultProtocol + "//www.gog.com";
        private const string Account = Root + "/account";
        public const string AccountGetFilteredProducts = Account + "/getFilteredProducts";
        public const string AccountGameDetailsTemplate = Account + "/gameDetails/{0}.json";
        public const string GamesAjaxFiltered = Root + "/games/ajax/filtered";
        public const string GamePage = Root + "/game/";
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
            { "sort", SortBy.Bestselling }
        };
    }
}
