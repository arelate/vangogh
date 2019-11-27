using System;
using System.Reflection;
using System.Collections.Generic;

using Interfaces.Controllers.Dependencies;

using Attributes;

namespace Controllers.Dependencies
{
    public class DependenciesController : IDependenciesController
    {
        private readonly Dictionary<Type, object> instancesCache;

        public DependenciesController()
        {
            instancesCache = new Dictionary<Type, object>();
        }

        public object Instantiate(Type type)
        {
            if (type == null)
                throw new ArgumentException($"Cannot instantiate null type");

            if (type.IsInterface ||
                type.IsAbstract)
                throw new ArgumentException(
                    $"Type {type.Name} is an interface or abstract class");

            if (!instancesCache.ContainsKey(type))
            {
                ConstructorInfo instatiationConstructor = null;
                Type[] implementationTypeDependencies = null;

                foreach (var constructorInfo in type.GetConstructors())
                {
                    if (constructorInfo.CustomAttributes != null)
                    {
                        var declaredDependencies = constructorInfo.GetCustomAttribute(
                            typeof(DependenciesAttribute))
                            as DependenciesAttribute;
                        if (declaredDependencies == null) continue;

                        instatiationConstructor = constructorInfo;
                        implementationTypeDependencies = new Type[declaredDependencies.Dependencies.Length];
                        for (var ii = 0; ii < implementationTypeDependencies.Length; ii++)
                            implementationTypeDependencies[ii] = Type.GetType(declaredDependencies.Dependencies[ii]);
                    }
                }

                if (instatiationConstructor == null &&
                    implementationTypeDependencies == null)
                {
                    implementationTypeDependencies = new Type[0];
                    instatiationConstructor = type.GetConstructor(implementationTypeDependencies);
                }
                // instatiationConstructor = type.GetConstructor(implementationTypeDependencies);

                if (instatiationConstructor == null)
                    throw new ArgumentException(
                        $@"Type {type.Name} cannot be instantitated as it does not
                        have constructor with specified dependencies");

                object[] implementationInstanceDependencies =
                    new object[implementationTypeDependencies.Length];
                for (var ii = 0; ii < implementationTypeDependencies.Length; ii++)
                {
                    implementationInstanceDependencies[ii] =
                        Instantiate(implementationTypeDependencies[ii]);
                }

                instancesCache[type] = instatiationConstructor.Invoke(implementationInstanceDependencies);
            }

            return instancesCache[type];
        }
    }
}