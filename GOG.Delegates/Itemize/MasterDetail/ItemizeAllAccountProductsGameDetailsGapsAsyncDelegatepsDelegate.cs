using Interfaces.Controllers.Data;
using Interfaces.Models.Dependencies;

using Delegates.Itemize.MasterDetail;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Itemize.MasterDetail
{
    public class ItemizeAllAccountProductsGameDetailsGapsAsyncDelegatepsDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<AccountProduct,GameDetails>
    {
        [Dependencies(
            DependencyContext.Default,
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "GOG.Controllers.Data.ProductTypes.GameDetailsDataController,GOG.Controllers")]
        public ItemizeAllAccountProductsGameDetailsGapsAsyncDelegatepsDelegate(
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<GameDetails> gameDetailsDataController):
            base(
                accountProductsDataController,
                gameDetailsDataController)
                {
                    // ...
                }
    }
}
