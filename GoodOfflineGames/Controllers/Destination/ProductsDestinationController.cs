using System;

using Interfaces.Destination;

namespace Controllers.Destination
{
    public class ProductsDestinationController : IDestinationController
    {
        public string GetDirectory(string source)
        {
            return "_products";
        }

        public string GetFilename(string source)
        {
            throw new NotImplementedException();
        }
    }
}
