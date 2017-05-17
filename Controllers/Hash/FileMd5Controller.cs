using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Hash;

namespace Controllers.Hash
{
    public class FileMd5Controller : IFileHashController
    {
        public Task<string> GetHashAsync(string uri)
        {
            throw new NotImplementedException();
        }
    }
}
