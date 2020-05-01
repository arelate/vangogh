using System;
using Interfaces.Models.Activities;

namespace Models.Activities
{
    public class Activity : IActivity
    {
        public string Title { get; set; }
        public int Progress { get; set; }
        public int Target { get; set; }
        public bool Complete { get; set; }
        public DateTime Started { get; set; }
        public DateTime Completed { get; set; }
    }
}