using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Find.System
{
    public class FindStringKeyStringValuePairDelegate : FindDelegate<KeyValuePair<string, string>>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.System.FindAllStringKeyStringValuePairDelegate,Delegates")]
        public FindStringKeyStringValuePairDelegate(
            IFindAllDelegate<KeyValuePair<string, string>> findAllStringKeyStringValuePairDelegate) :
            base(findAllStringKeyStringValuePairDelegate)
        {
            // ...
        }
    }
}