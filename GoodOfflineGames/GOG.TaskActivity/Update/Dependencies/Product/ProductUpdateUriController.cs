using Interfaces.UpdateDependencies;

using Models.ProductCore;

namespace GOG.TaskActivities.Update.Dependencies.Product
{
    public class ProductUpdateUriController : IUpdateUriController
    {
        public string GetUpdateUri<T>(T productCore)
        {
            return (productCore as ProductCore).Id.ToString();
        }
    }
}
