namespace Delegates.Conversions.Strings
{
    public class ConvertStringToReplaceMarkersWithEmptyStringDelegate: ConvertStringToReplaceMarkersWithStringDelegate
    {
        public ConvertStringToReplaceMarkersWithEmptyStringDelegate()
        {
            replaceWith = string.Empty;
        }
    }
}