using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ArgsDefinitions
{
    public class GetArgsDefinitionsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(Delegates.GetFilename.GetJsonFilenameDelegate))]
        public GetArgsDefinitionsFilenameDelegate(IGetFilenameDelegate getJsonFilenameDelegate) :
            base(Filenames.ArgsDefinitions, getJsonFilenameDelegate)
        {
            // ...
        }
    }
}