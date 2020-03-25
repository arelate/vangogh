using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetUpdatedFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetUpdatedFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Updated, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}