using Interfaces.Delegates.Convert;

using Delegates.Convert;

using Models.ProductCore;

namespace Creators.Delegates
{
    public static class ConvertDelegateFactory
    {
        public static IConvertDelegate<Type, long> CreateConvertToIndexDelegate<Type>() where Type : ProductCore
        {
            return new ConvertProductCoreToIndexDelegate<Type>();
        }
    }
}
