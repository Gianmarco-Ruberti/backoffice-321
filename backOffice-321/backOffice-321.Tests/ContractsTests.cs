using Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Schema;
using System.Text.Json.Nodes;

namespace MilleMiettes.Tests
{
    [TestClass]
    public class ContractsTests
    {
        private static readonly string ContractsDir = Path.Combine(AppContext.BaseDirectory, "contracts");

        [TestMethod]
        public void TimeSample_PayloadMatchesTimeSchema()
        {
            JsonSchema schema = JsonSchema.FromFile(Path.Combine(ContractsDir, "time.schema.json"));

            // The sample is a full envelope: the time schema only describes its payload
            JsonNode sample = JsonNode.Parse(File.ReadAllText(Path.Combine(ContractsDir, "samples", "time.json")))!;
            JsonNode? payload = sample["payload"];

            EvaluationResults results = schema.Evaluate(payload, new EvaluationOptions { OutputFormat = OutputFormat.List });

            Assert.IsTrue(results.IsValid, Describe(results));
        }

        [TestMethod]
        public void TimeSample_MatchesEnvelopeSchema()
        {
            JsonSchema schema = JsonSchema.FromFile(Path.Combine(ContractsDir, "envelope.schema.json"));
            JsonNode sample = JsonNode.Parse(File.ReadAllText(Path.Combine(ContractsDir, "samples", "time.json")))!;

            // In draft 2020-12, "format" (uuid, date-time) is only an annotation unless validation is required explicitly
            EvaluationOptions options = new EvaluationOptions
            {
                OutputFormat = OutputFormat.List,
                RequireFormatValidation = true
            };
            EvaluationResults results = schema.Evaluate(sample, options);

            Assert.IsTrue(results.IsValid, Describe(results));
        }

        [TestMethod]
        public void BadTimeSample_FailsEnvelopeSchemaOnId()
        {
            JsonSchema schema = JsonSchema.FromFile(Path.Combine(ContractsDir, "envelope.schema.json"));
            JsonNode sample = JsonNode.Parse(File.ReadAllText(Path.Combine(ContractsDir, "samples", "badtime.json")))!;

            EvaluationOptions options = new EvaluationOptions
            {
                OutputFormat = OutputFormat.List,
                RequireFormatValidation = true
            };
            EvaluationResults results = schema.Evaluate(sample, options);

            Assert.IsFalse(results.IsValid, "badtime.json should be rejected");

            // Make sure it is rejected for the expected reason: "1234" is not a uuid
            bool idRejected = results.Details.Any(d => d.HasErrors && d.InstanceLocation.ToString() == "/id");
            Assert.IsTrue(idRejected, "Expected an error on /id, got:" + Environment.NewLine + Describe(results));
        }

        // Lists every failing location with its error messages, to make a red test readable
        private static string Describe(EvaluationResults results)
        {
            IEnumerable<string> errors = results.Details
                .Where(d => d.HasErrors)
                .SelectMany(d => d.Errors!.Select(e => $"{d.InstanceLocation}: {e.Value}"));
            return string.Join(Environment.NewLine, errors);
        }
    }
}