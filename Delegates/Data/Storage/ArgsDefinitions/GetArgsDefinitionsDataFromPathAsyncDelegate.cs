using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ArgsDefinitions;

namespace Delegates.Data.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<ArgsDefinition>
    {  
        [Dependencies(
            "Delegates.Data.Storage.ArgsDefinitions.GetArgsDefinitionsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ArgsDefinitions.GetArgsDefinitionsPathDelegate,Delegates")]
        public GetArgsDefinitionsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getJSONDataAsyncDelegate, 
            IGetPathDelegate getPathDelegate) : 
            base(getJSONDataAsyncDelegate, getPathDelegate)
        {
            // ...
        }
    }
}