using System.Threading.Tasks;

namespace Interfaces.Products
{
    public interface ILoadProductsDelegate
    {
        Task LoadProducts();
    }

    public interface IGetProductByIdDelegate<Type>
    {
        Task<Type> GetProductById(long id);
    }

    public interface IUpdateProductDelegate<Type>
    {
        Task UpdateProduct(Type product);
    }

    public interface IRemoveProductDelegate<Type>
    {
        Task RemoveProduct(Type product);
    }

    public interface IContainsProductDelegate<Type>
    {
        bool Contains(Type product);
    }

    public interface IProductsController<Type>:
        ILoadProductsDelegate,
        IGetProductByIdDelegate<Type>,
        IUpdateProductDelegate<Type>,
        IRemoveProductDelegate<Type>,
        IContainsProductDelegate<Type>
    {
        // ...
    }
}
