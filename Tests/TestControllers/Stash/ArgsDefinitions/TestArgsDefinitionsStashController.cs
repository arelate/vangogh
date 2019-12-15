using System.Threading.Tasks;

using Interfaces.Controllers.Stash;

using Interfaces.Status;

using Models.ArgsDefinitions;

namespace TestControllers.Stash.ArgsDefinitions
{
    public class TestArgsDefinitionsStashController : IStashController<ArgsDefinition>
    {
        public bool DataAvailable
        {
            get
            {
                return true;
            }
        }

        public async Task<ArgsDefinition> GetDataAsync(IStatus status)
        {
            return await Task.Run(() =>
            {
                return TestModels.ArgsDefinitions.ReferenceArgsDefinition.ArgsDefinition;
            });
        }

        public Task LoadAsync(IStatus status)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAsync(IStatus status)
        {
            throw new System.NotImplementedException();
        }
    }
}