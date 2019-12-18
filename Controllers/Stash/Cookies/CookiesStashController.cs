using System.Collections.Generic;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;

using Attributes;

using Models.Dependencies;

namespace Controllers.Stash.Cookies
{
    public class CookiesStashController: StashController<Dictionary<string, string>>
    {
        [Dependencies(
            "Delegates.GetPath.Json.GetCookiesPathDelegate,Delegates",
            Dependencies.JSONSerializedStorageController,
            "Controllers.Logs.ResponseLogController,Controllers")]
        public CookiesStashController(
            IGetPathDelegate getCookiePathDelegate,
            ISerializedStorageController jsonSerializedStorageController,
            IActionLogController actionLogController):
            base(
                getCookiePathDelegate,
                jsonSerializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}