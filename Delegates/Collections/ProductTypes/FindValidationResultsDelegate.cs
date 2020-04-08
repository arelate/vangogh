using Attributes;
using Interfaces.Delegates.Collections;
using Models.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindValidationResultsDelegate : FindDelegate<ValidationResults>
    {
        [Dependencies(
            "Delegates.Collections.ProductTypes.FindAllValidationResultsDelegate,Delegates")]
        public FindValidationResultsDelegate(
            IFindAllDelegate<ValidationResults> findAllValidationResultsDelegate) :
            base(findAllValidationResultsDelegate)
        {
            // ...
        }
    }
}