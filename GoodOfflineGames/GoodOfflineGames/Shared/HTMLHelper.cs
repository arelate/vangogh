using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    static class HTMLHelper
    {

        private static List<string> SplitStringToInnerHtml(string html)
        {
            if (html == null)
            {
                return null;
            }

            return new List<string>(
                html.Split(
                    new string[3] { Separators.HtmlOpenTag, Separators.HtmlExplicitCloseTag, Separators.HtmlImplicitCloseTag },
                    StringSplitOptions.RemoveEmptyEntries));
        }

        private static List<string> FilterInnerHtmlByValues(IEnumerable<string> innerHtml, List<string> values)
        {
            if (innerHtml == null)
            {
                return null;
            }

            List<string> innerHtmlStrings = new List<string>();

            foreach (string innerHtmlString in innerHtml)
            {
                foreach (string value in values)
                {
                    if (innerHtmlString.Contains(value))
                    {
                        innerHtmlStrings.Add(innerHtmlString);
                    }
                }
            }

            return innerHtmlStrings;
        }

        private static Dictionary<string, string> GetAttributes(List<string> attributesKeyValuePairs)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            foreach (string keyValuePair in attributesKeyValuePairs)
            {

                string[] attributeKeyValue = keyValuePair.Split(
                    new string[1] { Separators.KeyValueSeparator },
                    2, // break only on first instance to avoid breaking on value separators
                    StringSplitOptions.RemoveEmptyEntries);

                if (attributeKeyValue.Length < 2) continue;

                attributes.Add(
                    attributeKeyValue[0], // attribute key
                    TrimAttributeValueQuotes(attributeKeyValue[1]) //attribute value
                    );
            }

            return attributes;
        }

        private static Dictionary<string, string> GetAttributes(string innerHtml)
        {
            List<string> attributeKeyValuePairs = GetAttributesKeyValuePairs(innerHtml);
            return GetAttributes(attributeKeyValuePairs);
        }

        private static List<string> GetAttributesKeyValuePairs(string innerHtml)
        {
            // it's not sufficient to split by space, since attribute values can contain spaces
            // instead we split by spaces outside quoted data

            List<string> keyValuePairs = new List<string>();
            string currentData = string.Empty;
            bool insideQuotedData = false;

            foreach (char innerHtmlChar in innerHtml)
            {
                if (innerHtmlChar == Separators.AttributeValueQuote)
                {
                    insideQuotedData = !insideQuotedData;
                }
                else if (innerHtmlChar == Separators.Attributes)
                {
                    if (insideQuotedData)
                    {
                        currentData += innerHtmlChar;
                    }
                    else
                    {
                        keyValuePairs.Add(currentData);
                        currentData = string.Empty;
                    }
                }
                else
                {
                    currentData += innerHtmlChar;
                }
            }

            // add current payload since 
            keyValuePairs.Add(currentData);

            return keyValuePairs;
        }

        private static Dictionary<string, string> FilterAttributesValuesByKeys(Dictionary<string, string> attributes, List<string> keys)
        {
            Dictionary<string, string> attributeValuesByKey = new Dictionary<string, string>();

            foreach (string key in keys)
            {
                if (attributes.ContainsKey(key))
                {
                    attributeValuesByKey.Add(key, attributes[key]);
                }
            }

            return attributeValuesByKey;
        }

        private static string TrimAttributeValueQuotes(string attributeValue)
        {
            return attributeValue.Trim(Separators.AttributeValueQuote);
        }

        public static List<Dictionary<string, string>> ExtractAttributesValues(string html, List<string> filter, List<string> attributeNames)
        {
            List<Dictionary<string, string>> attributeValuesCollection = new List<Dictionary<string, string>>();

            List<string> innerHtmlStrings = SplitStringToInnerHtml(html);
            if (innerHtmlStrings != null)
            {
                List<string> targetInnerHtml = FilterInnerHtmlByValues(innerHtmlStrings, filter);

                foreach (string innerHtmlString in targetInnerHtml)
                {
                    Dictionary<string, string> targetAttributes = GetAttributes(innerHtmlString);
                    if (targetAttributes != null)
                    {
                        Dictionary<string, string> attributeValues = FilterAttributesValuesByKeys(targetAttributes, attributeNames);
                        attributeValuesCollection.Add(attributeValues);
                    }
                }
            }
            return attributeValuesCollection;
        }

        //public static List<string> ExtractInnerContent(string html, string fromTagValue, string toTagValue)
        //{
        //    List<string> valuesToDiscard = new List<string>() { "/div", "br", "br\\", " ", "div class=\"list_det_head\"", "/span", "span", "My serial number:" };
        //    List<string> innerHtmlStrings = SplitStringToInnerHtml(html);
        //    List<string> filteredValues = new List<string>();

        //    if (innerHtmlStrings == null)
        //    {
        //        return null;
        //    }

        //    bool processingInnerContent = false;
        //    foreach (string line in innerHtmlStrings)
        //    {
        //        if (line.Contains(toTagValue))
        //        {
        //            break;
        //        }

        //        if (line.Contains(fromTagValue))
        //        {
        //            processingInnerContent = true;
        //            continue;
        //        }

        //        if (processingInnerContent)
        //        {
        //            if (!valuesToDiscard.Contains(line))
        //            {
        //                filteredValues.Add(line.Trim());
        //            }
        //        }

        //    }

        //    return filteredValues;
        //}
    }
}
