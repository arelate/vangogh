using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Interfaces.Delegates.FillGaps;
using GOG.Models;
using Delegates.Activities;
using Delegates.Values.Uri.ProductTypes;
using GOG.Delegates.Data.Models.ProductTypes;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method = "update", Collection = "gamedetails")]
    public class RespondToUpdateGameDetailsRequestDelegate :
        RespondToUpdateMasterDetailsRequestDelegate<GameDetails, AccountProduct>
    {
        [Dependencies(
            typeof(GetGameDetailsUpdateUriDelegate),
            typeof(Convert.UpdateIdentity.ConvertAccountProductToGameDetailsUpdateIdentityDelegate),
            typeof(UpdateGameDetailsAsyncDelegate),
            typeof(CommitGameDetailsAsyncDelegate),
            typeof(Itemize.MasterDetail.ItemizeAllAccountProductsGameDetailsGapsAsyncDelegate),
            typeof(GetDeserializedGameDetailsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate),
            typeof(FillGaps.FillGameDetailsGapsDelegate))]
        public RespondToUpdateGameDetailsRequestDelegate(
            IGetValueDelegate<string, string> getGameDetailsUpdateUriDelegate,
            IConvertDelegate<AccountProduct, string> convertAccountProductToGameDetailsUpdateIdentityDelegate,
            IUpdateAsyncDelegate<GameDetails> updateGameDetailsAsyncDelegate,
            ICommitAsyncDelegate commitGameDetailsAsyncDelegate,
            IItemizeAllAsyncDelegate<AccountProduct> itemizeAllAccountProductsGameDetailsGapsAsyncDelegate,
            IGetDataAsyncDelegate<GameDetails, string> getDeserializedGameDetailsAsyncDelegate,
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