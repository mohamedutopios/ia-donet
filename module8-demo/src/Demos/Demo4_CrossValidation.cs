using Microsoft.ML;
using Module8.MlNet.Models;

namespace Module8.MlNet.Demos;

/// <summary>
/// Démo 4 — Cross-validation k-fold + sauvegarde et rechargement du modèle
/// </summary>
public class Demo4_CrossValidation
{
    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 4 : Cross-Validation & Persistance du modèle ────");
        Console.ResetColor();

        var ml   = new MLContext(seed: 42);
        var path = Path.Combine(AppContext.BaseDirectory, "data", "intentions.csv");
        var data = ml.Data.LoadFromTextFile<IntentInput>(path, hasHeader: true, separatorChar: ',');

        var pipeline = ml.Transforms.Text
            .FeaturizeText("Features", nameof(IntentInput.Text))
            .Append(ml.Transforms.Conversion.MapValueToKey("Label", nameof(IntentInput.Label)))
            .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        // ── Cross-validation 5-fold ────────────────────────────────────────────
        Console.WriteLine("\n  [1/3] Cross-validation 5-fold en cours...");
        var sw      = System.Diagnostics.Stopwatch.StartNew();
        var results = ml.MulticlassClassification.CrossValidate(data, pipeline, numberOfFolds: 5);
        sw.Stop();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  Résultats 5-fold ({sw.ElapsedMilliseconds}ms) :");
        Console.ResetColor();

        foreach (var (fold, i) in results.Select((r, i) => (r, i + 1)))
        {
            var color = fold.Metrics.MicroAccuracy >= 0.80 ? ConsoleColor.Green : ConsoleColor.Yellow;
            Console.ForegroundColor = color;
            Console.WriteLine($"    Fold {i} → Accuracy: {fold.Metrics.MicroAccuracy:P1}  |  F1: {fold.Metrics.MacroAccuracy:P1}");
            Console.ResetColor();
        }

        var avgAcc = results.Average(r => r.Metrics.MicroAccuracy);
        var stdDev = Math.Sqrt(results.Average(r => Math.Pow(r.Metrics.MicroAccuracy - avgAcc, 2)));

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  Moyenne : {avgAcc:P1}  ±{stdDev:P1}");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  (un écart-type faible = modèle stable)");
        Console.ResetColor();

        // ── Entraîner sur tout le dataset et sauvegarder ───────────────────────
        Console.WriteLine("\n  [2/3] Entraînement final sur 100% des données...");
        var finalModel = pipeline.Fit(data);
        const string modelPath = "intent_final.zip";
        ml.Model.Save(finalModel, data.Schema, modelPath);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✅ Modèle sauvegardé → {modelPath}");
        Console.ResetColor();

        // ── Recharger et utiliser ─────────────────────────────────────────────
        Console.WriteLine("\n  [3/3] Rechargement du modèle depuis le fichier...");
        var loadedModel = ml.Model.Load(modelPath, out var schema);
        var engine      = ml.Model.CreatePredictionEngine<IntentInput, IntentPrediction>(loadedModel);

        Console.WriteLine("  Modèle rechargé ✅ — test d'inférence :");

        var testPhrases = new[]
        {
            "Je veux passer commande pour ce soir",
            "Quelle est votre politique de retour ?",
            "Cela fait 3 fois que je réclame sans réponse !",
        };

        Console.WriteLine();
        foreach (var phrase in testPhrases)
        {
            var pred  = engine.Predict(new IntentInput { Text = phrase });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"  [{pred.Category.ToUpper(),-8}]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {pred.Confidence:P0}  ");
            Console.ResetColor();
            Console.WriteLine($"\"{phrase}\"");
        }

        await Task.CompletedTask;
    }
}
