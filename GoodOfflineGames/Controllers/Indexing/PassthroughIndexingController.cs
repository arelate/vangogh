using Interfaces.Indexing;

namespace Controllers.Indexing
{
    public class PassthroughIndexingController : IIndexingController
    {
        public long GetIndex<Type>(Type data)
        {
            return long.Parse(data.ToString());
        }
    }
}
