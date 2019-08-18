namespace Models.Patterns
{
    public static class AttributeValuesPatterns
    {
        public const string LoginUsername = "name=\"login\\[username\\]\" type=\"email\" disabled=\"disabled\" value=\"([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)\"";
        public const string LoginToken = "name=\"login\\[_token\\]\" value=\"[\\w-]{43}\"";
        public const string LoginId = "name=\"login\\[id\\]\" value=\"\\d{17}\"";
        public const string SecondStepAuthenticationToken = "name=\"second_step_authentication\\[_token\\]\" value=\"[\\w-]{43}\"";
    }
}
