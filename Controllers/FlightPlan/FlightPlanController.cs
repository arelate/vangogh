using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Destination.Filename;
using Interfaces.SerializedStorage;
using Interfaces.FlightPlan;

namespace Controllers.FlightPlan
{
    public class FlightPlanController : ILoadDelegate, IFlightPlanProperty
    {
        private IGetFilenameDelegate getFilenameDelegate;
        private ISerializedStorageController serializedStorageController;

        public FlightPlanController(
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController)
        {
            this.getFilenameDelegate = getFilenameDelegate;
            this.serializedStorageController = serializedStorageController;
        }

        public IFlightPlan[] FlightPlan { get; private set; }

        public async Task LoadAsync()
        {
            FlightPlan = await serializedStorageController.DeserializePullAsync<
                Models.FlightPlan.FlightPlan[]>(
                getFilenameDelegate.GetFilename());

            // set defaults

            if (FlightPlan == null)
                FlightPlan = new Models.FlightPlan.FlightPlan[] { };
        }
    }
}
