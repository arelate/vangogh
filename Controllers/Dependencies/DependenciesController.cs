using System;
using System.Reflection;
using System.Collections.Generic;

using Interfaces.Controllers.Dependencies;

namespace Controllers.Dependencies
{
    public class DependenciesController : IDependenciesController
    {
        private readonly Dictionary<Type, object> instancesCache;
        private readonly Dictionary<Type, List<Type>> typeDependencies;

        public DependenciesController()
        {
            instancesCache = new Dictionary<Type, object>();
            typeDependencies = new Dictionary<Type, List<Type>>();
        }

        public void AddDependencies<TargetType>(params Type[] dependencies) where TargetType : class
        {
            var targetType = typeof(TargetType);
            if (!typeDependencies.ContainsKey(targetType))
                typeDependencies.Add(targetType, new List<Type>());
            typeDependencies[targetType].AddRange(dependencies);
        }

        public object GetInstance(Type type)
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
                Type[] implementationTypeDependencies = new Type[0];

                if (typeDependencies.ContainsKey(type))
                    implementationTypeDependencies = typeDependencies[type].ToArray();

                instatiationConstructor = type.GetConstructor(implementationTypeDependencies);

                if (instatiationConstructor == null)
                    throw new ArgumentException(
                        $@"Type {type.Name} cannot be instantitated as it does not
                        have constructor with specified dependencies");

                object[] implementationInstanceDependencies = 
                    new object[implementationTypeDependencies.Length];
                for (var ii=0; ii<implementationTypeDependencies.Length; ii++) {
                    implementationInstanceDependencies[ii] = 
                        GetInstance(implementationTypeDependencies[ii]);
                }

                instancesCache[type] = instatiationConstructor.Invoke(implementationInstanceDependencies);
            }

            return instancesCache[type];
        }
    }
}