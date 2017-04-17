using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Interfaces.Tree;

namespace Controllers.Tree
{
    public abstract class TreeToEnumerableController<T> : ITreeToEnumerableController<T>
    {
        public virtual IEnumerable<T> GetChildren(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ToEnumerable(T item)
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

                var children = GetChildren(current);

                if (children == null) continue;

                queue.InsertRange(0, children);
            }
        }
    }
}
