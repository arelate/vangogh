using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Storage
{
    public interface IPullDelegate<Type>
    {
        Task<Type> Pull(string uri);
    }

    public interface IPushDelegate<Type>
    {
        Task Push(string uri, Type data);
    }

    //public interface IPushProductTypeDelegate
    //{
    //    Task Push<Type>(ProductTypes.ProductTypes productType, IList<Type> products);
    //} 

    //public interface IPullProductTypeDelegate
    //{
    //    Task<List<Type>> Pull<Type>(ProductTypes.ProductTypes productType);
    //}

    //public interface IProductTypeStorageController:
    //    IPullProductTypeDelegate,
    //    IPushProductTypeDelegate
    //{
    //    // ...
    //}

    public interface IStorageController<Type>:
        IPullDelegate<Type>,
        IPushDelegate<Type>
    {
        // ...
    }
}
