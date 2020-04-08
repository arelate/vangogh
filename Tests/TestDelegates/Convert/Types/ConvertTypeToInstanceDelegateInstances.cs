using System;
using System.Reflection;
using Interfaces.Delegates.Convert;
using Delegates.Convert.Types;

namespace TestDelegates.Convert.Types
{
    public static class ConvertTypeToInstanceDelegateInstances
    {
        private static IConvertDelegate<Type, ConstructorInfo> convertTypeToDependenciesConstructorInfoDelegate =
            new ConvertTypeToDependenciesConstructorInfoDelegate();

        private static IConvertDelegate<ConstructorInfo, Type[]> convertConstructorInfoToDependenciesTypesDelegate =
            new ConvertConstructorInfoToDependenciesTypesDelegate(
                TestModels.Dependencies.TestContextReplacements.Map);

        public static IConvertDelegate<Type, object> Test =
            new ConvertTypeToInstanceDelegate(
                convertTypeToDependenciesConstructorInfoDelegate,
                convertConstructorInfoToDependenciesTypesDelegate);
    }
}