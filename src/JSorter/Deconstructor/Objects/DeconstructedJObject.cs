using Newtonsoft.Json.Linq;

namespace JSorter.Deconstructor.Objects;

internal class DeconstructedJObject : IDeconstructedJToken
{
    public JToken OriginalJToken { get; set; }
    public List<DeconstructedJProperty>? ObjectsJProperties { get; set; }

    public DeconstructedJObject(JToken jToken)
    {
        this.OriginalJToken = jToken;
        this.ObjectsJProperties = new List<DeconstructedJProperty>();
    }
}