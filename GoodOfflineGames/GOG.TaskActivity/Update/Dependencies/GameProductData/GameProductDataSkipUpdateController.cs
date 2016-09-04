using Interfaces.UpdateDependencies;

namespace GOG.TaskActivities.Update.Dependencies.GameProductData
{
    public class GameProductDataSkipUpdateController : ISkipUpdateController
    {
        public bool SkipUpdate<T>(T product)
        {
            return (product as Models.Product).IsComingSoon;
        }
    }
}
