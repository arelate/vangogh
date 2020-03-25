using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.ProductTypes
{
    public class FindValidationResultsDelegate : FindDelegate<ValidationResults>
    {
        [Dependencies(
            "Delegates.Find.ProductTypes.FindAllValidationResultsDelegate,Delegates")]
        public FindValidationResultsDelegate(
            IFindAllDelegate<ValidationResults> findAllValidationResultsDelegate) :
            base(findAllValidationResultsDelegate)
        {
            // ...
        }
    }
}