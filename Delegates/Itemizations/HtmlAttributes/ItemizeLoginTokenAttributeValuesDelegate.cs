using Models.Patterns;

namespace Delegates.Itemizations.HtmlAttributes
{
    public class ItemizeLoginTokenAttributeValuesDelegate : ItemizeAttributeValuesDelegate
    {
        public ItemizeLoginTokenAttributeValuesDelegate() :
            base(AttributeValuesPatterns.LoginToken)
        {
            // ...
        }
    }
}