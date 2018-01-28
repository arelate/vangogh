<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;

using Interfaces.Models.Entities;

namespace Models.Filenames
{
    public static class Filenames
    {
        public static IDictionary<Entity, string> Base = new Dictionary<Entity, string>{
            { Entity.Index, "index" },
        };
=======
﻿namespace Models.Filenames
{
    public static class Filenames
    {
        public static string Hashes = "hashes";
        public static string AppTemplates = "app";
        public static string ReportTemplates = "report";
        public static string Cookies = "cookies";
        public static string Settings = "settings";
        public static string Index = "index";
        public static string Wishlisted = "wishlisted";
        public static string Updated = "updated";
>>>>>>> 031a7711257f6ae79974a8ec1a38663d42935d5c
    }
}
