using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;

/// <summary>
/// Construit un message de log conforme aux contrats contracts/envelope.schema.json
/// et contracts/log.schema.json :
/// { v, id, from, to, ts, type: "log", payload: { level, text } }
/// </summary>
public static class LogEnvelope
{
    // Sans échappement de +, ' ou des accents : la sortie reste lisible comme celle des autres nœuds
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Create(string from, string to, string level, string text)
    {
        var envelope = new Dictionary<string, object?>
        {
            ["v"] = 1,
            ["id"] = Guid.NewGuid().ToString(),
            ["from"] = from,
            ["to"] = to,
            // UTC, ISO 8601 avec fraction et offset (2026-11-25T12:51:00.4945971+00:00)
            ["ts"] = DateTimeOffset.UtcNow.ToString("o"),
            ["type"] = "log",
            ["payload"] = new Dictionary<string, object?>
            {
                ["level"] = level,
                ["text"] = text
            }
        };

        return JsonSerializer.Serialize(envelope, JsonOptions);
    }
}
