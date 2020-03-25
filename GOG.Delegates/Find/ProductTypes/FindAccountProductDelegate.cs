using GOG.Models;

using Attributes;

using Interfaces.Delegates.Find;


using Delegates.Find;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindAccountProductDelegate : FindDelegate<AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.Find.ProductTypes.FindAllAccountProductDelegate,GOG.Delegates")]
        public FindAccountProductDelegate(
            IFindAllDelegate<AccountProduct> findAllAccountProductDelegate) :
            base(findAllAccountProductDelegate)
        {
            // ...
        }
    }
}