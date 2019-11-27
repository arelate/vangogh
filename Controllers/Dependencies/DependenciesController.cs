using System;
using System.Reflection;
using System.Collections.Generic;

using Interfaces.Controllers.Dependencies;

using Attributes;

namespace Controllers.Dependencies
{
    public class DependenciesController : IDependenciesController
    {
        private readonly Dictionary<Type, object> singletonInstancesCache = new Dictionary<Type, object>();

        public object Instantiate(Type type)
        {
            if (type == null ||
                type.IsInterface ||
                type.IsAbstract)
                throw new ArgumentException($"Invalid type: {type.Name}: null, an interface or abstract");

            if (!singletonInstancesCache.ContainsKey(type))
            {
                var dependentConstructor = GetDependentConstructor(type);

                if (dependentConstructor == null)
                {
                    dependentConstructor = type.GetConstructor(Type.EmptyTypes);

                    if (dependentConstructor == null)
                        throw new ArgumentException(
                            $@"Type {type.Name} cannot be instantiated as it does not
                            have constructor with specified dependencies or default constructor");
                }

                var dependentConstructorDependencies =
                    GetDependentConstructorDependencyTypes(dependentConstructor);

                var instantiatedDependencies =
                    Instantiate(dependentConstructorDependencies);

                singletonInstancesCache[type] = dependentConstructor.Invoke(instantiatedDependencies);
            }

            return singletonInstancesCache[type];
        }

        public object[] Instantiate(Type[] types)
        {
            object[] instances = new object[types.Length];

            for (var ii = 0; ii < types.Length; ii++)
                instances[ii] = Instantiate(types[ii]);

            return instances;
        }

        public ConstructorInfo GetDependentConstructor(Type type)
        {
            foreach (var constructorInfo in type.GetConstructors())
                if (constructorInfo.CustomAttributes != null)
                {
                    var declaredDependencies = constructorInfo.GetCustomAttribute(
                        typeof(DependenciesAttribute))
                        as DependenciesAttribute;
                    if (declaredDependencies == null) continue;

                    return constructorInfo;
                }

            return null;
        }

        public Type[] GetDependentConstructorDependencyTypes(ConstructorInfo constructorInfo)
        {
            Type[] implementationTypeDependencies = null;

            var declaredDependencies = constructorInfo.GetCustomAttribute(
                typeof(DependenciesAttribute))
                as DependenciesAttribute;

            if (declaredDependencies == null)
                return Type.EmptyTypes;

            implementationTypeDependencies = new Type[declaredDependencies.Dependencies.Length];
            for (var ii = 0; ii < implementationTypeDependencies.Length; ii++)
                implementationTypeDependencies[ii] = Type.GetType(declaredDependencies.Dependencies[ii]);

            return implementationTypeDependencies;
        }
    }
}