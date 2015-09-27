using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;
using GOG.Interfaces;

namespace GOG.Controllers
{
    public abstract class ProductCoreController<Type> :
        CollectionController<Type>,
        IProductCoreController<Type>
        where Type : ProductCore
    {
        private ICollectionContainer<Product> productsCollectionContainer;
        private IStringGetController stringGetDelegate;

        public ProductCoreController(IList<Type> collection): base (collection)
        {
            // ...
        }

        public ProductCoreController(
            IList<Type> collection,
            ICollectionContainer<Product> productsCollectionContainer,
            IStringGetController stringGetDelegate) : this(collection)
        {
            this.productsCollectionContainer = productsCollectionContainer;
            this.stringGetDelegate = stringGetDelegate;
        }

        public Type Find(long id)
        {
            return Find(p => p.Id == id);
        }

        protected virtual string GetRequestTemplate()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetRequestDetails(Product product)
        {
            throw new NotImplementedException();
        }

        protected virtual bool Skip(Product product)
        {
            throw new NotImplementedException();
        }

        protected virtual Type Deserialize(string data)
        {
            throw new NotImplementedException();
        }

        public async Task Update(IConsoleController consoleController)
        {
            if (productsCollectionContainer == null ||
                stringGetDelegate == null)
            {
                throw new InvalidOperationException(@"To use Update method 
                    you need to construct ProductCoreController instance 
                    with ICollectionContainer, IStringGetController and 
                    override protected methods in child class.");
            }

            foreach (var product in productsCollectionContainer.Collection)
            {
                if (Skip(product)) continue;

                string requestUri = string.Format(
                    GetRequestTemplate(),
                    GetRequestDetails(product));

                var dataString = await stringGetDelegate.GetString(requestUri);

                var data = Deserialize(dataString);

                data.Id = product.Id;

                Add(data);

                if (consoleController != null) consoleController.Write(".");
            }
        }
    }
}
