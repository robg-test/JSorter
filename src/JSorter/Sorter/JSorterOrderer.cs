using System.Runtime.CompilerServices;
using JSorter.Configuration;
using JSorter.Deconstructor.Objects;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("JSorter.Test")]
namespace JSorter.Sorter;

internal class JSorterOrderer
{
    public JSorterOrderer(IDeconstructedJToken parentJsonToken, JSorterConfiguration configuration)
    {
        this.JsonToSort = parentJsonToken;
        this.Configuration = configuration;
    }

    public JSorterOrderer(IDeconstructedJToken parentJsonToken)
    {
        this.JsonToSort = parentJsonToken;
        this.Configuration = DefaultJSorterConfiguration.Configuration;
    }

    private JSorterConfiguration Configuration { get; set; }
    private IDeconstructedJToken JsonToSort { get; set; }
    public IDeconstructedJToken? SortedJson { get; private set; }

    public void Sort()
    {
        if (JsonToSort.GetType() == typeof(DeconstructedJObject))
        {
            var orderedObject = SortObject((DeconstructedJObject)JsonToSort);
            SortedJson = orderedObject;
        }

        if (JsonToSort.GetType() == typeof(DeconstructedJArray))
        {
            var orderedArray = SortArray((DeconstructedJArray)JsonToSort);
            SortedJson = orderedArray;
        }
    }

    private DeconstructedJObject SortObject(DeconstructedJObject token)
    {
        token.ObjectsJProperties =
            token.ObjectsJProperties!.OrderBy(property => property.TextualKey).ToList();
        var jProps = token.ObjectsJProperties;
        foreach (var i in jProps!)
        {
            if (i.PropertiesJObject != null)
            {
                i.PropertiesJObject = SortObject(i.PropertiesJObject);
            }

            if (i.PropertyJArray != null)
            {
                i.PropertyJArray = SortArray(i.PropertyJArray);
            }
        }

        return token;
    }

    private DeconstructedJArray SortArray(DeconstructedJArray array)
    {
        array.JArrayElements.Sort(JArrayValueSorter);
        return array;
    }
//TODO - Rewrite the KVP usage it requires digging to understand, and we can make it more verbose with an object.
    private int JArrayValueSorter(KeyValuePair<string, object> x, KeyValuePair<string, object> y)
    {
        if (x.Key == "" && y.Key == "")
        {
            return SortJsonElementsWithNoKey(x, y);
        }
        if (x.Key == "") return 1;
        if (x.Value.GetType() == typeof(JValue) && Configuration.SortPrimitiveValuesInArrays)
        {
            return string.Compare(x.Value.ToString(), y.Value.GetType() == typeof(JValue) ? y.Value.ToString() : y.Key, StringComparison.CurrentCulture);
        }
        if (y.Key == "") return -1;
        if (x.Value.GetType() != typeof(JValue) || Configuration.SortPrimitiveValuesInArrays)
            return string.Compare(x.Key, y.Key, StringComparison.CurrentCulture);
        if (y.Value.GetType() == typeof(JValue) || y.Key == "")
        {
            return 0;
        }
        //Quite a bit of behaviour on this line - By default if primitives aren't sorted then primitive values will be at the bottom of the array
        //In the scenario where we have values and arrays or objects in the JArray
        return -1;
    }

    private static int SortJsonElementsWithNoKey(KeyValuePair<string, object> x, KeyValuePair<string, object> y)
    {
        if (x.Value.GetType() == typeof(DeconstructedJObject) && y.Value.GetType() == typeof(DeconstructedJObject))
        {
            var deconstructedObjectX = (DeconstructedJObject)x.Value;
            var deconstructedObjectY = (DeconstructedJObject)y.Value;
            return string.Compare(deconstructedObjectX.OriginalJToken.ToString(),
                deconstructedObjectY.OriginalJToken.ToString(), StringComparison.CurrentCulture);
        }
        return 0;
    }
}