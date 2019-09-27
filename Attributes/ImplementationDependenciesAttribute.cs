using System;

namespace Attributes
{

    [AttributeUsage(AttributeTargets.Constructor, Inherited = true, AllowMultiple = false)]
    public class ImplementationDependenciesAttribute : Attribute
    {
        public ImplementationDependenciesAttribute(params Type[] implementationTypes)
        {
            ImplementationTypes = implementationTypes;
        }

        public Type[] ImplementationTypes { get; private set; }
    }
}
