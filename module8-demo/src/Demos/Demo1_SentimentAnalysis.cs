using Microsoft.ML;
using Module8.MlNet.Models;
using Module8.MlNet.Services;

namespace Module8.MlNet.Demos;

/// <summary>
/// Démo 1 — Classification binaire : analyse de sentiment (positif / négatif)
/// </summary>
public class Demo1_SentimentAnalysis
{
    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 1 : Analyse de Sentiment (Binaire) ──────────────");
        Console.ResetColor();

        var ml   = new MLContext(seed: 42);
        var path = Path.Combine(AppContext.BaseDirectory, "data", "sentiment.csv");

        // ── 1. Charger les données ─────────────────────────────────────────────
        Console.WriteLine("\n  [1/5] Chargement des données...");
        var data  = ml.Data.LoadFromTextFile<SentimentInput>(path, hasHeader: true, separatorChar: ',');
        var split = ml.Data.TrainTestSplit(data, testFraction: 0.25);

        var totalRows = data.GetColumn<bool>("Label").Count();
        Console.WriteLine($"  {totalRows} exemples chargés — 75% train / 25% test");

        // ── 2. Pipeline ────────────────────────────────────────────────────────
        Console.WriteLine("\n  [2/5] Construction du pipeline...");
        var pipeline = ml.Transforms.Text
            .FeaturizeText("Features", nameof(SentimentInput.Text))
            .Append(ml.BinaryClassification.Trainers
                .SdcaLogisticRegression(
                    labelColumnName:   nameof(SentimentInput.Label),
                    featureColumnName: "Features"));

        // ── 3. Entraînement ────────────────────────────────────────────────────
        Console.WriteLine("\n  [3/5] Entraînement...");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var model = pipeline.Fit(split.TrainSet);
        sw.Stop();
        Console.WriteLine($"  Modèle entraîné en {sw.ElapsedMilliseconds}ms");

        // ── 4. Évaluation ──────────────────────────────────────────────────────
        Console.WriteLine("\n  [4/5] Évaluation sur le test set...");
        var predictions = model.Transform(split.TestSet);
        var metrics     = ml.BinaryClassification.Evaluate(
            predictions,
            labelColumnName: nameof(SentimentInput.Label));
        TrainingHelper.PrintBinaryMetrics(metrics, "Sentiment");

        // ── 5. Inférence ───────────────────────────────────────────────────────
        Console.WriteLine("\n  [5/5] Prédictions sur de nouvelles phrases :");
        var engine = ml.Model.CreatePredictionEngine<SentimentInput, SentimentPrediction>(model);

        var testPhrases = new[]
        {
            "Ce produit dépasse toutes mes attentes !",
            "Jamais reçu ma commande, service nul",
            "Correct sans être exceptionnel",
            "La qualité est vraiment décevante",
            "Livraison le lendemain, parfait !",
        };

        Console.WriteLine();
        foreach (var phrase in testPhrases)
        {
            var pred  = engine.Predict(new SentimentInput { Text = phrase });
            var label = pred.Prediction ? "✅ POSITIF" : "❌ NÉGATIF";
            var color = pred.Prediction ? ConsoleColor.Green : ConsoleColor.Red;

            Console.ForegroundColor = color;
            Console.Write($"  {label}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" ({pred.Probability:P0})");
            Console.ResetColor();
            Console.WriteLine($"  \"{phrase}\"");
        }

        // Sauvegarder
        ml.Model.Save(model, data.Schema, "sentiment.zip");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  💾 Modèle sauvegardé → sentiment.zip");
        Console.ResetColor();

        await Task.CompletedTask;
    }
}
