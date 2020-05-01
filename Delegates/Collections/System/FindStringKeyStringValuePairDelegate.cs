using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class FindStringKeyStringValuePairDelegate : FindDelegate<KeyValuePair<string, string>>
    {
        [Dependencies(
            typeof(FindAllStringKeyStringValuePairDelegate))]
        public FindStringKeyStringValuePairDelegate(
            IFindAllDelegate<KeyValuePair<string, string>> findAllStringKeyStringValuePairDelegate) :
            base(findAllStringKeyStringValuePairDelegate)
        {
            // ...
        }
    }
}