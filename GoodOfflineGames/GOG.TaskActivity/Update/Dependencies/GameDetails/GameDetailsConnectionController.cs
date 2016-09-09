using Interfaces.UpdateDependencies;

namespace GOG.TaskActivities.Update.Dependencies.GameDetails
{
    public class GameDetailsConnectionController : IConnectionController
    {
        public void Connect<FromType, ToType>(FromType gameDetails, ToType product)
        {
            (gameDetails as Models.GameDetails).Id = (product as Models.AccountProduct).Id;
        }
    }
}
