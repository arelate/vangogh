using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Intersect.System
{
    public class IntersectStringDelegate : IntersectDelegate<string>
    {
        [Dependencies(
            "Delegates.Find.System.FindAllStringDelegate,Delegates",
            "Delegates.Find.System.FindStringDelegate,Delegates")]
        public IntersectStringDelegate(
            IFindAllDelegate<string> findAllStringDelegate, 
            IFindDelegate<string> findStringDelegate) : 
            base(findAllStringDelegate, findStringDelegate)
        {
            // ...
        }
    }
}