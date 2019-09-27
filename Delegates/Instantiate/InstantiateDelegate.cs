using System;

using Interfaces.Delegates.Instantiate;

using Attributes;

namespace Delegates.Instantiate
{
    public class InstantiateDelegate : IInstantiateDelegate
    {
        public object Instantiate(Type type)
        {
            ImplementationDependenciesAttribute implementationsDependenciesAttribute = null;

            foreach (var constructorInfo in type.GetConstructors())
            {
                implementationsDependenciesAttribute = Attribute.GetCustomAttribute(
                    constructorInfo,
                    typeof(ImplementationDependenciesAttribute))
                    as ImplementationDependenciesAttribute;

                if (implementationsDependenciesAttribute != null)
                    break;
            }

            if (implementationsDependenciesAttribute == null)
            {
                var defaultConstructor = type.GetConstructor(Type.EmptyTypes)!;
                if (defaultConstructor != null)
                    return defaultConstructor.Invoke(null);

                throw new ArgumentException($"Type {type.Name} doesn't contain an accessible default constructor");
            }
            else
            {
                var implementationTypes = implementationsDependenciesAttribute.ImplementationTypes;
                var constructor = type.GetConstructor(implementationTypes);

                if (constructor == null)
                    throw new ArgumentException($"Type {type.Name} doesn't contain an accessible constructor accepting implementation types: {string.Join(',', implementationsDependenciesAttribute.ImplementationTypes as object[])}");

                object[] implementationDependencies = new object[implementationTypes.Length];

                for (var ii = 0; ii < implementationDependencies.Length; ii++)
                    implementationDependencies[ii] = Instantiate(implementationTypes[ii]);

                return constructor.Invoke(implementationDependencies);
            }
        }
    }
}