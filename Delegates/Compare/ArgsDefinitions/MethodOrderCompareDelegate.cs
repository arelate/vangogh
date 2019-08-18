using System;
using System.Collections.Generic;

using Interfaces.Controllers.Collection;

using Models.ArgsDefinitions;

namespace Delegates.Compare.ArgsDefinitions
{

    public class MethodOrderCompareDelegate : IComparer<string>
    {
        private Method[] methods;
        private ICollectionController collectionController;
        public MethodOrderCompareDelegate(
            Method[] methods,
            ICollectionController collectionController)
        {
            this.methods = methods;
            this.collectionController = collectionController;
        }

        public int Compare(string x, string y)
        {
            var cx = collectionController.Find(methods, c => c.Title == x);
            var cy = collectionController.Find(methods, c => c.Title == y);

            if (cx == null || cy == null)
                throw new ArgumentNullException();

            return cx.Order - cy.Order;
        }
    }
}
