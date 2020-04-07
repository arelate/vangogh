using Attributes;
using Interfaces.Models.Properties;

using Interfaces.Delegates.Collections;

namespace Delegates.Collections.Properties
{
    public class FindITitlePropertyDelegate: FindDelegate<ITitleProperty>
    {
        [Dependencies(
            "Delegates.Collections.Properties.FindAllITitlePropertyDelegate,Delegates")]
        public FindITitlePropertyDelegate(
            IFindAllDelegate<ITitleProperty> findAllITitlePropertyDelegate):
            base(findAllITitlePropertyDelegate)
            {
                // ...
            }
    }
}