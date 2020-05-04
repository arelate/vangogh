using Interfaces.Delegates.Conversions;
using Models.ProductTypes;

namespace Delegates.Conversions
{
    public abstract class ConvertProductCoreToIndexDelegate<Type> : IConvertDelegate<Type, long>
        where Type : ProductCore
    {
        public long Convert(Type data)
        {
            return data.Id;
        }
    }
}