using System;
using System.Collections.Generic;

namespace Controllers.Tree
{
    public class ExceptionTreeToEnumerableController : TreeToEnumerableController<Exception>
    {
        public override IEnumerable<Exception> GetChildren(Exception item)
        {
            return new Exception[] { item.InnerException };
        }
    }
}
