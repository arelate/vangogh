using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;

namespace Delegates.Convert.Types
{
    public sealed class ConvertConstructorInfoToDependenciesTypesDelegate : IConvertDelegate<ConstructorInfo, Type[]>
    {
        private readonly Dictionary<string, string> contextualTypeReplacementMap;

        public ConvertConstructorInfoToDependenciesTypesDelegate(
            Dictionary<string, string> contextualTypeReplacementMap = null)
        {
            this.contextualTypeReplacementMap = contextualTypeReplacementMap;
        }

        public Type[] Convert(ConstructorInfo constructorInfo)
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
                for (var dd = 0; dd < dependenciesAttribute.Dependencies.Length; dd++)
                {
                    if (string.IsNullOrEmpty(dependenciesAttribute.Dependencies[dd])) continue;
                    resolvedDependencies[dd] = dependenciesAttribute.Dependencies[dd];
                }
            }

            if (contextualTypeReplacementMap != null)
            {
                for (var ii = 0; ii < resolvedDependencies.Length; ii++)
                {
                    var type = resolvedDependencies[ii];
                    if (contextualTypeReplacementMap.ContainsKey(type))
                        resolvedDependencies[ii] = contextualTypeReplacementMap[type];
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