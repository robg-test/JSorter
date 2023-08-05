using JSorter.Configuration;
using JSorter.Constructor;
using JSorter.Deconstructor;
using JSorter.Sorter;
using Newtonsoft.Json.Linq;

namespace JSorter.Extensions;
public static class JSorter
{
    
    public static JObject Sort(this JObject jObject, JSorterConfiguration configuration)
    {
        var token = ((JToken)jObject).Sort(configuration);
        return (JObject)token;
    }

    public static JArray Sort(this JArray jArray, JSorterConfiguration configuration)
    {
        var token = ((JToken)jArray).Sort(configuration);
        return (JArray)token;
    }
    
    public static JArray Sort(this JArray token)
    {
        return token.Sort(new JSorterConfiguration());
    }

    public static JObject Sort(this JObject token)
    {
        return (JObject)((JToken)token).Sort(new JSorterConfiguration());
    }

    private static JToken Sort(this JToken token, JSorterConfiguration configuration)
    {
        var deconstructor = new JsonDeconstrcutor(configuration);
        var deconstructedJson = deconstructor.Deconstruct(token);
        var orderer = new JSorterOrderer(deconstructedJson);
        orderer.Sort();
        var constructor = new JsonConstructor();
        var rToken = constructor.ConstructJToken(orderer.SortedJson!);
        return rToken;
    }
}