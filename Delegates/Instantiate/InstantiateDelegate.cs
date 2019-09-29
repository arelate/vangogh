using System;
using System.Reflection;
using System.Collections.Generic;

using Interfaces.Delegates.Instantiate;

using Attributes;

namespace Delegates.Instantiate
{
    public class InstantiateDelegate : IInstantiateDelegate
    {
        private readonly Dictionary<Type, object> instancesCache;

        private InstantiateDelegate()
        { 
            instancesCache = new Dictionary<Type, object>();
        }

        public object Instantiate(Type type)
        {
            if (type == null)
                throw new ArgumentException($"Cannot instantiate null type");

            if (type.IsInterface ||
                type.IsAbstract)
                throw new ArgumentException($"Type {type.Name} is an interface or abstract class");

            if (!instancesCache.ContainsKey(type))
            {
                ConstructorInfo instatiationConstructor = null;
                object[] implementationDependencies = null;

                foreach (var constructorInfo in type.GetConstructors())
                {
                    var implementationsDependenciesAttribute = Attribute.GetCustomAttribute(
                        constructorInfo,
                        typeof(ImplementationDependenciesAttribute))
                        as ImplementationDependenciesAttribute;

                    if (implementationsDependenciesAttribute != null)
                    {
                        instatiationConstructor = constructorInfo;
                        implementationDependencies = new object[
                            implementationsDependenciesAttribute.ImplementationTypes.Length];

                        for (var ii = 0; ii < implementationDependencies.Length; ii++)
                            implementationDependencies[ii] = Instantiate(
                                implementationsDependenciesAttribute.ImplementationTypes[ii]);
                    }
                }

                if (instatiationConstructor == null)
                {
                    instatiationConstructor = type.GetConstructor(Type.EmptyTypes)!;

                    if (instatiationConstructor == null)
                        throw new ArgumentException(
                            $@"Type {type.Name} cannot be instantitated as it does not
                        have constructor with specified dependencies or without any parameters");
                }

                instancesCache[type] = instatiationConstructor.Invoke(implementationDependencies);
            }

            return instancesCache[type];
        }
    }
}