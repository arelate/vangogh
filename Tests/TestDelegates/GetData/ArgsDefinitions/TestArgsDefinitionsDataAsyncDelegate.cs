using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Models.ArgsDefinitions;

namespace Tests.TestDelegates.Data.ArgsDefinitions
{
    public class TestArgsDefinitionsDataAsyncDelegate : IGetDataAsyncDelegate<ArgsDefinition, string>
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