namespace JSorter.Configuration;

public class JSorterConfiguration
{
   /// <summary>
   /// Preservers the order in which JValues appear in the array, placing them below JTokens of another type in the Array. Defaults to "false"
   /// </summary>
   public bool SortPrimitiveValuesInArrays { get; set; }
   /// <summary>
   /// An value to sort Objects belonging in an array. Defaults to "id"
   /// </summary>
   public List<string> SortArrayObjectBy { get; set; } = new();
}

public static class DefaultJSorterConfiguration
{
   public static JSorterConfiguration Configuration;
   static DefaultJSorterConfiguration()
   {
      Configuration = new JSorterConfiguration();
   }
}