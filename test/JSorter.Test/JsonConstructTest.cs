using FluentAssertions;
using JSorter.Constructor;
using JSorter.Deconstructor;
using JSorter.Deconstructor.Objects;
using JSorter.Sorter;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace JSorter.Test;

[TestFixture]
public class JsonConstructTest
{
    [Test]
    public void ConstructJsonFromJObject()
    {
        const string json = @"{ ""b"": ""b"", ""a"": ""c"" }";
        var jsonUnderTest = DeconstructAndSort(json);
        var constructor = new JsonConstructor();
        var jObject = constructor.ConstructJToken(jsonUnderTest);
        var jProperties = ((JObject)jObject).Properties().ToList();
        jProperties[0].ToString().Should().Be(@"""a"": ""c""");
        jProperties[1].ToString().Should().Be(@"""b"": ""b""");
    }

    [Test]
    public void ConstructJsonFromObjectWithObject()
    {
        const string json = @"{ ""a"": { ""b"": ""b"", ""a"": ""c"" }}";
        var jsonUnderTest = DeconstructAndSort(json);
        var constructor = new JsonConstructor();
        var jObject = constructor.ConstructJToken(jsonUnderTest);
        var firstJObject = (JObject)((JObject)jObject).Properties().ToList()[0].Value;
        firstJObject.Properties().ToList()[0].ToString().Should().Be(@"""a"": ""c""");
        firstJObject.Properties().ToList()[1].ToString().Should().Be(@"""b"": ""b""");
    }

    [Test]
    public void ConstructJsonFromObjectWithObjectMultipleObjects()
    {
        const string json = @"{ ""b"": { ""b"": ""b"", ""a"": ""c"" }, ""a"": ""b""}";
        var jsonUnderTest = DeconstructAndSort(json);
        var constructor = new JsonConstructor();
        var jObject = constructor.ConstructJToken(jsonUnderTest);
        ((JObject)jObject).Properties().ToList()[0].ToString().Should().Be(@"""a"": ""b""");
    }

    [Test]
    public void ConstructJsonFromObjectWithContainedArrays()
    {
        const string json = @"{ ""b"": [ 1,2,3 ], ""a"": [ 4,5,6 ]}";
        var jsonUnderTest = DeconstructAndSort(json);
        var constructor = new JsonConstructor();
        var jObject = constructor.ConstructJToken((DeconstructedJObject)jsonUnderTest);
        ((JObject)jObject).Properties().ToList()[0].ToString().TrimAllWhitespace()
            .Should().Be(@"""a"":[4,5,6]");
    }
    
    [Test]
    public void ConstructJsonFromArrayWithContainedArray()
    {
        const string json = @"[[ 1,2,3 ],[ 4,5,6 ]]";
        var jsonUnderTest = DeconstructAndSort(json);
        var constructor = new JsonConstructor();
        var jArray = (JArray)constructor.ConstructJToken((DeconstructedJArray)jsonUnderTest);
        jArray[0][0]!.ToString().Should().Be("1");
    }

    [Test]
    public void ConstructJsonArrayWithNestedJObject()
    {
        const string json = @"[{""a"":""b""}]";
        var jsonUnderTest = DeconstructAndSort(json);
        var constructor = new JsonConstructor();
        var jArray = constructor.ConstructJToken((DeconstructedJArray)jsonUnderTest);
    }

    private static IDeconstructedJToken DeconstructAndSort(string json)
    {
        var deconstructor = new JsonDeconstrcutor();
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        return deconstructedJson;
    }
}