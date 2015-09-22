using System.Collections.Generic;

using GOG.Model;

namespace GOG.Controllers
{
    public class GameDetailsController: 
        CollectionController<GameDetails>
    {
        public GameDetailsController(IList<GameDetails> gameDetails) : base(gameDetails)
        {
            // ...
        }

        public GameDetails Find(long id)
        {
            return Find(gd => gd.Id == id);
        }
    }
}
