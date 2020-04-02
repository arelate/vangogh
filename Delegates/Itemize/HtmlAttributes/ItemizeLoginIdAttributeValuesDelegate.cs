using Models.Patterns;

namespace Delegates.Itemize.HtmlAttributes
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