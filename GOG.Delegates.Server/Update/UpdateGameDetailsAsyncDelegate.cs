using Attributes;
using Delegates.Activities;
using Delegates.Values.Uri.ProductTypes;
using GOG.Delegates.Conversions.UpdateIdentity;
using GOG.Delegates.Data.Models.ProductTypes;
using GOG.Delegates.Itemizations.MasterDetail;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Values;

namespace GOG.Delegates.Server.Update
{
    [RespondsToRequests(Method = "update", Collection = "gamedetails")]
    public class UpdateGameDetailsAsyncDelegate :
        UpdateMasterDetailsAsyncDelegate<GameDetails, AccountProduct>
    {
        [Dependencies(
            typeof(GetGameDetailsUpdateUriDelegate),
            typeof(ConvertAccountProductToGameDetailsUpdateIdentityDelegate),
            typeof(UpdateGameDetailsAsyncDelegate),
            typeof(CommitGameDetailsAsyncDelegate),
            typeof(ItemizeAllAccountProductsGameDetailsGapsAsyncDelegate),
            typeof(GetDeserializedGameDetailsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public UpdateGameDetailsAsyncDelegate(
            IGetValueDelegate<string, string> getGameDetailsUpdateUriDelegate,
            IConvertDelegate<AccountProduct, string> convertAccountProductToGameDetailsUpdateIdentityDelegate,
            IUpdateAsyncDelegate<GameDetails> updateGameDetailsAsyncDelegate,
            ICommitAsyncDelegate commitGameDetailsAsyncDelegate,
            IItemizeAllAsyncDelegate<AccountProduct> itemizeAllAccountProductsGameDetailsGapsAsyncDelegate,
            IGetDataAsyncDelegate<GameDetails, string> getDeserializedGameDetailsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getGameDetailsUpdateUriDelegate,
                convertAccountProductToGameDetailsUpdateIdentityDelegate,
                updateGameDetailsAsyncDelegate,
                commitGameDetailsAsyncDelegate,
                itemizeAllAccountProductsGameDetailsGapsAsyncDelegate,
                getDeserializedGameDetailsAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}