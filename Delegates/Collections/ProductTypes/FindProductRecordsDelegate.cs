using Attributes;
using Interfaces.Delegates.Collections;
using Models.ProductTypes;
using Delegates.Collections.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindProductRecordsDelegate : FindDelegate<ProductRecords>
    {
        [Dependencies(
            typeof(FindAllProductRecordsDelegate))]
        public FindProductRecordsDelegate(
            IFindAllDelegate<ProductRecords> findAllProductRecordsDelegate) :
            base(findAllProductRecordsDelegate)
        {
            // ...
        }
    }
}