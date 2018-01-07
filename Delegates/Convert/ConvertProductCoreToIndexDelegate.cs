using Interfaces.Delegates.Convert;

using Models.ProductCore;

namespace Delegates.Convert
{
    public class ConvertProductCoreToIndexDelegate<Type> : IConvertDelegate<Type, long> where Type: ProductCore
    {
        public long Convert(Type data)
        {
            return data.Id;
        }
    }
}
