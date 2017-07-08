using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Activities
{
    public class Substitute
    {
        public string Activity { get; set; }
        public string[] Substitutes { get; set; }

    }

    public static class Dependencies
    {
        public static Substitute[] Substitutes = new Substitute[]
        {
            new Substitute { Activity = "sync", Substitutes = new string[] { } }
        };
    }
}
