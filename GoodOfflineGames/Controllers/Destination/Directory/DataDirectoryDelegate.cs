using Interfaces.Destination;

namespace Controllers.Destination.Directory
{
    public class DataDirectoryDelegate : IGetDirectoryDelegate
    {
        public virtual string GetDirectory(string source) { return "data"; }
    }
}
