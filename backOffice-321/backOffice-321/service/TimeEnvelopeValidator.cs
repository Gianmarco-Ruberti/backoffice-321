using System;
using System.Text.Json;
using System.Text.Json.Nodes;

public class TimeEnvelopeValidator
{
    public static bool ValidateTimeEnvelope(string jsonString, out string errorMessage)
    {
        errorMessage = string.Empty;

        try
        {
            // 1. Analyser le JSON en tant qu'objet dynamique (JsonNode)
            var jsonNode = JsonNode.Parse(jsonString);
            if (jsonNode is not JsonObject jsonObject)
            {
                errorMessage = "Le JSON n'est pas un objet valide.";
                return false;
            }

            // 2. Vérifier "additionalProperties: false" (seule la clé "timestamp" est autorisée)
            foreach (var kvp in jsonObject)
            {
                if (kvp.Key != "timestamp")
                {
                    errorMessage = $"Propriété non autorisée : '{kvp.Key}' (additionalProperties est à false).";
                    return false;
                }
            }

            // 3. Vérifier que la propriété requise "timestamp" est présente
            if (!jsonObject.ContainsKey("timestamp"))
            {
                errorMessage = "La propriété requise 'timestamp' est absente.";
                return false;
            }

            // 4. Vérifier que "timestamp" est bien un nombre / entier (integer)
            var timestampNode = jsonObject["timestamp"];
            if (timestampNode == null || timestampNode.GetValueKind() != JsonValueKind.Number)
            {
                errorMessage = "La propriété 'timestamp' doit être de type entier (integer).";
                return false;
            }

            return true; // Le JSON respecte parfaitement le schéma !
        }
        catch (JsonException ex)
        {
            errorMessage = $"Erreur de syntaxe JSON : {ex.Message}";
            return false;
        }
    }
}