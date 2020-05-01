using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ArgsDefinitions;

namespace Delegates.Data.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataAsyncDelegate : GetJSONDataAsyncDelegate<ArgsDefinition>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.ArgsDefinitions.ConvertJSONToArgsDefinitionDelegate))]
        public GetArgsDefinitionsDataAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
            IConvertDelegate<string, ArgsDefinition> convertJSONToTypeDelegate) :
            base(getStringDataAsyncDelegate, convertJSONToTypeDelegate)
        {
            // ...
        }
    }
}