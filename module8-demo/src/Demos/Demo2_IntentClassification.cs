using Microsoft.ML;
using Module8.MlNet.Models;
using Module8.MlNet.Services;

namespace Module8.MlNet.Demos;

/// <summary>
/// Démo 2 — Classification multi-classe : intentions (commande / question / plainte)
/// </summary>
public class Demo2_IntentClassification
{
    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 2 : Classification d'intentions (Multi-classe) ──");
        Console.ResetColor();

        var ml   = new MLContext(seed: 42);
        var path = Path.Combine(AppContext.BaseDirectory, "data", "intentions.csv");

        // ── 1. Charger ──────────────────────────────────────────────────────────
        Console.WriteLine("\n  [1/5] Chargement des données...");
        var data  = ml.Data.LoadFromTextFile<IntentInput>(path, hasHeader: true, separatorChar: ',');
        var split = ml.Data.TrainTestSplit(data, testFraction: 0.25);

        // ── 2. Pipeline ─────────────────────────────────────────────────────────
        Console.WriteLine("\n  [2/5] Construction du pipeline SDCA MaximumEntropy...");
        var pipeline = ml.Transforms.Text
            .FeaturizeText("Features", nameof(IntentInput.Text))
            .Append(ml.Transforms.Conversion
                .MapValueToKey(
                    outputColumnName: "Label",
                    inputColumnName:  nameof(IntentInput.Label)))
            .Append(ml.MulticlassClassification.Trainers
                .SdcaMaximumEntropy(
                    labelColumnName:   "Label",
                    featureColumnName: "Features"))
            .Append(ml.Transforms.Conversion
                .MapKeyToValue("PredictedLabel"));

        // ── 3. Entraîner ────────────────────────────────────────────────────────
        Console.WriteLine("\n  [3/5] Entraînement...");
        var sw    = System.Diagnostics.Stopwatch.StartNew();
        var model = pipeline.Fit(split.TrainSet);
        sw.Stop();
        Console.WriteLine($"  Modèle entraîné en {sw.ElapsedMilliseconds}ms");

        // ── 4. Évaluer ──────────────────────────────────────────────────────────
        Console.WriteLine("\n  [4/5] Évaluation...");
        var preds   = model.Transform(split.TestSet);
        var metrics = ml.MulticlassClassification.Evaluate(preds, "Label", "PredictedLabel");
        TrainingHelper.PrintMulticlassMetrics(metrics, "Intent Classification");

        // ── 5. Inférence ────────────────────────────────────────────────────────
        Console.WriteLine("\n  [5/5] Prédictions :");
        var engine = ml.Model.CreatePredictionEngine<IntentInput, IntentPrediction>(model);

        var testPhrases = new[]
        {
            "Je veux commander 2 croissants et un café",
            "Votre site est-il disponible en anglais ?",
            "Mon remboursement est toujours en attente depuis 3 semaines !",
            "Ajouter ce produit à mon panier",
            "Quels moyens de paiement acceptez-vous ?",
            "C'est scandaleux, je vais porter plainte",
        };

        Console.WriteLine();
        var colors = new Dictionary<string, ConsoleColor>
        {
            ["commande"] = ConsoleColor.Green,
            ["question"] = ConsoleColor.Cyan,
            ["plainte"]  = ConsoleColor.Red,
        };

        foreach (var phrase in testPhrases)
        {
            var pred  = engine.Predict(new IntentInput { Text = phrase });
            var color = colors.GetValueOrDefault(pred.Category, ConsoleColor.Gray);

            Console.ForegroundColor = color;
            Console.Write($"  [{pred.Category.ToUpper(),-8}]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {pred.Confidence:P0}  ");
            Console.ResetColor();
            Console.WriteLine($"\"{phrase}\"");
        }

        ml.Model.Save(model, data.Schema, "intent.zip");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  💾 Modèle sauvegardé → intent.zip");
        Console.ResetColor();

        await Task.CompletedTask;
    }
}
