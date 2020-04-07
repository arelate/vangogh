using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;


using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class WishlistedRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.WishlistedRecordsDataController,Controllers")]
        public WishlistedRecordsIndexController(
            IDataController<ProductRecords> wishlistedRecordsController) :
            base(
                wishlistedRecordsController)
        {
            // ...
        }
    }
}