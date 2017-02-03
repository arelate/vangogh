using System.IO;

namespace Controllers.Destination.Directory
{
    public class ApiProductsDirectoryDelegate: DataDirectoryDelegate
    {
        public override string GetDirectory(string source)
        {
            return Path.Combine(
                base.GetDirectory(source),
                "apiProducts");
        }
    }
}
