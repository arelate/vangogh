using System.Collections.Generic;

namespace Models.ArgsTokens
{
    public static class TokensGroups
    {
        // Tokens are divided into three groups:
        // 1) methods: (-method_abbrevation | methods_set | methods...) 
        // 2) optional collections: [collections...] 
        // 3) optional parameter and values: [(--parameter_title | --parameter_abbrevation) parameter_values...]...
        // 
        // Groups contain only certain tokens. 
        // If the current group doesn't contain expected tokens we can saely advance to the next groups

        public static IDictionary<Groups, Tokens[]> ParsingExpectations =
        new Dictionary<Groups, Tokens[]>()
        {
            { 
                Groups.Methods, 
                new Tokens[] { 
                    Tokens.LikelyMethodsAbbrevation,
                    Tokens.MethodsSet, 
                    Tokens.MethodTitle }},
            { 
                Groups.Collections, 
                new Tokens[] { 
                    Tokens.CollectionTitle }},
            { 
                Groups.ParametersValues, 
                new Tokens[] {
                    Tokens.ParameterTitle, 
                    Tokens.LikelyParameterValue }}
        };

        // public static ArgsTokenGroup StartArgsTokenGroup = ArgsTokenGroup.Methods;
    }
}
