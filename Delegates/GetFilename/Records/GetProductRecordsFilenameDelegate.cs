using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetProductRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetProductRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.Products, getBinFilenameDelegate)
        {
            // ...
        }
    }
}