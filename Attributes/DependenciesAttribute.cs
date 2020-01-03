using System;

using Interfaces.Models.Dependencies;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
    public class DependenciesAttribute : Attribute
    {
        // NOTE: Dependency attribute is using string[] intentionally,
        // as it allows any assembly to link to any other assembly without the need
        // to add references (that can produce cross-reference issues).
        // The implicit expectation is that dependencies would be resolved
        // inside the domain that has access to all the dependencies' assemblies
        public string[] Dependencies { get; private set; }
        public DependencyContext Context { get; private set; }

        public DependenciesAttribute(DependencyContext context, params string[] dependencies)
        {
            Context = context;
            Dependencies = dependencies;
        }

    }
}