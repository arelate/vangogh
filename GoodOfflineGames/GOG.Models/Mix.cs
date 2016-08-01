using GOG.Interfaces.Models;

namespace GOG.Models
{
    class Mix: IMix
    {
        public string Slug { get; set; }
        public int Votes { get; set; }
        public string Title { get; set; }
        public IUser User { get; set; }
    }
}
