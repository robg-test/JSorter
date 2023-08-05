using Newtonsoft.Json.Linq;

namespace JSorter.Deconstructor.Objects;

internal interface IDeconstructedJToken
{
    public JToken OriginalJToken { get; set; }
}