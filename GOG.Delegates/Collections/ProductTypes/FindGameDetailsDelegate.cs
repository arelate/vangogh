using Attributes;
using Delegates.Collections;
using GOG.Models;
using Interfaces.Delegates.Collections;

namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindGameDetailsDelegate : FindDelegate<GameDetails>
    {
        [Dependencies(
            typeof(FindAllGameDetailsDelegate))]
        public FindGameDetailsDelegate(
            IFindAllDelegate<GameDetails> findAllGameDetailsDelegate) :
            base(findAllGameDetailsDelegate)
        {
            // ...
        }
    }
}