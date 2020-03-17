using System.Threading.Tasks;

using Interfaces.Controllers.Stash;

using Models.ArgsDefinitions;

namespace TestControllers.Stash.ArgsDefinitions
{
    public class TestArgsDefinitionsStashController : IStashController<ArgsDefinition>
    {
        // public bool DataAvailable
        // {
        //     get
        //     {
        //         return true;
        //     }
        // }

        public async Task<ArgsDefinition> GetDataAsync()
        {
            return await Task.Run(() =>
            {
                return TestModels.ArgsDefinitions.ReferenceArgsDefinition.ArgsDefinition;
            });
        }

        // public Task LoadAsync()
        // {
        //     throw new System.NotImplementedException();
        // }

        public Task PostDataAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}