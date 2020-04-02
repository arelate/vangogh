using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Interfaces.Delegates.Convert;
using Delegates.Convert.Types;
using Delegates.Itemize.Types.Attributes;

namespace Tests.Delegates.Convert.Types
{
    public class ConvertTypeToInstanceDelegateTests
    {
        private static IConvertDelegate<Type, ConstructorInfo> _convertTypeToDependenciesConstructorInfoDelegate =
            new ConvertTypeToDependenciesConstructorInfoDelegate();

        private static IConvertDelegate<ConstructorInfo, Type[]> _convertConstructorInfoToDependenciesTypesDelegate =
            new ConvertConstructorInfoToDependenciesTypesDelegate(
                TestModels.Dependencies.TestContextReplacements.Map);

        private static IConvertDelegate<Type, object> _convertTypeToInstanceDelegate =
            new ConvertTypeToInstanceDelegate(
                _convertTypeToDependenciesConstructorInfoDelegate,
                _convertConstructorInfoToDependenciesTypesDelegate);

        public static IEnumerable<object[]> EnumerateTypesWithDependencies()
        {
            var itemizeTypesWithDependencies =
                (ItemizeAllDependenciesAttributeTypesDelegate)
                _convertTypeToInstanceDelegate.Convert(typeof(ItemizeAllDependenciesAttributeTypesDelegate));

            if (itemizeTypesWithDependencies != null)
                foreach (var type in itemizeTypesWithDependencies.ItemizeAll())
                    yield return new object[] {type};
        }
        
        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        public void InstancesControllerCanInitializeAllDeclaredDependencies(params Type[] types)
        {
            Assert.NotEmpty(types);
            Assert.NotNull(types);
        
            var instances = new object[types.Length];
            for (var ii = 0; ii < types.Length; ii++)
                instances[ii] = _convertTypeToInstanceDelegate.Convert(types[ii]);
            Assert.NotNull(instances);
        
            foreach (var instance in instances)
                Assert.NotNull(instance);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        public void NumberOfDependenciesMatchesNumberOfConstructorParameters(params Type[] types)
        {
            Assert.NotNull(types);
            Assert.NotEmpty(types);

            foreach (var type in types)
            {
                var constructorInfo = _convertTypeToDependenciesConstructorInfoDelegate.Convert(type);
                var parameters = constructorInfo.GetParameters();
                var dependenciesTypes =
                    _convertConstructorInfoToDependenciesTypesDelegate.Convert(constructorInfo);
                Assert.Equal(dependenciesTypes.Length, parameters.Length);
                for (var ii = 0; ii < parameters.Length; ii++)
                {
                    var instance = _convertTypeToInstanceDelegate.Convert(dependenciesTypes[ii]);
                    Assert.IsAssignableFrom(parameters[ii].ParameterType, instance);
                }
            }
        }
    }
}