using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.System
{
    public class FindStringDelegate: FindDelegate<string>
    {
        [Dependencies(
            "Delegates.Find.System.FindAllStringDelegate,Delegates")]
        public FindStringDelegate(
            IFindAllDelegate<string> findAllStringDelegate):
            base(findAllStringDelegate)
            {
                // ...
            }
    }
}