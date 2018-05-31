using System.Linq;
using System.Collections.Generic;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;

namespace Delegates.Convert
{
    public class ConvertTreeToEnumerableDelegate<T> : IConvertDelegate<T, IEnumerable<T>> where T: class
    {
        readonly IItemizeDelegate<T, T> itemizeChildrenDelegate;

        public ConvertTreeToEnumerableDelegate(
            IItemizeDelegate<T, T> itemizeChildrenDelegate)
        {
            this.itemizeChildrenDelegate = itemizeChildrenDelegate;
        }

        public IEnumerable<T> Convert(T item)
        {
            if (item == null) yield break;

            var queue = new List<T>();

            queue.Insert(0, item);

            while (queue.Any())
            {
                var current = queue[0];
                queue.RemoveAt(0);

                if (current == null) continue;

                yield return current;

                var children = itemizeChildrenDelegate.Itemize(current);

                if (children == null) continue;

                queue.InsertRange(0, children);
            }
        }
    }
}
