using Interfaces.Controllers.Collection;
using Interfaces.Models.Dependencies;

using Delegates.Confirm;

using Attributes;

using Models.Separators;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmGameDetailsContainsLanguageDelegate : ConfirmStringMatchesAllDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Controllers.Collection.CollectionController,Controllers")]
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