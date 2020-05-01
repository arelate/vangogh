using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ArgsDefinitions;

namespace Delegates.Data.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<ArgsDefinition>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ArgsDefinitions.GetArgsDefinitionsDataAsyncDelegate),
            typeof(Delegates.GetPath.ArgsDefinitions.GetArgsDefinitionsPathDelegate))]
        public GetArgsDefinitionsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition, string> getJSONDataAsyncDelegate,
            IGetPathDelegate getPathDelegate) :
            base(getJSONDataAsyncDelegate, getPathDelegate)
        {
            // ...
        }
    }
}