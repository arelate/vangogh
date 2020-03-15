using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;
using Interfaces.Models.Properties;

namespace Delegates.Find.Properties
{
    public class FindITitlePropertyDelegate: FindDelegate<ITitleProperty>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.Properties.FindAllITitlePropertyDelegate,Delegates")]
        public FindITitlePropertyDelegate(
            IFindAllDelegate<ITitleProperty> findAllITitlePropertyDelegate):
            base(findAllITitlePropertyDelegate)
            {
                // ...
            }
    }
}