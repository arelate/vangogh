using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Interfaces.Delegates.GetDeserialized;
using GOG.Interfaces.Delegates.FillGaps;
using GOG.Models;
using Delegates.GetValue.Uri.ProductTypes;
using Delegates.Activities;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method = "update", Collection = "gamedetails")]
    public class RespondToUpdateGameDetailsRequestDelegate :
        RespondToUpdateMasterDetailsRequestDelegate<GameDetails, AccountProduct>
    {
        [Dependencies(
            typeof(GetGameDetailsUpdateUriDelegate),
            typeof(GOG.Delegates.Convert.UpdateIdentity.ConvertAccountProductToGameDetailsUpdateIdentityDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.UpdateGameDetailsAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.CommitGameDetailsAsyncDelegate),
            typeof(GOG.Delegates.Itemize.MasterDetail.ItemizeAllAccountProductsGameDetailsGapsAsyncDelegate),
            typeof(GOG.Delegates.GetDeserialized.ProductTypes.GetDeserializedGameDetailsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate),
            typeof(GOG.Delegates.FillGaps.FillGameDetailsGapsDelegate))]
        public RespondToUpdateGameDetailsRequestDelegate(
            IGetValueDelegate<string> getGameDetailsUpdateUriDelegate,
            IConvertDelegate<AccountProduct, string> convertAccountProductToGameDetailsUpdateIdentityDelegate,
            IUpdateAsyncDelegate<GameDetails> updateGameDetailsAsyncDelegate,
            ICommitAsyncDelegate commitGameDetailsAsyncDelegate,
            IItemizeAllAsyncDelegate<AccountProduct> itemizeAllAccountProductsGameDetailsGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<GameDetails> getDeserializedGameDetailsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate,
            IFillGapsDelegate<GameDetails, AccountProduct> fillGameDetailsGapsDelegate) :
            base(
                getGameDetailsUpdateUriDelegate,
                convertAccountProductToGameDetailsUpdateIdentityDelegate,
                updateGameDetailsAsyncDelegate,
                commitGameDetailsAsyncDelegate,
                itemizeAllAccountProductsGameDetailsGapsAsyncDelegate,
                getDeserializedGameDetailsAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate,
                fillGameDetailsGapsDelegate)
        {
            // ...
        }
    }
}