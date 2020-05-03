using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames.ProductTypes
{
    public class GetValidationResultsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetValidationResultsFilenameDelegate(IGetValueDelegate<string, string> GetBinFilenameDelegate) :
            base(Models.Filenames.Filenames.ValidationResults, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}