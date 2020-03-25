using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;


using Models.ArgsDefinitions;

namespace Delegates.GetData.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<ArgsDefinition>
    {  
        [Dependencies(
            "Delegates.GetData.Storage.ArgsDefinitions.GetArgsDefinitionsDataAsyncDelegate,Delegates",
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