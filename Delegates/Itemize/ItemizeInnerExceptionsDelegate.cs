using System;
using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

namespace Delegates.Itemize
{
    public class ItemizeInnerExceptionsDelegate : IItemizeDelegate<Exception, Exception>
    {
        public IEnumerable<Exception> Itemize(Exception item)
        {
            return new Exception[] { item.InnerException };
        }
    }
}
