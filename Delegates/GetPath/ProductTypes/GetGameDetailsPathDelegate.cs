using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetGameDetailsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDirectory.Root.GetDataDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetGameDetailsFilenameDelegate))]
        public GetGameDetailsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getGameDetailsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getGameDetailsFilenameDelegate)
        {
            // ...
        }
    }
}