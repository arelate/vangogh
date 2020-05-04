using Attributes;
using Delegates.Conversions.JSON.ArgsDefinitions;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ArgsDefinitions;

namespace Delegates.Data.Storage.ArgsDefinitions
{
    public class GetArgsDefinitionsDataAsyncDelegate : GetJSONDataAsyncDelegate<ArgsDefinition>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(ConvertJSONToArgsDefinitionDelegate))]
        public GetArgsDefinitionsDataAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
            IConvertDelegate<string, ArgsDefinition> convertJSONToTypeDelegate) :
            base(getStringDataAsyncDelegate, convertJSONToTypeDelegate)
        {
            // ...
        }
    }
}