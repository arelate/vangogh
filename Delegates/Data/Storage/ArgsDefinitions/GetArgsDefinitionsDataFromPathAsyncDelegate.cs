using Attributes;
using Delegates.Values.Paths.ArgsDefinitions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ArgsDefinitions;

namespace Delegates.Data.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<ArgsDefinition>
    {
        [Dependencies(
            typeof(GetArgsDefinitionsDataAsyncDelegate),
            typeof(GetArgsDefinitionsPathDelegate))]
        public GetArgsDefinitionsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition, string> getJSONDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate) :
            base(getJSONDataAsyncDelegate, getPathDelegate)
        {
            // ...
        }
    }
}