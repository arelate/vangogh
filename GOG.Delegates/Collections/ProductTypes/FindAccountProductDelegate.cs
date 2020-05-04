using Attributes;
using Delegates.Collections;
using GOG.Models;
using Interfaces.Delegates.Collections;

namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindAccountProductDelegate : FindDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(FindAllAccountProductDelegate))]
        public FindAccountProductDelegate(
            IFindAllDelegate<AccountProduct> findAllAccountProductDelegate) :
            base(findAllAccountProductDelegate)
        {
            // ...
        }
    }
}