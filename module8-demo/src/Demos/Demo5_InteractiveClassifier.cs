using Microsoft.ML;
using Module8.MlNet.Models;

namespace Module8.MlNet.Demos;

/// <summary>
/// Démo 5 — Classifier interactif : l'utilisateur tape des phrases et voit les prédictions en temps réel.
/// </summary>
public class Demo5_InteractiveClassifier
{
    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 5 : Classifier Interactif ───────────────────────");
        Console.ResetColor();

        var ml = new MLContext(seed: 42);

        // ── Entraîner les 3 modèles ────────────────────────────────────────────
        Console.WriteLine("\n  Entraînement des 3 modèles en cours...");

        var sentimentEngine = BuildSentimentEngine(ml);
        var intentEngine    = BuildIntentEngine(ml);
        var spamEngine      = BuildSpamEngine(ml);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  ✅ 3 modèles prêts : Sentiment | Intent | Spam\n");
        Console.ResetColor();

        Console.WriteLine("""
  Tapez une phrase pour la classifier selon les 3 modèles.
  Commandes : "exit" pour quitter.
""");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("  📝 > ");
            Console.ResetColor();

            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            Console.WriteLine();

            // Sentiment
            var sent = sentimentEngine.Predict(new SentimentInput { Text = input });
            Console.Write("  Sentiment : ");
            Console.ForegroundColor = sent.Prediction ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write(sent.Prediction ? "POSITIF" : "NÉGATIF");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  ({sent.Probability:P0})");
            Console.ResetColor();

            // Intent
            var intent = intentEngine.Predict(new IntentInput { Text = input });
            var intentColors = new Dictionary<string, ConsoleColor>
            {
                ["commande"] = ConsoleColor.Green,
                ["question"] = ConsoleColor.Cyan,
                ["plainte"]  = ConsoleColor.Red,
            };
            Console.Write("  Intention : ");
            Console.ForegroundColor = intentColors.GetValueOrDefault(intent.Category, ConsoleColor.Gray);
            Console.Write(intent.Category.ToUpper());
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  ({intent.Confidence:P0})");
            Console.ResetColor();

            // Spam
            var spam = spamEngine.Predict(new SpamInput { Text = input });
            Console.Write("  Spam      : ");
            Console.ForegroundColor = spam.IsSpam ? ConsoleColor.Red : ConsoleColor.Green;
            Console.Write(spam.IsSpam ? "OUI 🔴" : "NON ✅");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  ({spam.Probability:P0})");
            Console.ResetColor();
            Console.WriteLine();
        }

        await Task.CompletedTask;
    }

    private static PredictionEngine<SentimentInput, SentimentPrediction> BuildSentimentEngine(MLContext ml)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "sentiment.csv");
        var data = ml.Data.LoadFromTextFile<SentimentInput>(path, hasHeader: true, separatorChar: ',');
        var pipeline = ml.Transforms.Text.FeaturizeText("Features", nameof(SentimentInput.Text))
            .Append(ml.BinaryClassification.Trainers.SdcaLogisticRegression(
                nameof(SentimentInput.Label), "Features"));
        var model = pipeline.Fit(data);
        return ml.Model.CreatePredictionEngine<SentimentInput, SentimentPrediction>(model);
    }

    private static PredictionEngine<IntentInput, IntentPrediction> BuildIntentEngine(MLContext ml)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "intentions.csv");
        var data = ml.Data.LoadFromTextFile<IntentInput>(path, hasHeader: true, separatorChar: ',');
        var pipeline = ml.Transforms.Text.FeaturizeText("Features", nameof(IntentInput.Text))
            .Append(ml.Transforms.Conversion.MapValueToKey("Label", nameof(IntentInput.Label)))
            .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
        var model = pipeline.Fit(data);
        return ml.Model.CreatePredictionEngine<IntentInput, IntentPrediction>(model);
    }

    private static PredictionEngine<SpamInput, SpamPrediction> BuildSpamEngine(MLContext ml)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "spam.csv");
        var data = ml.Data.LoadFromTextFile<SpamInput>(path, hasHeader: true, separatorChar: ',');
        var pipeline = ml.Transforms.Text.FeaturizeText("Features", nameof(SpamInput.Text))
            .Append(ml.BinaryClassification.Trainers.SdcaLogisticRegression(
                nameof(SpamInput.IsSpam), "Features"));
        var model = pipeline.Fit(data);
        return ml.Model.CreatePredictionEngine<SpamInput, SpamPrediction>(model);
    }
}
