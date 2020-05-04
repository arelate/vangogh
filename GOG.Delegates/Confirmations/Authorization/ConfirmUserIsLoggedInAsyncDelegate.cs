using System.Threading.Tasks;
using Attributes;
using Delegates.Activities;
using Delegates.Data.Network;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.Uris;

namespace GOG.Delegates.Confirmations.Authorization
{
    public class ConfirmUserIsLoggedInAsyncDelegate : IConfirmAsyncDelegate<string>
    {
        private readonly IGetDataAsyncDelegate<string, string> getUriDataAsyncDelegate;
        private readonly IConvertDelegate<string, UserData> convertJSONToUserDataDelegate;

        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GetUriDataAsyncDelegate),
            typeof(ConvertJSONToUserDataDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public ConfirmUserIsLoggedInAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getUriDataAsyncDelegate,
            IConvertDelegate<string, UserData> convertJSONToUserDataDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.convertJSONToUserDataDelegate = convertJSONToUserDataDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }
        
        public async Task<bool> ConfirmAsync(string data)
        {
            startDelegate.Start("Get userData.json");

            var userDataString = await getUriDataAsyncDelegate.GetDataAsync(
                Uris.Endpoints.Authentication.UserData);

            if (string.IsNullOrEmpty(userDataString)) return false;

            var userData = convertJSONToUserDataDelegate.Convert(userDataString);

            completeDelegate.Complete();

            return userData.IsLoggedIn;
        }
    }
}