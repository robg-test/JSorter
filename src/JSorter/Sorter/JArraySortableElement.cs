using System.Globalization;
using JSorter.Deconstructor.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSorter.Sorter;

internal class JArraySortableElement
{
    public int SortingPriority;
    public string? SortingValue;
    public DeconstructedJArray? jArrayToSort;
    public DeconstructedJObject? jObject;
    public JValue? jValueToSort;

    public JArraySortableElement(DeconstructedJArray a)
    {
        this.SortingPriority = int.MaxValue;
        SortingValue = null;
        jArrayToSort = a;
    }

    public JArraySortableElement(DeconstructedJObject b, IEnumerable<string> tokenSortList)
    {
        jObject = b;
        this.SortingPriority = int.MaxValue;
        var token = (JObject)b.OriginalJToken;
        var listOfTokensToSort = tokenSortList.Select(c => token.SelectToken(c)).ToList();
        var index = 0;
        foreach (var i in listOfTokensToSort)
        {
            if (i != null)
            {
                SortingPriority = index;
                SortingValue = GetJValueSortingValue((JValue)i);
                return;
            }
            index++;
        }
        this.SortingValue = token.ToString();
    }

    public JArraySortableElement(JValue valueToSort)
    {
        this.SortingPriority = int.MaxValue;
        SortingValue = GetJValueSortingValue(valueToSort);
        this.jValueToSort = valueToSort;
    }

    string GetJValueSortingValue(JValue value)
    {
        return value.Type == JTokenType.String
            ? value.ToString(CultureInfo.InvariantCulture)
            : value.ToString(Formatting.None);
    }
}