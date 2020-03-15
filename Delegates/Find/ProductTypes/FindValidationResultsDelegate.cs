using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Find.ProductTypes
{
    public class FindValidationResultsDelegate : FindDelegate<ValidationResults>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.ProductTypes.FindAllValidationResultsDelegate,Delegates")]
        public FindValidationResultsDelegate(
            IFindAllDelegate<ValidationResults> findAllValidationResultsDelegate) :
            base(findAllValidationResultsDelegate)
        {
            // ...
        }
    }
}