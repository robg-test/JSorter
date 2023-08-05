using JSorter.Configuration;
using JSorter.Extensions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace JSorter.Test;

[TestFixture]
public class ExtensionTest
{
    [Test]
    public void SortValidJObject()
    {
        var jObject = new JObject();
        _ = jObject.Sort();
    }
    [Test]
    public void SortValidJArray()
    {
        var jArray = new JArray();
        _ = jArray.Sort();
    }

    [Test]
    public void SortValidJObjectWithConfig()
    {
        var jObject = new JObject();
        _ = jObject.Sort(new JSorterConfiguration 
        {
            ArrayObjectsSortingPath = "test",
           SortPrimitiveValuesInArrays = true
        });
    }

    [Test]
    public void SortValidJArrayWithConfig()
    {
        var jArray = new JArray();
        _ = jArray.Sort(new JSorterConfiguration
        {
            ArrayObjectsSortingPath = "test",
            SortPrimitiveValuesInArrays = true
        });
    }
}