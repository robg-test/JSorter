# JSorter #

Sort all the JSON

__Disclaimer__: This tool is designed in mind for checking JSON as part of Quality Assurance for the sake of the human viewing JSON.
Your own code should not be reliant on how things are ordered. Also keep in mind that order can matter in JSON and it's up to you to decide how the tool will be used.

## Downloads ##

The latest stable release of JSorter is [available on NuGet](https://www.nuget.org/packages/RobTest.JSorter/) or can be [downloaded from GitHub](https://github.com/JSorter/Jsorter/releases). 

## Using JSorter ##

To order using default configuration is a simple to use extension method
```csharp
using JSorter;

var a = JObject.Parse(@"{ ""b"" : ""b"", ""a"": ""a"" }");
a = a.Sort();
Console.Write(a.ToString());
```

```json
{
  "a": "a",
  "b": "b"
}
```

## Configuration ##

### Sorting Primitive Values ###

By default sorting primitive values in Json is disabled, this is due to the fact that the sequence of primitives may matter
This can be enabled via configuration passed as an additional argument when using the ``Sort()`` method 

__Example__

```csharp
using JSorter;
using JSorter.Configuration;

jTest = jTest.Sort(new JSorterConfiguration() {
        SortPrimitiveValuesInArrays = true
    }

var a = JArray.Parse(@"[2,1,3]");
a = a.Sort();
Console.Write(a.ToString());
```

__Outputs__

```json
[ 1,2,3 ]
```

### Sorting JSON Objects in an Array by a Key. ###

By default, objects belonging to a JSON array are sorted by there string representation. However this may cause problems if the first field changes
You can use a selector to use as a sorting value for arrays.

__Example__

```csharp
using JSorter;
using JSorter.Configuration;

jTest = jTest.Sort(new JSorterConfiguration() {
            SortArrayObjectBy = new List<string>() { "id" }
    }

var a = JArray.Parse(@"[{""a"": 1, ""id"":""2""},{""b"": 1, ""id"":""1""}]");
a = a.Sort();
Console.Write(a.ToString());
```

__Outputs__

```json
[
  {
    "b" : 1,
    "id": 1
  },
  {
    "a": 1,
    "id": 2
  }
]
```
### Disable Property Sorting ###

In some scenarios you may not want to sort the properties of an object. 
The sorting of properties can be disabled using the ``SortJsonObjectProperties`` flag.

```csharp
using JSorter;
using JSorter.Configuration;

jTest = jTest.Sort(new JSorterConfiguration() {
            SortArrayObjectBy = new List<string>() { "id" },
            SortJsonObjectProperties = false
    }

var a = JArray.Parse(@"[{""k"": 1, ""id"":""2""},{""l"": 1, ""id"":""1""}]");
a = a.Sort();
Console.Write(a.ToString());
```
__Outputs__

```json
[
  {
    "l" : 1,
    "id": 1
  },
  {
    "k": 1,
    "id": 2
  }
]
```