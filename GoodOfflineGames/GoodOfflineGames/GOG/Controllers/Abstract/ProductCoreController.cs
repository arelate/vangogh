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
        private IStringGetController stringGetDelegate;

        public event EventHandler<Type> OnProductUpdated;
        public event BeforeAddingDelegate<Type> OnBeforeAdding;

        public ProductCoreController(IList<Type> collection): base (collection)
        {
            // ...
        }

        public ProductCoreController(IList<Type> collection,
            IStringGetController stringGetDelegate) : this(collection)
        {
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

        protected virtual Type Deserialize(string data)
        {
            throw new NotImplementedException();
        }

        public override void UpdateOrAdd(Type item)
        {
            for (var ii=0; ii<Collection.Count; ii++)
            {
                if (Collection[ii].Id == item.Id)
                {
                    Collection[ii] = item;
                    return;
                }
            }

            Add(item);
        }

        public async Task Update(IList<string> items, IPostUpdateDelegate postUpdateDelegate = null) 
        {
            if (stringGetDelegate == null)
            {
                throw new InvalidOperationException(@"To use Update method 
                    you need to construct IStringGetController and 
                    override protected methods in child class.");
            }

            foreach (var item in items)
            {
                string requestUri = string.Format(
                    GetRequestTemplate(),
                    item);

                var dataString = await stringGetDelegate.GetString(requestUri);

                var data = Deserialize(dataString);

                if (data == null) continue;

                if (OnBeforeAdding != null)
                {
                    OnBeforeAdding(ref data, item);
                }

                UpdateOrAdd(data);

                if (OnProductUpdated != null)
                {
                    OnProductUpdated(this, data);
                }

                if (postUpdateDelegate != null)
                    postUpdateDelegate.PostUpdate();
            }
        }
    }
}
