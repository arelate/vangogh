using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Intersect.System
{
    public class IntersectStringDelegate : IntersectDelegate<string>
    {
        [Dependencies(
            DependencyContext.Default,
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