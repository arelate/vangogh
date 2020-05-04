using System;
using System.Collections.Generic;
using System.Reflection;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Types
{
    public sealed class ConvertTypeToInstanceDelegate : IConvertDelegate<Type, object>
    {
        private readonly Dictionary<Type, object> singletonInstances = new Dictionary<Type, object>();
        private readonly IConvertDelegate<Type, ConstructorInfo> convertTypeToDependenciesConstructorInfoDelegate;
        private readonly IConvertDelegate<ConstructorInfo, Type[]> convertConstructorInfoToDependenciesTypesDelegate;

        public ConvertTypeToInstanceDelegate(
            IConvertDelegate<Type, ConstructorInfo> convertTypeToDependenciesConstructorInfoDelegate,
            IConvertDelegate<ConstructorInfo, Type[]> convertConstructorInfoToDependenciesTypesDelegate)
        {
            this.convertTypeToDependenciesConstructorInfoDelegate = convertTypeToDependenciesConstructorInfoDelegate;
            this.convertConstructorInfoToDependenciesTypesDelegate = convertConstructorInfoToDependenciesTypesDelegate;
        }

        public object Convert(Type type)
        {
            if (type == null ||
                type.IsInterface ||
                type.IsAbstract)
                throw new ArgumentException($"Invalid type: {type.Name}: null, an interface or abstract");

            if (!singletonInstances.ContainsKey(type))
            {
                var constructorWithDependencies = convertTypeToDependenciesConstructorInfoDelegate.Convert(type);
                if (constructorWithDependencies == null)
                {
                    constructorWithDependencies = type.GetConstructor(Type.EmptyTypes);

                    if (constructorWithDependencies == null)
                        throw new ArgumentException(
                            $@"Type {type.Name} cannot be instantiated as it does not
                            have constructor with specified dependencies or default constructor");
                }

                var constructorDependencies = convertConstructorInfoToDependenciesTypesDelegate.Convert(
                    constructorWithDependencies);

                var instantiatedDependencies = new object[constructorDependencies.Length];
                for (var dd = 0; dd < constructorDependencies.Length; dd++)
                    instantiatedDependencies[dd] = Convert(constructorDependencies[dd]);

                singletonInstances[type] = constructorWithDependencies.Invoke(instantiatedDependencies);
            }

            return singletonInstances[type];
        }
    }
}