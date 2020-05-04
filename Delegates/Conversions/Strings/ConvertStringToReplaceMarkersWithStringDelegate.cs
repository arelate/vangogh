using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Strings
{
    public abstract class ConvertStringToReplaceMarkersWithStringDelegate: IConvertDelegate<(string Source, string[] Markers), string>
    {
        protected string replaceWith;
        
        public string Convert((string Source, string[] Markers) stringMarkers)
        {
            var input = stringMarkers.Source;
            foreach (var match in stringMarkers.Markers)
                input = input.Replace(match, replaceWith);

            return input;
        }
    }
}