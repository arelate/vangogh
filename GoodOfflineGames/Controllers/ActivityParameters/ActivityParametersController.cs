using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Destination.Filename;
using Interfaces.SerializedStorage;
using Interfaces.ActivityParameters;

namespace Controllers.ActivityParameters
{
    public class ActivityParametersController : ILoadDelegate, IActivityParametersProperty
    {
        private IGetFilenameDelegate getFilenameDelegate;
        private ISerializedStorageController serializedStorageController;

        public ActivityParametersController(
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController)
        {
            this.getFilenameDelegate = getFilenameDelegate;
            this.serializedStorageController = serializedStorageController;
        }

        public IActivityParameters[] ActivityParameters { get; private set; }

        public async Task LoadAsync()
        {
            ActivityParameters = await serializedStorageController.DeserializePullAsync<
                Models.ActivityParameters.ActivityParameters[]>(
                getFilenameDelegate.GetFilename());

            // set defaults

            if (ActivityParameters == null)
                ActivityParameters = new Models.ActivityParameters.ActivityParameters[0];
        }
    }
}
