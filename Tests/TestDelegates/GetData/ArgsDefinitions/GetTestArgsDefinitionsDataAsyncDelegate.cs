using System.Threading.Tasks;

using Interfaces.Delegates.Data;

using Models.ArgsDefinitions;

namespace TestDelegates.Data.ArgsDefinitions
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