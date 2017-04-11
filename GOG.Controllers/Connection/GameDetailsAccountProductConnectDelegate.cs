using Interfaces.Connection;

using GOG.Models;

namespace GOG.Controllers.Connection
{
    public class GameDetailsAccountProductConnectDelegate : IConnectDelegate<GameDetails, AccountProduct>
    {
        public void Connect(GameDetails gameDetails, AccountProduct accountProduct)
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
