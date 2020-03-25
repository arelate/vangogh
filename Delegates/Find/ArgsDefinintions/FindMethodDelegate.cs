using Attributes;

using Interfaces.Delegates.Find;


using Models.ArgsDefinitions;

namespace Delegates.Find.ArgsDefinitions
{
    public class FindMethodDelegate: FindDelegate<Method>
    {
        [Dependencies(
            "Delegates.Find.ArgsDefinitions.FindAllMethodDelegate,Delegates")]
        public FindMethodDelegate(
            IFindAllDelegate<Method> findAllMethodDelegate):
            base(findAllMethodDelegate)
            {
                // ...
            }
    }
}