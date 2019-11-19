using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Controllers.Stash;

using GOG.Models;

namespace GOG.Controllers.Stash.Data
{
    public class AccountProductsStashController :
        StashController<Dictionary<long, AccountProduct>>
    {
        public AccountProductsStashController(
            IGetPathDelegate getAccountProductsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getAccountProductsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}