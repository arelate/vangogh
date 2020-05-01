using Models.Patterns;

namespace Delegates.Itemize.HtmlAttributes
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