using System;
using System.Collections.Generic;

using Interfaces.Models.Entities;

namespace Models.Filenames
{
    public static class Filenames
    {
        public static IDictionary<Entity, string> Base = new Dictionary<Entity, string>{
            { Entity.Index, "index" },
        };
    }
}
