using Models.Patterns;

namespace Delegates.Itemizations.HtmlAttributes
{
    public class ItemizeLoginIdAttributeValuesDelegate : ItemizeAttributeValuesDelegate
    {
        public ItemizeLoginIdAttributeValuesDelegate() :
            base(AttributeValuesPatterns.LoginId)
        {
            // ...
        }
    }
}