using System.Threading.Tasks;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Activities;
using Models.Uris;
using GOG.Models;
using Attributes;

namespace GOG.Delegates.Confirm.Authorization
{
    public class ConfirmUserIsLoggedInAsyncDelegate : IConfirmAsyncDelegate<string>
    {
        private readonly IGetDataAsyncDelegate<string, string> getUriDataAsyncDelegate;
        private readonly IConvertDelegate<string, UserData> convertJSONToUserDataDelegate;

        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "Delegates.Data.Network.GetUriDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToUserDataDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
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