using Attributes;
using Interfaces.Models.Properties;
using Interfaces.Delegates.Collections;

namespace Delegates.Collections.Properties
{
    public class FindITitlePropertyDelegate : FindDelegate<ITitleProperty>
    {
        [Dependencies(
            typeof(FindAllITitlePropertyDelegate))]
        public FindITitlePropertyDelegate(
            IFindAllDelegate<ITitleProperty> findAllITitlePropertyDelegate) :
            base(findAllITitlePropertyDelegate)
        {
            // ...
        }
    }
}