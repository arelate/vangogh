using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.Root;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetValidationResultsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetDataDirectoryDelegate),
            typeof(GetValidationResultsFilenameDelegate))]
        public GetValidationResultsPathDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetValueDelegate<string, string> getValidationResultsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getValidationResultsFilenameDelegate)
        {
            // ...
        }
    }
}