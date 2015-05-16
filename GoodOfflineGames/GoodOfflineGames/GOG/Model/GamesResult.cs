using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG
{
    [DataContract]
    public class GamesResult: PagedProductsResult
    {
        [DataMember(Name = "totalGamesFound")]
        public int TotalGamesFound { get; set; }
        [DataMember(Name = "totalMoviesFound")]
        public int TotalMoviesFound { get; set; }
        [DataMember(Name = "totalResults")]
        public int TotalResults { get; set; }
        [DataMember(Name = "ts")]
        public string Ts { get; set; }

    }
}
