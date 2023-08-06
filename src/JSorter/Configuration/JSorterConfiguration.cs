namespace JSorter.Configuration;

public class JSorterConfiguration
{
   /// <summary>
   /// Preservers the order in which primitive JValues appear in the array, placing them below JTokens of another type in the Array. Defaults to "false"
   /// </summary>
   public bool SortPrimitiveValuesInArrays { get; set; }
   /// <summary>
   /// An value to sort Objects belonging in an array.
   /// </summary>
   public List<string> SortArrayObjectBy { get; set; } = new();
}