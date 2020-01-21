using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Interfaces.Controllers.Instances;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Controllers.Instances
{

    public class SingletonInstancesController : IInstancesController
    {
        private readonly Dictionary<Type, object> singletonInstancesContextualCache = new Dictionary<Type, object>();
        private readonly DependencyContext context;

        public SingletonInstancesController(DependencyContext context = DependencyContext.Default)
        {
            this.context = context;
        }

        // TODO: Create delegate? Convert delegate?
        public object GetInstance(Type type)
        {
            if (type == null ||
                type.IsInterface ||
                type.IsAbstract)
                throw new ArgumentException($"Invalid type: {type.Name}: null, an interface or abstract");

            if (!singletonInstancesContextualCache.ContainsKey(type))
            {
                var constructorWithDependencies = GetInstantiationConstructorInfo(type);
                if (constructorWithDependencies == null)
                {
                    constructorWithDependencies = type.GetConstructor(Type.EmptyTypes);

                    if (constructorWithDependencies == null)
                        throw new ArgumentException(
                            $@"Type {type.Name} cannot be instantiated as it does not
                            have constructor with specified dependencies or default constructor");
                }

                var constructorDependencies = GetTypesForConstructor(constructorWithDependencies);

                var instantiatedDependencies = GetInstances(constructorDependencies);

                singletonInstancesContextualCache[type] = constructorWithDependencies.Invoke(instantiatedDependencies);
            }

            return singletonInstancesContextualCache[type];
        }

        // TODO: Same as above
        public object[] GetInstances(Type[] types)
        {
            object[] instances = new object[types.Length];

            for (var ii = 0; ii < types.Length; ii++)
                instances[ii] = GetInstance(types[ii]);

            return instances;
        }

        // TODO: Convert delegate
        public ConstructorInfo GetInstantiationConstructorInfo(Type type)
        {
            foreach (var constructorInfo in type.GetConstructors())
                if (constructorInfo.IsDefined(typeof(DependenciesAttribute)))
                    return constructorInfo;

            return null;
        }

        // TODO: Itemize delegate
        public Type[] GetTypesForConstructor(ConstructorInfo constructorInfo)
        {
            Type[] implementationTypeDependencies = null;

            if (constructorInfo == null)
                return Type.EmptyTypes;

            var dependenciesAttributes = constructorInfo.GetCustomAttributes(
                typeof(DependenciesAttribute));

            if (!dependenciesAttributes.Any())
                return Type.EmptyTypes;

            var resolvedDependencies = new string[(dependenciesAttributes.First() as DependenciesAttribute).Dependencies.Length];
            foreach (var attribute in dependenciesAttributes)
            {
                var dependenciesAttribute = attribute as DependenciesAttribute;
                // Skip dependencies attributes for non-matching contexts
                if (!context.HasFlag(dependenciesAttribute.Context)) continue;
                for (var dd = 0; dd < dependenciesAttribute.Dependencies.Length; dd++)
                {
                    if (string.IsNullOrEmpty(dependenciesAttribute.Dependencies[dd]) &&
                        dependenciesAttribute.Context != DependencyContext.Default) continue;
                    resolvedDependencies[dd] = dependenciesAttribute.Dependencies[dd];
                }
            }

            implementationTypeDependencies = new Type[resolvedDependencies.Length];
            for (var rr = 0; rr < resolvedDependencies.Length; rr++)
            {
                var type = Type.GetType(resolvedDependencies[rr]);
                if (type == null)
                    throw new ArgumentNullException($"Couldn't find the dependency type: {resolvedDependencies[rr]}");
                implementationTypeDependencies[rr] = type;
            }
            return implementationTypeDependencies;
        }
    }
}