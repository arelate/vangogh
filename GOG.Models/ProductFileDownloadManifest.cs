using Interfaces.Models.Properties;

namespace GOG.Models
{
    public class ProductFileDownloadManifest :
        ITitleProperty
    {
        public long Id { get; }
        public string Title { get; }
        public string Source { get; }
        public string Destination { get; }

        public ProductFileDownloadManifest(
            long id,
            string title,
            string source,
            string destination)
        {
            Id = id;
            Title = title;
            Source = source;
            Destination = destination;
        }
    }
}