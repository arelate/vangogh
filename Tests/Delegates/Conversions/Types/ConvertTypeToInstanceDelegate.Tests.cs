using System;
using System.Collections.Generic;
using Delegates.Itemizations.Types.Attributes;
using Tests.TestDelegates.Conversions.Types;
using Xunit;

namespace Tests.Delegates.Conversions.Types
{
    public class ConvertTypeToInstanceDelegateTests
    {
        public static IEnumerable<object[]> EnumerateTypesWithDependencies()
        {
            var itemizeTypesWithDependencies =
                (ItemizeAllDependenciesAttributeTypesDelegate)
                DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(
                    typeof(ItemizeAllDependenciesAttributeTypesDelegate));

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
                instances[ii] = DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(types[ii]);
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
                var constructorInfo =
                    DelegatesInstances.TestConvertTypeToDependenciesConstructorInfoDelegate.Convert(type);
                var parameters = constructorInfo.GetParameters();
                var dependenciesTypes =
                    DelegatesInstances.TestConvertConstructorInfoToDependenciesTypesDelegate.Convert(constructorInfo);
                Assert.Equal(dependenciesTypes.Length, parameters.Length);
                for (var ii = 0; ii < parameters.Length; ii++)
                {
                    var instance =
                        DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(dependenciesTypes[ii]);
                    Assert.IsAssignableFrom(parameters[ii].ParameterType, instance);
                }
            }
        }
    }
}