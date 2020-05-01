using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetUpdatedFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetUpdatedFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.Updated, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}