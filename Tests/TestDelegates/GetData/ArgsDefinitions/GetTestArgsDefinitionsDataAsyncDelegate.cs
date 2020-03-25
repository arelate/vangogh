using System.Threading.Tasks;

using Interfaces.Delegates.GetData;

using Models.ArgsDefinitions;

namespace TestControllers.Stash.ArgsDefinitions
{
    public class GetTestArgsDefinitionsDataAsyncDelegate : IGetDataAsyncDelegate<ArgsDefinition>
    {
        public async Task<ArgsDefinition> GetDataAsync(string uri = null)
        {
            return await Task.Run(() =>
            {
                return TestModels.ArgsDefinitions.ReferenceArgsDefinition.ArgsDefinition;
            });
        }
    }
}