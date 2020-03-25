using GOG.Models;

using Attributes;

using Interfaces.Delegates.Find;


using Delegates.Find;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindGameDetailsDelegate : FindDelegate<GameDetails>
    {
        [Dependencies(
            "GOG.Delegates.Find.ProductTypes.FindAllGameDetailsDelegate,GOG.Delegates")]
        public FindGameDetailsDelegate(
            IFindAllDelegate<GameDetails> findAllGameDetailsDelegate) :
            base(findAllGameDetailsDelegate)
        {
            // ...
        }
    }
}