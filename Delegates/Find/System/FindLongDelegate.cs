using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.System
{
    public class FindLongDelegate: FindDelegate<long>
    {
        [Dependencies(
            "Delegates.Find.System.FindAllLongDelegate,Delegates")]
        public FindLongDelegate(
            IFindAllDelegate<long> findAllLongDelegate):
            base(findAllLongDelegate)
            {
                // ...
            }
    }
}