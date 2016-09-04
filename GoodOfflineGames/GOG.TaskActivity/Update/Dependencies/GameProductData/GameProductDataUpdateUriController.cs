using Interfaces.UpdateDependencies;

namespace GOG.TaskActivities.Update.Dependencies.GameProductData
{
    public class GameProductDataUpdateUriController : IUpdateUriController
    {
        public string GetUpdateUri<T>(T product)
        {
            return (product as Models.Product).Url;
        }
    }
}
