using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ArgsDefinitions
{
    public class GetArgsDefinitionsFilenameDelegate: GetFixedFilenameDelegate
    {
        public GetArgsDefinitionsFilenameDelegate(IGetFilenameDelegate getFilenameDelegate):
            base(Filenames.ArgsDefinitions, getFilenameDelegate)
            {
                // ...
            }
    }
}