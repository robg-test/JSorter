using System.Globalization;
using FluentAssertions;
using JSorter.Deconstructor;
using JSorter.Deconstructor.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace JSorter.Test;

[TestFixture]
public class DeconstructorTest
{
    [Test]
    public void DeconstructJObjectWithJProperties()
    {
        const string json = @"{
            ""s1"":""hello"",
            ""s2"":""goodbye"",
            ""b"":true,
            ""c"":1234    
         }";
        var jsonUnderTest = JObject.Parse(json);
        var deconstructor = new JsonDeconstrcutor();
        var deconstructedJson = (DeconstructedJObject)deconstructor.Deconstruct(jsonUnderTest);
        deconstructedJson.ObjectsJProperties![0].JValue!.ToString(CultureInfo.InvariantCulture).Should().Be("hello");
        deconstructedJson.ObjectsJProperties![1].JValue!.ToString(CultureInfo.InvariantCulture).Should().Be("goodbye");
        deconstructedJson.ObjectsJProperties![2].JValue!.ToString(Formatting.None).Should().Be("true");
        deconstructedJson.ObjectsJProperties![3].JValue!.ToString(CultureInfo.InvariantCulture).Should().Be("1234");
    }

    [Test]
    public void DeconstructJObjectWithJObject()
    {
        const string json = @"{
            ""hello"": { 
               ""frominside"":""hello""
                }
         }";
        var jsonUnderTest = JObject.Parse(json);
        var deconstructor = new JsonDeconstrcutor();
        var deconstructedJson = (DeconstructedJObject)deconstructor.Deconstruct(jsonUnderTest);
        deconstructedJson.ObjectsJProperties![0].TextualKey.Should().Be("hello");
        deconstructedJson.ObjectsJProperties![0].PropertiesJObject!.ObjectsJProperties![0].JValue!
            .ToString(CultureInfo.InvariantCulture).Should()
            .Be("hello");
        deconstructedJson.ObjectsJProperties![0].PropertiesJObject!.ObjectsJProperties![0].TextualKey.Should()
            .Be("frominside");
    }

    [Test]
    public void DeconstructedJsonArrayWithPrimitives()
    {
        const string jArray = $@"[ 1,2,3 ]";
        var jsonUnderTest = JArray.Parse(jArray);
        var deconstructor = new JsonDeconstrcutor();
        var deconstructedJson = (DeconstructedJArray)deconstructor.Deconstruct(jsonUnderTest);
        deconstructedJson.JArrayElements[0].jValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("1");
        deconstructedJson.JArrayElements[1].jValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("2");
        deconstructedJson.JArrayElements[2].jValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("3");
    }

    [Test]
    public void DeconstructJsonArrayWithJObject()
    {
        const string jArray = @"[{ ""a"": ""b""}, { ""c"": ""d"" }]";
        var jsonUnderTest = JArray.Parse(jArray);
        var deconstructor = new JsonDeconstrcutor();
        var deconstructedJson = (DeconstructedJArray)deconstructor.Deconstruct(jsonUnderTest);
        deconstructedJson.JArrayElements[0].jObject!.ObjectsJProperties![0].JValue!
            .ToString(CultureInfo.InvariantCulture).Should().Be("b");
    }

    [Test]
    public void DeconstructJsonArrayWithJsonArray()
    {
        const string jArray = @"[[ 1 ]]";
        var jsonUnderTest = JArray.Parse(jArray);
        var deconstructor = new JsonDeconstrcutor();
        var deconstructedJson = (DeconstructedJArray)deconstructor.Deconstruct(jsonUnderTest);
        var primaryArray = deconstructedJson.JArrayElements[0].jArrayToSort;
        var innerValue = primaryArray!.JArrayElements[0].jValueToSort;
        innerValue!.Value!.ToString().Should().Be("1");
    }

    [Test]
    public void DeconstructedJsonObjectWithJsonArray()
    {
        const string jObject = @"{""h"": [ 1, 2, 3 ]}";
        var jsonUnderTest = JObject.Parse(jObject);
        var deconstructor = new JsonDeconstrcutor();
        var deconstructedJson = (DeconstructedJObject)deconstructor.Deconstruct(jsonUnderTest);
        deconstructedJson.ObjectsJProperties![0].PropertyJArray!.JArrayElements[0].jValueToSort!
            .ToString(CultureInfo.CurrentCulture).Should().Be("1");
        deconstructedJson.ObjectsJProperties![0].PropertyJArray!.JArrayElements[1].jValueToSort!.
            ToString(CultureInfo.CurrentCulture).Should().Be("2");
        deconstructedJson.ObjectsJProperties![0].PropertyJArray!.JArrayElements[2].jValueToSort!.
            ToString(CultureInfo.CurrentCulture).Should().Be("3");
    }

    //We need to handle strings if a user passes in a string its interpreted as a JValue, attempting to interpret the string is possibly the best workaround for this
    [Test]
    public void DeconstructJsonString()
    {
        const string json = @"{
           ""s1"":""hello"",
            ""s2"":""goodbye"",
            ""b"":true,
            ""c"":1234    
         }";
        var deconstructor = new JsonDeconstrcutor();
        var deconstructedJson = (DeconstructedJObject)deconstructor.Deconstruct(json);
        deconstructedJson.ObjectsJProperties![0].JValue!.ToString(CultureInfo.InvariantCulture).Should().Be("hello");
    }

    [Test]
    public void DeconstructInvalidJsonStringThrowsException()
    {
        const string invalidJson = "{{}{}";
        var deconstructor = new JsonDeconstrcutor();
        Func<IDeconstructedJToken> func= () => (DeconstructedJObject)deconstructor.Deconstruct(invalidJson);
        func.Should().Throw<Exception>();
    }


    [Test]
    public void DeconstructJsonTokenOfTypeJObject()
    {
        var jObject = new JObject();
        var deconstructor = new JsonDeconstrcutor();
        deconstructor.Deconstruct(jObject).Should().BeOfType(typeof(DeconstructedJObject));
    }

    [Test]
    public void DeconstructJsonTokenOfTypeJArray()
    {
        var jArray = new JArray();
        var deconstructor = new JsonDeconstrcutor();
        deconstructor.Deconstruct(jArray).Should().BeOfType(typeof(DeconstructedJArray));
    }

    [Test]
    public void DeconstructJsonTokenOfTypeJProperty()
    {
        var jProperty = new JProperty("a", "a");
        var deconstructor = new JsonDeconstrcutor();
        deconstructor.Deconstruct(jProperty).Should().BeOfType(typeof(DeconstructedJProperty));
    }

    [Test]
    public void DeconstructJsonTokenOfUnsupportedType()
    {
        var jValue = new JValue("12");
        var deconstructor = new JsonDeconstrcutor();
        Action act = () => deconstructor.Deconstruct(jValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}