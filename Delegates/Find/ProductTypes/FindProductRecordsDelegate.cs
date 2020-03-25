using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.ProductTypes
{
    public class FindProductRecordsDelegate : FindDelegate<ProductRecords>
    {
        [Dependencies(
            "Delegates.Find.ProductTypes.FindAllProductRecordsDelegate,Delegates")]
        public FindProductRecordsDelegate(
            IFindAllDelegate<ProductRecords> findAllProductRecordsDelegate) :
            base(findAllProductRecordsDelegate)
        {
            // ...
        }
    }
}