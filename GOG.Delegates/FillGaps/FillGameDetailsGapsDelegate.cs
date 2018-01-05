using GOG.Interfaces.Delegates.FillGaps;

using GOG.Models;

namespace GOG.Delegates.FillGaps
{
    public class FillGameDetailsGapsDelegate : IFillGapsDelegate<GameDetails, AccountProduct>
    {
        public void FillGaps(GameDetails gameDetails, AccountProduct accountProduct)
        {
            // GOG.com quirk
            // GameDetails are requested using xxxxxxxxx.json Uri, 
            // where xxxxxxxxx is Id that comes from AccountProduct.
            // Actual GameDetails payload doesn't contain Id,
            // so this controller "connects" GameDetails to AccountProduct

            gameDetails.Id = accountProduct.Id;
        }
    }
}
