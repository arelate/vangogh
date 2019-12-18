using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Attributes;

using Controllers.Stash;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class AccountProductsStashController : StashController<List<AccountProduct>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetAccountProductsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ResponseLogController,Controllers")]
        public AccountProductsStashController(
            IGetPathDelegate getAccountProductsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getAccountProductsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}