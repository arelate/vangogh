using Interfaces.Controllers.Collection;

using Delegates.Confirm;

using Attributes;

using Models.Separators;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmGameDetailsContainsLanguageDelegate : ConfirmStringMatchesAllDelegate
    {
        [Dependencies("Controllers.Collection.CollectionController,Controllers")]
        public ConfirmGameDetailsContainsLanguageDelegate(
            ICollectionController collectionController) :
            base(
                collectionController,
                Separators.GameDetailsDownloadsStart,
                Separators.GameDetailsDownloadsEnd)
        {
            // ...
        }
    }
}