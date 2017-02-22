using Interfaces.Containment;
using Interfaces.Collection;

namespace Controllers.Containment
{
    public class StringContainmentController : IContainmentController<string>
    {
        private string[] entries;
        private ICollectionController collectionController;

        public StringContainmentController(
            ICollectionController collectionController,
            params string[] entries)
        {
            this.collectionController = collectionController;
            this.entries = entries;
        }

        public bool Contained(string data)
        {
            var contained = true;

            collectionController.Map(entries, entry => contained &= data.Contains(entry));

            return contained;
        }
    }
}
