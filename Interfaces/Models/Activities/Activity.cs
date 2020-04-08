using System;
using Interfaces.Models.Properties;

namespace Interfaces.Models.Activities
{
    public interface IActivity : ITitleProperty
    {
        int Progress { get; set; }
        int Target { get; set; }
        bool Complete { get; set; }
        DateTime Started { get; set; }
        DateTime Completed { get; set; }
    }
}