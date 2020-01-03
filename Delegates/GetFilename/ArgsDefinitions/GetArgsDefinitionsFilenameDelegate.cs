using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ArgsDefinitions
{
    public class GetArgsDefinitionsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetArgsDefinitionsFilenameDelegate(IGetFilenameDelegate getFilenameDelegate):
            base(Filenames.ArgsDefinitions, getFilenameDelegate)
            {
                // ...
            }
    }
}