using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

using Attributes;

namespace Controllers.Stash.Cookies
{
    public class CookiesStashController: StashController<Dictionary<string, string>>
    {
        [Dependencies(
            "Delegates.GetPath.Json.GetCookiesPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public CookiesStashController(
            IGetPathDelegate getCookiePathDelegate,
            ISerializedStorageController jsonSerializedStorageController,
            IStatusController statusController):
            base(
                getCookiePathDelegate,
                jsonSerializedStorageController,
                statusController)
        {
            // ...
        }
    }
}