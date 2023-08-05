using JSorter.Sorter;
using Newtonsoft.Json.Linq;

namespace JSorter.Deconstructor.Objects;

internal class DeconstructedJArray : IDeconstructedJToken
{
    public List<JArraySortableElement> JArrayElements { get; set; } 
    public JToken OriginalJToken { get; set; }

    public DeconstructedJArray(JToken jToken)
    {
        this.OriginalJToken = jToken;
        JArrayElements = new List<JArraySortableElement>();
    }
}