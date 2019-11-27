using System;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class DependenciesAttribute: Attribute
    {
        public string[] Dependencies {get; set;}

        public DependenciesAttribute(params string[] dependencies)
        {
            Dependencies = dependencies;
        }

    }
}