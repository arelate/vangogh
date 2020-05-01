using System;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class DependenciesAttribute : Attribute
    {
        public Type[] Dependencies { get; }

        public DependenciesAttribute(params Type[] dependencies)
        {
            Dependencies = dependencies;
        }
    }
}