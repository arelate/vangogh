using GOG.Models;
using Attributes;
using Delegates.Collections;
using Interfaces.Delegates.Collections;


namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindAccountProductDelegate : FindDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Collections.ProductTypes.FindAllAccountProductDelegate))]
        public FindAccountProductDelegate(
            IFindAllDelegate<AccountProduct> findAllAccountProductDelegate) :
            base(findAllAccountProductDelegate)
        {
            // ...
        }
    }
}