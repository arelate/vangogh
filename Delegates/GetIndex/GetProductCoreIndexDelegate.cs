using Interfaces.Delegates.GetIndex;

using Models.ProductCore;

namespace Delegates.GetIndex
{
    public class GetProductCoreIndexDelegate<Type> : IGetIndexDelegate<Type> where Type: ProductCore
    {
        public long GetIndex(Type data)
        {
            return data.Id;
        }
    }
}
