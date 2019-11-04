using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

namespace Controllers.Stash.Cookies
{
    public class CookiesStashController: StashController<Dictionary<string, string>>
    {
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