using Attributes;

using Interfaces.Delegates.Find;


using Models.ArgsDefinitions;

namespace Delegates.Find.ArgsDefinitions
{
    public class FindMethodsSetDelegate: FindDelegate<MethodsSet>
    {
        [Dependencies(
            "Delegates.Find.ArgsDefinitions.FindAllMethodsSetDelegate,Delegates")]
        public FindMethodsSetDelegate(
            IFindAllDelegate<MethodsSet> findAllMethodsSetDelegate):
            base(findAllMethodsSetDelegate)
            {
                // ...
            }
    }
}