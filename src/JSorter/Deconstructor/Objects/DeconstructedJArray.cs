using Newtonsoft.Json.Linq;

namespace JSorter.Deconstructor.Objects;

internal class DeconstructedJArray : IDeconstructedJToken
{
    public readonly List<KeyValuePair<string, object>> JArrayElements; 
    public JToken OriginalJToken { get; set; }

    public DeconstructedJArray(JToken jToken)
    {
        this.JArrayElements = new List<KeyValuePair<string, object>>();
        this.OriginalJToken = jToken;
    }
}