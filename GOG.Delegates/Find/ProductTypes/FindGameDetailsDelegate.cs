using GOG.Models;
using Attributes;
using Delegates.Collections;
using Interfaces.Delegates.Collections;


namespace GOG.Delegates.Collections.ProductTypes
{
    public class FindGameDetailsDelegate : FindDelegate<GameDetails>
    {
        [Dependencies(
            typeof(GOG.Delegates.Collections.ProductTypes.FindAllGameDetailsDelegate))]
        public FindGameDetailsDelegate(
            IFindAllDelegate<GameDetails> findAllGameDetailsDelegate) :
            base(findAllGameDetailsDelegate)
        {
            // ...
        }
    }
}