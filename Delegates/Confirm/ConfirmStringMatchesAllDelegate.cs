using Interfaces.Delegates.Confirm;

using Interfaces.Controllers.Collection;

namespace Delegates.Confirm
{
    public class ConfirmStringMatchesAllDelegate : IConfirmDelegate<string>
    {
        private string[] matches;
        private ICollectionController collectionController;

        public ConfirmStringMatchesAllDelegate(
            ICollectionController collectionController,
            params string[] matches)
        {
            this.collectionController = collectionController;
            this.matches = matches;
        }

        public bool Confirm(string entry)
        {
            var matchesAll = true;

            collectionController.Map(matches, match => matchesAll &= entry.Contains(match));

            return matchesAll;
        }
    }
}
