﻿using System.Runtime.CompilerServices;
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
    private int JArrayValueSorter(JArraySortableElement x, JArraySortableElement y)
    {
        if (x.SortingPriority != y.SortingPriority)
        {
            return x.SortingPriority - y.SortingPriority;
        }

        if (x.jObject != null && y.jObject != null)
        {
            x.jObject = SortObject(x.jObject);
            y.jObject = SortObject(y.jObject);
            return string.Compare(x.SortingValue, y.SortingValue, StringComparison.CurrentCulture);
            
        }

        if (x.jValueToSort != null && y.jValueToSort != null)
        {
            if (Configuration.SortPrimitiveValuesInArrays)
            {
                return string.Compare(x.SortingValue, y.SortingValue, StringComparison.CurrentCulture);
            }

            return 0;
        }

        //No need to sort inner arrays (yet)
        if (x.jArrayToSort != null && y.jArrayToSort != null)
        {
            return 0;
        }

        return 0;
    }

    private static int SortJsonsObjects(JArraySortableElement x, JArraySortableElement y)
    {
        return string.Compare(x.SortingValue,
            y.SortingValue, StringComparison.CurrentCulture);
    }
}