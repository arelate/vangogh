using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;


using Models.ArgsDefinitions;

namespace Delegates.GetData.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataAsyncDelegate : GetJSONDataAsyncDelegate<ArgsDefinition>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ArgsDefinitions.ConvertJSONToArgsDefinitionDelegate,Delegates")]
        public GetArgsDefinitionsDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate, 
            IConvertDelegate<string, ArgsDefinition> convertJSONToTypeDelegate) : 
            base(getStringDataAsyncDelegate, convertJSONToTypeDelegate)
        {
            // ...
        }
    }
}