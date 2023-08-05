namespace JSorter.Test;

public static class TestExtensions
{
   public static string TrimAllWhitespace(this string input)
   {
      return input.ToCharArray()
         .Where(c => !char.IsWhiteSpace(c))
         .Select(c => c.ToString())
         .Aggregate((a, b) => a + b);
   } 
}