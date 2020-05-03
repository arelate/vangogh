using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames.ArgsDefinitions
{
    public class GetArgsDefinitionsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetJsonFilenameDelegate))]
        public GetArgsDefinitionsFilenameDelegate(IGetValueDelegate<string, string> getJsonFilenameDelegate) :
            base(Models.Filenames.Filenames.ArgsDefinitions, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}