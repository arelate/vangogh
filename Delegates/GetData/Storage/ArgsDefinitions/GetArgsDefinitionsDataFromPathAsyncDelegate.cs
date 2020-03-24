using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;
using Interfaces.Models.Dependencies;

using Models.ArgsDefinitions;

namespace Delegates.GetData.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataFromtPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<ArgsDefinition>
    {
        public GetArgsDefinitionsDataFromtPathAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getJSONDataAsyncDelegate, 
            IGetPathDelegate getPathDelegate) : 
            base(getJSONDataAsyncDelegate, getPathDelegate)
        {
            // ...
        }
    }
}