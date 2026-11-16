using System;
using System.Text.Json;
using System.Text.Json.Nodes;

public class EnvelopeValidator
{
    public static bool ValidateEnvelope(string jsonString, out string errorMessage)
    {
        errorMessage = string.Empty;

        try
        {
            var jsonNode = JsonNode.Parse(jsonString);
            if (jsonNode is not JsonObject jsonObject)
            {
                errorMessage = "Le JSON n'est pas un objet valide.";
                return false;
            }

            string[] requiredFields = { "v", "id", "from", "to", "ts", "type", "payload" };

            // 1. Vérifier la présence de toutes les propriétés requises
            foreach (var field in requiredFields)
            {
                if (!jsonObject.ContainsKey(field))
                {
                    errorMessage = $"La propriété requise '{field}' est absente.";
                    return false;
                }
            }

            // 2. Vérifier "additionalProperties: false" (aucune autre clé autorisée)
            foreach (var kvp in jsonObject)
            {
                if (Array.IndexOf(requiredFields, kvp.Key) == -1)
                {
                    errorMessage = $"Propriété non autorisée : '{kvp.Key}' (additionalProperties est à false).";
                    return false;
                }
            }

            // 3. Valider "v" (const: 1)
            var vNode = jsonObject["v"];
            if (vNode == null || vNode.GetValueKind() != JsonValueKind.Number || vNode.GetValue<int>() != 1)
            {
                errorMessage = "La propriété 'v' doit être un entier égal à 1.";
                return false;
            }

            // 4. Valider "id" (format uuid)
            var idNode = jsonObject["id"];
            if (idNode == null || idNode.GetValueKind() != JsonValueKind.String ||
                !Guid.TryParse(idNode.GetValue<string>(), out _))
            {
                errorMessage = "La propriété 'id' doit être une chaîne au format UUID valide.";
                return false;
            }

            // 5. Valider "from" (minLength: 1)
            var fromNode = jsonObject["from"];
            if (fromNode == null || fromNode.GetValueKind() != JsonValueKind.String ||
                string.IsNullOrEmpty(fromNode.GetValue<string>()))
            {
                errorMessage = "La propriété 'from' ne peut pas être vide.";
                return false;
            }

            // 6. Valider "to" (minLength: 1)
            var toNode = jsonObject["to"];
            if (toNode == null || toNode.GetValueKind() != JsonValueKind.String ||
                string.IsNullOrEmpty(toNode.GetValue<string>()))
            {
                errorMessage = "La propriété 'to' ne peut pas être vide.";
                return false;
            }

            // 7. Valider "ts" (format date-time ISO 8601)
            var tsNode = jsonObject["ts"];
            if (tsNode == null || tsNode.GetValueKind() != JsonValueKind.String ||
                !DateTimeOffset.TryParse(tsNode.GetValue<string>(), out _))
            {
                errorMessage = "La propriété 'ts' doit être une date valide au format ISO 8601 (date-time).";
                return false;
            }

            // 8. Valider "type" (enum: ["time", "log"])
            var typeNode = jsonObject["type"];
            if (typeNode == null || typeNode.GetValueKind() != JsonValueKind.String)
            {
                errorMessage = "La propriété 'type' doit être une chaîne de caractères.";
                return false;
            }
            string typeVal = typeNode.GetValue<string>();
            if (typeVal != "time" && typeVal != "log")
            {
                errorMessage = "La propriété 'type' doit valoir soit 'time', soit 'log'.";
                return false;
            }

            // 9. Valider "payload" (type object)
            var payloadNode = jsonObject["payload"];
            if (payloadNode == null || payloadNode.GetValueKind() != JsonValueKind.Object)
            {
                errorMessage = "La propriété 'payload' doit être un objet JSON.";
                return false;
            }

            return true; // L'enveloppe est entièrement valide !
        }
        catch (JsonException ex)
        {
            errorMessage = $"Erreur de syntaxe JSON : {ex.Message}";
            return false;
        }
    }
}