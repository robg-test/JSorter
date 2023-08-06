using JSorter.Configuration;
using JSorter;
using Newtonsoft.Json.Linq;
using Snapshooter.NUnit;

namespace JSorter.IntegrationTests;

[TestFixture]
public class SnapshooterIntegrationTest
{
    //Example from: https://developer.nhs.uk/apis/gpconnect-1-6-0/accessrecord_structured_development_fhir_examples_allergies.html
    [Test]
    public void SnapshooterIntegrationWithSortingKey()
    {
        var jTest = JObject.Parse(File.ReadAllText("Json/fhirExample.json"));
        jTest = jTest.Sort(new JSorterConfiguration()
        {
            SortArrayObjectBy = new List<string>() { "resource.id" }
        });
        Snapshot.Match(jTest);
    }

    [Test]
    public void SnapshooterIntegrationWithNoSortingKey()
    {
        var jTest = JObject.Parse(File.ReadAllText("Json/fhirExample.json"));
        jTest = jTest.Sort();
        Snapshot.Match(jTest);
    }
}