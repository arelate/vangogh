using System.Linq;

using Interfaces.UpdateDependencies;
using Interfaces.Data;

namespace GOG.TaskActivities.Update.Dependencies.GameDetails
{
    public class GameDetailsRequiredUpdatesController : IRequiredUpdatesController
    {
        private IDataController<long> updatedDataController;

        public GameDetailsRequiredUpdatesController(IDataController<long> updatedDataController)
        {
            this.updatedDataController = updatedDataController;
        }

        public long[] GetRequiredUpdates()
        {
            return updatedDataController.EnumerateIds().ToArray();
        }
    }
}
