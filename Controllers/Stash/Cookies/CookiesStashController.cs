using System.Collections.Generic;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Dependencies;

namespace Controllers.Stash.Cookies
{
    public class CookiesStashController: StashController<Dictionary<string, string>>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetPath.Json.GetCookiesPathDelegate,Delegates",
            Dependencies.JSONSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
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