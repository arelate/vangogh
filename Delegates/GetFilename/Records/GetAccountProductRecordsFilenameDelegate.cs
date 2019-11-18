using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetAccountProductRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetAccountProductRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.AccountProducts, getBinFilenameDelegate)
        {
            // ...
        }
    }
}