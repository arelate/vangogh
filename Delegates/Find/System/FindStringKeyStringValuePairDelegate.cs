using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.System
{
    public class FindStringKeyStringValuePairDelegate : FindDelegate<KeyValuePair<string, string>>
    {
        [Dependencies(
            "Delegates.Find.System.FindAllStringKeyStringValuePairDelegate,Delegates")]
        public FindStringKeyStringValuePairDelegate(
            IFindAllDelegate<KeyValuePair<string, string>> findAllStringKeyStringValuePairDelegate) :
            base(findAllStringKeyStringValuePairDelegate)
        {
            // ...
        }
    }
}