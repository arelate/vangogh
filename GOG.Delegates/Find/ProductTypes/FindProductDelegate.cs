using GOG.Models;
using Attributes;
using Delegates.Collections;
using Interfaces.Delegates.Collections;


namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindProductDelegate : FindDelegate<Product>
    {
        [Dependencies(
            typeof(GOG.Delegates.Collections.ProductTypes.FindAllProductDelegate))]
        public FindProductDelegate(
            IFindAllDelegate<Product> findAllProductDelegate) :
            base(findAllProductDelegate)
        {
            // ...
        }
    }
}