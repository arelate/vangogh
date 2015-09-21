using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Model;

namespace GOG.Controllers
{
    public class ProductsController: 
        CollectionController<Product>,
        IProductsController<Product>
    {
        protected IStringGetController stringGetController;
        private IStringifyController serializationController;
        private IConsoleController consoleController;

        public ProductsController(
            IList<Product> products,
            IStringGetController stringGetController,
            IStringifyController serializationController) :
            base(products)
        {
            this.stringGetController = stringGetController;
            this.serializationController = serializationController;
            this.consoleController = consoleController;
        }

        public Product Find(string title)
        {
            return Find(p => p.Title == title);
        }

        public Product Find(long id)
        {
            return Find(p => p.Id == id);
        }

        public IEnumerable<Product> Find(IEnumerable<string> names)
        {
            return Reduce(p => names.Contains(p.Title));
        }

        //public async Task<IList<Product>> Get(
        //    string uri,
        //    IDictionary<string, string> parameters,
        //    IList<long> filter = null,
        //    string message = null)
        //{
        //    consoleController.WriteLine(message);

        //    return await pagedResultController.GetAll(uri, parameters, filter);
        //}
    }
}

