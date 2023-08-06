using System.Runtime.CompilerServices;
using JSorter.Deconstructor.Objects;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("JSorter.Test")]
namespace JSorter.Constructor;


internal class JsonConstructor
{
    public JToken ConstructJToken(IDeconstructedJToken json)
    {
        if (json.GetType() == typeof(DeconstructedJObject))
        {
            return ConstructJObject((DeconstructedJObject)json);
        }
        else if (json.GetType() == typeof(DeconstructedJArray))
        {
            return ConstructJArray((DeconstructedJArray)json);
        }
        throw new Exception("Unsupported Deconstructed JToken Type");
    }
    private JObject ConstructJObject(DeconstructedJObject jsonUnderTest)
    {
        var jObject = new JObject();

        foreach (var jProp in jsonUnderTest.ObjectsJProperties!)
        {
            if (jProp.PropertiesJObject == null && jProp.PropertyJArray == null)
            {
                var prop = new JProperty(jProp.TextualKey!, jProp.JValue);
                jObject.Add(prop);
            }

            if (jProp.PropertiesJObject != null)
            {
                var obj = ConstructJObject(jProp.PropertiesJObject);
                var prop = new JProperty(jProp.TextualKey!, obj);
                jObject.Add(prop);
            }

            if (jProp.PropertyJArray != null)
            {
                var obj = ConstructJArray(jProp.PropertyJArray);
                var prop = new JProperty(jProp.TextualKey!, obj);
                jObject.Add(prop);
            }
        }
        return jObject;
    }

    private JArray ConstructJArray(DeconstructedJArray jPropPropertyJArray)
    {
        var jArray = new JArray();
        foreach (var i in jPropPropertyJArray.JArrayElements)
        {
            if (i.JObjectToSort != null)
            {
                jArray.Add(new JObject(ConstructJObject(i.JObjectToSort)));
            }

            if (i.JValueToSort != null)
            {
                jArray.Add(i.JValueToSort);
            }

            if (i.JArrayToSort != null)
            {
                jArray.Add(new JArray(ConstructJArray(i.JArrayToSort)));
            }
        }
        return jArray;
    }
}
    
