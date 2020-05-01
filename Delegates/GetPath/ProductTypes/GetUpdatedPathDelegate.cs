using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetUpdatedPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDirectory.Root.GetDataDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetUpdatedFilenameDelegate))]
        public GetUpdatedPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getUpdatedFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getUpdatedFilenameDelegate)
        {
            // ...
        }
    }
}