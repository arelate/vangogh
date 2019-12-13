﻿using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

using Attributes;

using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDeserialized;
using GOG.Interfaces.Delegates.FillGaps;
using GOG.Models;

namespace GOG.Activities.Update.ProductTypes
{
    public class UpdateGameDetailsByAccountProductsActivity :
        UpdateDetailProductsByMasterProductsActivity<GameDetails, AccountProduct>
    {
        [Dependencies(
            "Delegates.GetValue.Uri.ProductTypes.GetGameDetailsUpdateUriDelegate,Delegates",
            "GOG.Delegates.Convert.UpdateIdentity.ConvertAccountProductToGameDetailsUpdateIdentityDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.GameDetailsDataController,GOG.Controllers",
            "GOG.Delegates.Itemize.MasterDetail.ItemizeAllAccountProductsGameDetailsGapsAsyncDelegatepsDelegate,GOG.Delegates",
            "GOG.Delegates.GetDeserialized.ProductTypes.GetDeserializedGameDetailsAsyncDelegate,GOG.Delegates",
            "Controllers.Status.StatusController,Controllers",
            "GOG.Delegates.FillGaps.FillGameDetailsGapsDelegate,GOG.Delegates")]
        public UpdateGameDetailsByAccountProductsActivity(
            IGetValueDelegate<string> getGameDetailsUpdateUriDelegate,
            IConvertDelegate<AccountProduct, string> convertAccountProductToGameDetailsUpdateIdentityDelegate,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAllAsyncDelegate<AccountProduct> itemizeAllAccountProductsGameDetailsGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<GameDetails> getDeserializedGameDetailsAsyncDelegate,
            IStatusController statusController,
            IFillGapsDelegate<GameDetails,AccountProduct> fillGameDetailsGapsDelegate):
            base(
                getGameDetailsUpdateUriDelegate,
                convertAccountProductToGameDetailsUpdateIdentityDelegate,
                gameDetailsDataController,
                itemizeAllAccountProductsGameDetailsGapsAsyncDelegate,
                getDeserializedGameDetailsAsyncDelegate,
                statusController,
                fillGameDetailsGapsDelegate)
                {
                    // ...
                }
    }
}