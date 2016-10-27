
using Interfaces.Destination;

namespace Controllers.Destination
{
    public abstract class DataDestinationController : IDestinationController
    {
        public virtual string GetDirectory(string source)
        {
            return "_data";
        }

        public virtual string GetFilename(string source)
        {
            return source + ".js";
        }
    }
}
