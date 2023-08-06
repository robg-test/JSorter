using Newtonsoft.Json.Linq;

namespace JSorter.Deconstructor.Objects;

internal class DeconstructedJProperty : IDeconstructedJToken
{
    public DeconstructedJObject? PropertiesJObject { get; set; }
    public JValue? JValue { get; set; }
    public DeconstructedJArray? PropertyJArray { get; set; }
    public DeconstructedJProperty(JToken jToken)
    {
        this.OriginalJToken = jToken;
    }

    public JToken OriginalJToken { get; set; }
    public string? TextualKey { get; init; }
}