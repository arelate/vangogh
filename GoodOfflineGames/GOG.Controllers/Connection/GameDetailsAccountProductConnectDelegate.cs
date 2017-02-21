using Interfaces.Connection;

using GOG.Models;

namespace GOG.Controllers.Connection
{
    public class GameDetailsAccountProductConnectDelegate : IConnectDelegate<GameDetails, AccountProduct>
    {
        public void Connect(GameDetails gameDetails, AccountProduct accountProduct)
        {
            gameDetails.Id = accountProduct.Id;
        }
    }
}
