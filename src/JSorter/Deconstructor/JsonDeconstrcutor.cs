﻿using System.Globalization;
using System.Runtime.CompilerServices;
using JSorter.Configuration;
using JSorter.Deconstructor.Objects;
using JSorter.Sorter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("JSorter.Test")]

namespace JSorter.Deconstructor;

internal class JsonDeconstrcutor
{
    private readonly JSorterConfiguration _configuration;

    public JsonDeconstrcutor()
    {
        this._configuration = DefaultJSorterConfiguration.Configuration;
    }

    public JsonDeconstrcutor(JSorterConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public IDeconstructedJToken Deconstruct(JToken jsonUnderTest)
    {
        switch (jsonUnderTest.Type)
        {
            case JTokenType.Object:
                return DeconstructJObject(jsonUnderTest);
            case JTokenType.Array:
                return DeconstructJArray(jsonUnderTest);
            case JTokenType.Property:
                return DeconstructJProperty((JProperty)jsonUnderTest);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IDeconstructedJToken DeconstructJProperty(JProperty token)
    {
        var deconstructedJProperty = new DeconstructedJProperty(token)
        {
            TextualKey = token.Name
        };

        var jObject = token.Children<JObject>().FirstOrDefault();
        if (jObject != null)
        {
            var deconstructedJPropObject = Deconstruct(jObject);
            deconstructedJProperty.PropertiesJObject = (DeconstructedJObject)deconstructedJPropObject;
        }

        foreach (var inToken in token.Children<JValue>())
        {
            deconstructedJProperty.JValue =
                (JValue)inToken.Value<object>()!;
        }

        var jArray = token.Children<JArray>().FirstOrDefault();
        if (jArray != null)
        {
            var deconstructedJPropArray = Deconstruct(jArray);
            deconstructedJProperty.PropertyJArray = (DeconstructedJArray)deconstructedJPropArray;
        }

        return deconstructedJProperty;
    }

    private IDeconstructedJToken DeconstructJArray(JToken token)
    {
        var deconstructedJArray = new DeconstructedJArray(token);
        foreach (var inToken in token.Children())
        {
            if (inToken.Type == JTokenType.Object)
            {
                deconstructedJArray.JArrayElements.Add(
                    new JArraySortableElement((DeconstructedJObject)Deconstruct(inToken),
                        _configuration.SortArrayObjectBy));
            }

            else if (inToken.Type == JTokenType.Array)
            {
                deconstructedJArray.JArrayElements.Add(new JArraySortableElement(
                    (DeconstructedJArray)Deconstruct(inToken)));
            }

            else
            {
                deconstructedJArray.JArrayElements.Add(new JArraySortableElement(
                    (JValue)inToken.Value<object>()!));
            }
        }

        return deconstructedJArray;
    }

    private IDeconstructedJToken DeconstructJObject(JToken token)
    {
        var deconstructedJObject = new DeconstructedJObject(token)
        {
            OriginalJToken = token
        };
        foreach (var inToken in token.Children<JProperty>())
        {
            deconstructedJObject.ObjectsJProperties!.Add(
                (DeconstructedJProperty)Deconstruct(inToken));
        }

        return deconstructedJObject;
    }

    public IDeconstructedJToken Deconstruct(string json)
    {
        var token = JToken.Parse(json);
        return Deconstruct(token);
    }
}