using System;
using System.Collections.Generic;
using System.Reflection;
using Attributes;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Types
{
    public sealed class ConvertConstructorInfoToDependenciesTypesDelegate : IConvertDelegate<ConstructorInfo, Type[]>
    {
        private readonly Dictionary<Type, Type> contextualTypeReplacements;

        public ConvertConstructorInfoToDependenciesTypesDelegate(
            Dictionary<Type, Type> contextualTypeReplacements = null)
        {
            this.contextualTypeReplacements = contextualTypeReplacements;
        }

        public Type[] Convert(ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
                return Type.EmptyTypes;

            var dependenciesAttribute = constructorInfo.GetCustomAttribute(
                typeof(DependenciesAttribute))
                as DependenciesAttribute;

            if (dependenciesAttribute == null) return Type.EmptyTypes;

            var resolvedDependencies = new Type[dependenciesAttribute.Dependencies.Length];
            dependenciesAttribute.Dependencies.CopyTo(resolvedDependencies, 0);

            if (contextualTypeReplacements == null) return resolvedDependencies;
            
            for (var ii = 0; ii < resolvedDependencies.Length; ii++)
            {
                var type = resolvedDependencies[ii];
                if (contextualTypeReplacements.ContainsKey(type))
                    resolvedDependencies[ii] = contextualTypeReplacements[type];
            }

            return resolvedDependencies;
        }
    }
}