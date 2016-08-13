using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Storage
{
    //public interface IPullDelegate
    //{
    //    Task<Type> Pull<Type>(string uri);
    //}

    public interface IPullStringDelegate
    {
        Task<string> Pull(string uri);
    }

    //public interface IPushDelegate
    //{
    //    Task Push<Type>(string uri, Type data);
    //}

    public interface IPushStringDelegate
    {
        Task Push(string uri, string data);
    }

    public interface IPushProductTypeDelegate
    {
        Task Push<Type>(ProductTypes.ProductTypes productType, IList<Type> products);
    } 

    public interface IPullProductTypeDelegate
    {
        Task<IList<Type>> Pull<Type>(ProductTypes.ProductTypes productType);
    }

    public interface IProductTypeStorageController:
        IPullProductTypeDelegate,
        IPushProductTypeDelegate
    {
        // ...
    }

    public interface IStringStorageController:
        IPullStringDelegate,
        IPushStringDelegate
    {
        // ...
    }
}
