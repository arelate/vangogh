using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ArgsDefinitions;

namespace Delegates.Data.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataAsyncDelegate : GetJSONDataAsyncDelegate<ArgsDefinition>
    {
        [Dependencies(
            "Delegates.Data.Storage.GetStringDataAsyncDelegate,Delegates",
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