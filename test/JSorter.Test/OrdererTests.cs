using System.Globalization;
using FluentAssertions;
using JSorter.Configuration;
using JSorter.Deconstructor;
using JSorter.Deconstructor.Objects;
using JSorter.Sorter;
using NUnit.Framework;

namespace JSorter.Test;

[TestFixture]
public class OrdererTests
{
    [Test]
    public void OrderPropertiesWithDefaultSettings()
    {
        const string json = @"{
            ""s1"":""hello"",
            ""s2"":""goodbye"",
            ""b"":true,
            ""c"":1234    
         }";
        var deconstructor = new JsonDeconstrcutor(); 
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![0].TextualKey.Should().Be("b");
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![1].TextualKey.Should().Be("c");
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![2].TextualKey.Should().Be("s1");
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![3].TextualKey.Should().Be("s2");
    }
    
    [Test]
    public void DoNotOrderPropertiesWhenPropertySortIsDisabled()
    {
        const string json = @"{
            ""s1"":""hello"",
            ""s2"":""goodbye"",
            ""b"":true,
            ""c"":1234    
         }";
        var config = new JSorterConfiguration()
        {
            SortJsonObjectProperties = false
        };
        var deconstructor = new JsonDeconstrcutor(config);
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson, config);
        orderer.Sort();
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![0].TextualKey.Should().Be("s1");
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![1].TextualKey.Should().Be("s2");
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![2].TextualKey.Should().Be("b");
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![3].TextualKey.Should().Be("c");
    }
    
    [Test]
    public void OrderPropertiesDefaultId()
    {
        const string json = @"{
        ""a"": { ""b"": ""1"", ""a"": ""2""}
        }";
        var deconstructor = new JsonDeconstrcutor(); 
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        ((DeconstructedJObject)orderer.SortedJson!).ObjectsJProperties![0].PropertiesJObject!.ObjectsJProperties![0]
            .TextualKey.Should().Be("a");
        ((DeconstructedJObject)orderer.SortedJson).ObjectsJProperties![0].PropertiesJObject!.ObjectsJProperties![1]
            .TextualKey.Should().Be("b");
    }

    [Test]
    public void OrderPropertiesInArrayWithArrayPrimitivesSortingEnabled()
    {
        var configuration = new JSorterConfiguration()
        {
            SortPrimitiveValuesInArrays = true
        };
        const string json = @"[ ""b"", ""a"", 1, 1.03, true ]";
        var deconstructor = new JsonDeconstrcutor(configuration); 
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson, configuration);
        orderer.Sort();
        var listOfElements = ((DeconstructedJArray)orderer.SortedJson!).JArrayElements;
        listOfElements[0].JValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("1");
        listOfElements[1].JValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("1.03");
        listOfElements[2].JValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("a");
        listOfElements[3].JValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("b");
        listOfElements[4].JValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("True");
    }

    [Test]
    public void OrderArrayWithArrayPrimitivesSortingDisabled()
    {
        var deconstructor = new JsonDeconstrcutor(); 
        const string json = @"[ ""b"", ""a"", 1, 1.01, true ]";
        var deconstructedJson =  deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        ((DeconstructedJArray)orderer.SortedJson!).JArrayElements[0].JValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("b");
        ((DeconstructedJArray)orderer.SortedJson!).JArrayElements[1].JValueToSort!.ToString(CultureInfo.CurrentCulture).Should().Be("a");
    }

    [Test]
    public void OrderArrayWithObjectsWithSortingElement()
    {
        var conf = new JSorterConfiguration()
        {
            SortPrimitiveValuesInArrays = true
        };
        const string json = @"[ { ""id"" : ""b""}, { ""id"" : ""a""}]";
        var deconstructor = new JsonDeconstrcutor(conf); 
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        var elements = ((DeconstructedJArray)orderer.SortedJson!).JArrayElements.Select(c => c.JObjectToSort).ToList();
        elements[0]!.ObjectsJProperties![0].JValue!.Value.Should().Be("a");
        elements[1]!.ObjectsJProperties![0].JValue!.Value.Should().Be("b");
    }

    [TestCase(@"[ { ""not-id"" : ""a""}, { ""not-id"" : ""b""}]")]
    [TestCase(@"[ { ""not-id"" : ""b""}, { ""not-id"" : ""a""} ]")]
    public void OrderArrayWithObjectWithKeysMissingSortingElement(string json)
    {
        var conf = new JSorterConfiguration()
        {
            SortPrimitiveValuesInArrays = true
        };
        var deconstructor = new JsonDeconstrcutor(conf); 
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        var elements = ((DeconstructedJArray)orderer.SortedJson!).JArrayElements.Select(c => c.JObjectToSort).ToList();
        elements[0]!.ObjectsJProperties![0].JValue!.Value.Should().Be("a");
        elements[1]!.ObjectsJProperties![0].JValue!.Value.Should().Be("b");
        
    }

    [TestCase(@"[ { ""aid"" : ""a""}, { ""id"" : ""b""}]")]
    [TestCase(@"[ { ""id"" : ""b""}, { ""aid"" : ""a""} ]")]
    public void OrderArrayWithObjectWithSortingElementsWithPrecedenceOverMissingKey(string json)
    {
        var conf = new JSorterConfiguration()
        {
            SortPrimitiveValuesInArrays = true,
            SortArrayObjectBy = new List<string>() { "id" }
        };
        var deconstructor = new JsonDeconstrcutor(conf); 
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        var elements = ((DeconstructedJArray)orderer.SortedJson!).JArrayElements.Select(c => c.JObjectToSort).ToList();
        elements[0]!.ObjectsJProperties![0].JValue!.Value.Should().Be("b");
        elements[1]!.ObjectsJProperties![0].JValue!.Value.Should().Be("a");
    }
 
    [TestCase(@"[[],{},3]")]
    [TestCase("[3,{},[]]")]
    public void PreserveOrderOnArraysWithDifferingTypes(string json)
    {
        var deconstructor = new JsonDeconstrcutor(); 
        var deconstructedJson = deconstructor.Deconstruct(json);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        ((DeconstructedJArray)deconstructedJson).OriginalJToken.ToString().TrimAllWhitespace().Should().Be(json);
    }
}