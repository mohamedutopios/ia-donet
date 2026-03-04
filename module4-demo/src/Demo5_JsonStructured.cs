using System.Text.Json;
using System.Text.Json.Serialization;

namespace Module4.PromptEngineering;

// ── Modèles attendus en sortie ────────────────────────────────────────────────

public class RecetteIA
{
    [JsonPropertyName("nom")]         public string   Nom         { get; set; } = "";
    [JsonPropertyName("ingredients")] public string[] Ingredients { get; set; } = [];
    [JsonPropertyName("etapes")]      public string[] Etapes      { get; set; } = [];
    [JsonPropertyName("temps_min")]   public int      TempsMins   { get; set; }
    [JsonPropertyName("difficulte")]  public string   Difficulte  { get; set; } = "";
}

/// <summary>
/// Démo 5 — Sortie JSON structurée avec validation et retry
/// </summary>
public class JsonStructuredDemo
{
    private readonly AiClient _ai;
    private const int MaxRetries = 3;

    public JsonStructuredDemo(AiClient ai) => _ai = ai;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 5 : JSON Structuré + Retry ──────────────");
        Console.ResetColor();

        var systemPrompt = """
            Tu es un assistant culinaire. Tu réponds UNIQUEMENT en JSON valide.
            Aucun texte avant ou après le JSON. Pas de backticks markdown.
            Schema OBLIGATOIRE :
            {
              "nom": "string",
              "ingredients": ["string", ...],
              "etapes": ["string", ...],
              "temps_min": number,
              "difficulte": "facile|moyen|difficile"
            }
            """;

        var userPrompt = "Donne-moi une recette simple de pâtes carbonara.";

        RecetteIA? recette = null;
        int attempt = 0;

        while (recette == null && attempt < MaxRetries)
        {
            attempt++;
            Console.WriteLine($"\n  Tentative {attempt}/{MaxRetries}...");

            var (raw, pt, ct) = await _ai.ChatAsync(systemPrompt, userPrompt, temperature: 0, maxTokens: 400);

            // Nettoyer les backticks Markdown éventuels
            var json = raw
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            // Extraire entre { et }
            var start = json.IndexOf('{');
            var end   = json.LastIndexOf('}');

            if (start == -1 || end == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  ⚠️  JSON introuvable dans la réponse — retry");
                Console.ResetColor();
                userPrompt = "Réponds UNIQUEMENT avec le JSON, sans aucun autre texte. " + userPrompt;
                continue;
            }

            try
            {
                recette = JsonSerializer.Deserialize<RecetteIA>(json[start..(end + 1)]);

                if (recette == null || string.IsNullOrEmpty(recette.Nom))
                    throw new JsonException("Objet vide");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  ✅ JSON valide !");
                Console.ResetColor();
                AiClient.Print("Recette désérialisée", "", pt, ct);

                Console.WriteLine($"  Nom         : {recette.Nom}");
                Console.WriteLine($"  Ingrédients : {string.Join(", ", recette.Ingredients)}");
                Console.WriteLine($"  Temps       : {recette.TempsMins} min");
                Console.WriteLine($"  Difficulté  : {recette.Difficulte}");
                Console.WriteLine($"  Étapes      : {recette.Etapes.Length} étape(s)");
                foreach (var (etape, i) in recette.Etapes.Select((e, i) => (e, i + 1)))
                    Console.WriteLine($"    {i}. {etape}");
            }
            catch (JsonException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ⚠️  Désérialisation échouée : {ex.Message} — retry");
                Console.ResetColor();
            }
        }

        if (recette == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  ❌ Échec après {MaxRetries} tentatives.");
            Console.ResetColor();
        }
    }
}
