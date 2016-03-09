using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG.SharedModels
{
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
            { "redirect_uri", Urls.LoginRedirect},
            { "response_type", "code" },
            { "layout", "default" }
        };

        public static Dictionary<string, string> LoginAuthenticate = new Dictionary<string, string>()
        {
            { "login[username]", "" },
            { "login[password]", "" },
            { "login[login]", "" },
            { "login[_token]", "" },
        };

        public static Dictionary<string, string> TwoStepAuthenticate = new Dictionary<string, string>()
        {
            { "second_step_authentication[token][letter_1]", "" },
            { "second_step_authentication[token][letter_2]", "" },
            { "second_step_authentication[token][letter_3]", "" },
            { "second_step_authentication[token][letter_4]", "" },
            { "second_step_authentication[send]", "" },
            { "second_step_authentication[_token]", "" }
        };
    }
}
