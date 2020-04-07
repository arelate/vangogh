using Attributes;

using Interfaces.Delegates.Collections;

using Models.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindProductRecordsDelegate : FindDelegate<ProductRecords>
    {
        [Dependencies(
            "Delegates.Collections.ProductTypes.FindAllProductRecordsDelegate,Delegates")]
        public FindProductRecordsDelegate(
            IFindAllDelegate<ProductRecords> findAllProductRecordsDelegate) :
            base(findAllProductRecordsDelegate)
        {
            // ...
        }
    }
}