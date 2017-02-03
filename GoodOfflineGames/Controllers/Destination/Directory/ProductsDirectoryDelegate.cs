using System.IO;

namespace Controllers.Destination.Directory
{
    public class ProductsDirectoryDelegate: DataDirectoryDelegate
    {
        public override string GetDirectory(string source)
        {
            return Path.Combine(
                base.GetDirectory(source), 
                "products");
        }
    }
}
