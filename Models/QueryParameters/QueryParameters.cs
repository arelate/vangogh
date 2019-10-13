using System.Collections.Generic;

namespace Models.QueryParameters
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
        // AccountGetFilteredProducts
        public static string HasHiddenProducts = "hasHiddenProducts";
        public static string HiddenFlag = "hiddenFlag";
        public static string IsUpdated = "isUpdated";
        public static string MediaType = "mediaType";
        public static string Page = "page";
        public static string SortBy = Sort + "By";
        // GamesAjaxFiltered
        // mediaType
        // page
        public static string Sort = "sort";
        // Authenticate
        public static string ClientId = "client_id";
        public static string RedirectUri = "redirect_uri";
        public static string ResponseType = "response_type";
        public static string Layout = "layout";
        public static string Brand = "brand";
        // LoginAuthenticate
        static string Login = "login";
        public static string LoginUsername = Login + "[username]";
        public static string LoginPassword = Login + "[password]";
        public static string LoginId = Login + "[id]";
        public static string LoginToken = Login + "[_token]";
        // SecondStepAuthentication
        static string SecondStepAuthentication = "second_step_authentication";
        static string SecondStepAuthenticationTokenLetters = SecondStepAuthentication + "[token]";
        public static string SecondStepAuthenticationTokenLetter1 = SecondStepAuthenticationTokenLetters + "[letter_1]";
        public static string SecondStepAuthenticationTokenLetter2 = SecondStepAuthenticationTokenLetters + "[letter_2]";
        public static string SecondStepAuthenticationTokenLetter3 = SecondStepAuthenticationTokenLetters + "[letter_3]";
        public static string SecondStepAuthenticationTokenLetter4 = SecondStepAuthenticationTokenLetters + "[letter_4]";
        //public static string SecondStepAuthenticationSend = SecondStepAuthentication + "[send]";
        public static string SecondStepAuthenticationToken = SecondStepAuthentication + "[_token]";
    }

    public static class QueryParametersCollections
    {
        public static Dictionary<string, string> AccountGetFilteredProducts = new Dictionary<string, string>
        {
            { QueryParameters.HasHiddenProducts, "false" },
            { QueryParameters.HiddenFlag, "0" },
            { QueryParameters.IsUpdated, "0" },
            { QueryParameters.MediaType, "1" },
            { QueryParameters.Page, "1" },
            { QueryParameters.SortBy, SortBy.DatePurchased }
        };

        public static Dictionary<string, string> GamesAjaxFiltered = new Dictionary<string, string>
        {
            { QueryParameters.MediaType, "game" },
            { QueryParameters.Page, "1" },
            { QueryParameters.Sort, SortBy.DateAdded }
        };

        public static Dictionary<string, string> Authenticate = new Dictionary<string, string>
        {
            { QueryParameters.ClientId, "46755278331571209" },
            { QueryParameters.RedirectUri, Uris.Uris.Endpoints.Authentication.OnLoginSuccess},
            { QueryParameters.ResponseType, "code" },
            { QueryParameters.Layout, "default" },
            { QueryParameters.Brand, "gog" }
        };

        public static Dictionary<string, string> LoginAuthenticate = new Dictionary<string, string>
        {
            { QueryParameters.LoginUsername, string.Empty },
            { QueryParameters.LoginPassword, string.Empty },
            { QueryParameters.LoginId, string.Empty },
            { QueryParameters.LoginToken, string.Empty }
        };

        public static Dictionary<string, string> SecondStepAuthentication = new Dictionary<string, string>
        {
            { QueryParameters.SecondStepAuthenticationTokenLetter1, string.Empty },
            { QueryParameters.SecondStepAuthenticationTokenLetter2, string.Empty },
            { QueryParameters.SecondStepAuthenticationTokenLetter3, string.Empty },
            { QueryParameters.SecondStepAuthenticationTokenLetter4, string.Empty },
            //{ QueryParameters.SecondStepAuthenticationSend, string.Empty },
            { QueryParameters.SecondStepAuthenticationToken, string.Empty }
        };
    }
}
