using System;

namespace Interfaces.Models.Dependencies
{
    [Flags]
    public enum  DependencyContext
    {
        Default,
        Test
    }
}