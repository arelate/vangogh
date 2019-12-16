using System;
using System.Reflection;
using System.Collections.Generic;

using Interfaces.Controllers.Instances;

using Attributes;

namespace Controllers.Instances
{

    public class SingletonInstancesController : IInstancesController
    {
        private readonly Dictionary<Type, object> singletonInstancesCache = new Dictionary<Type, object>();
        private readonly bool enableTestDependenciesOverrides;

        public SingletonInstancesController(bool enableTestDependenciesOverrides = false)
        {
            this.enableTestDependenciesOverrides = enableTestDependenciesOverrides;
        }

        // TODO: Create delegate? Convert delegate?
        public object GetInstance(Type type)
        {
            if (type == null ||
                type.IsInterface ||
                type.IsAbstract)
                throw new ArgumentException($"Invalid type: {type.Name}: null, an interface or abstract");

            if (!singletonInstancesCache.ContainsKey(type))
            {
                var dependentConstructor = GetInstantiationConstructorInfo(type);

                if (dependentConstructor == null)
                {
                    dependentConstructor = type.GetConstructor(Type.EmptyTypes);

                    if (dependentConstructor == null)
                        throw new ArgumentException(
                            $@"Type {type.Name} cannot be instantiated as it does not
                            have constructor with specified dependencies or default constructor");
                }

                var dependentConstructorDependencies =
                    GetTypesForConstructor(dependentConstructor);

                var instantiatedDependencies =
                    GetInstances(dependentConstructorDependencies);

                singletonInstancesCache[type] = dependentConstructor.Invoke(instantiatedDependencies);
            }

            return singletonInstancesCache[type];
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

        // TODO: Itemize delegate
        public Type[] GetTypesForConstructor(ConstructorInfo constructorInfo)
        {
            Type[] implementationTypeDependencies = null;

            if (constructorInfo == null)
                return Type.EmptyTypes;

            var dependencies = constructorInfo.GetCustomAttribute(
                typeof(DependenciesAttribute))
                as DependenciesAttribute;

            if (dependencies == null)
                return Type.EmptyTypes;

            if (enableTestDependenciesOverrides)
            {
                var testDependenciesOverrides = constructorInfo.GetCustomAttribute(
                    typeof(TestDependenciesOverridesAttribute))
                    as TestDependenciesOverridesAttribute;

                if (testDependenciesOverrides != null)
                {
                    if (testDependenciesOverrides.TestDependenciesOverrides.Length !=
                        dependencies.Dependencies.Length)
                        throw new ArgumentOutOfRangeException(@"Test dependencies overrides should match the number of dependencies. 
                        Use string.Empty to preserve dependency without specifying it.");

                    for (var ii = 0; ii < testDependenciesOverrides.TestDependenciesOverrides.Length; ii++)
                    {
                        if (string.IsNullOrEmpty(testDependenciesOverrides.TestDependenciesOverrides[ii]))
                            continue;

                        dependencies.Dependencies[ii] = testDependenciesOverrides.TestDependenciesOverrides[ii];
                    }
                }
            }

            implementationTypeDependencies = new Type[dependencies.Dependencies.Length];
            for (var ii = 0; ii < implementationTypeDependencies.Length; ii++)
            {
                implementationTypeDependencies[ii] = Type.GetType(dependencies.Dependencies[ii]);
                if (implementationTypeDependencies[ii] == null)
                    throw new ArgumentNullException($"Couldn't find the dependency type: {dependencies.Dependencies[ii]}");
            }
            return implementationTypeDependencies;
        }
    }
}