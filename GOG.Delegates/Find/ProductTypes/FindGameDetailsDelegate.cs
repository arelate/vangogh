using GOG.Models;

using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Delegates.Find;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindGameDetailsDelegate : FindDelegate<GameDetails>
    {
        [Dependencies(
            DependencyContext.Default,
            "GOG.Delegates.Find.ProductTypes.FindAllGameDetailsDelegate,GOG.Delegates")]
        public FindGameDetailsDelegate(
            IFindAllDelegate<GameDetails> findAllGameDetailsDelegate) :
            base(findAllGameDetailsDelegate)
        {
            // ...
        }
    }
}