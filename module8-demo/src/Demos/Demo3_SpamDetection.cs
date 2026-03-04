using Microsoft.ML;
using Module8.MlNet.Models;
using Module8.MlNet.Services;

namespace Module8.MlNet.Demos;

/// <summary>
/// Démo 3 — Détection de spam : comparaison de 2 algorithmes
/// </summary>
public class Demo3_SpamDetection
{
    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 3 : Détection de Spam — Comparaison d'algorithmes ─");
        Console.ResetColor();

        var ml   = new MLContext(seed: 42);
        var path = Path.Combine(AppContext.BaseDirectory, "data", "spam.csv");

        var data  = ml.Data.LoadFromTextFile<SpamInput>(path, hasHeader: true, separatorChar: ',');
        var split = ml.Data.TrainTestSplit(data, testFraction: 0.25);

        // Features communes
        var featurePipeline = ml.Transforms.Text
            .FeaturizeText("Features", nameof(SpamInput.Text));

        // ── Algorithme 1 : SDCA ──────────────────────────────────────────────
        Console.WriteLine("\n  [1/2] Entraînement SDCA Logistic Regression...");
        var sdcaPipeline = featurePipeline
            .Append(ml.BinaryClassification.Trainers
                .SdcaLogisticRegression(
                    labelColumnName:   nameof(SpamInput.IsSpam),
                    featureColumnName: "Features"));

        var sdcaModel = sdcaPipeline.Fit(split.TrainSet);
        var sdcaPreds = sdcaModel.Transform(split.TestSet);
        var sdcaM     = ml.BinaryClassification.Evaluate(sdcaPreds, nameof(SpamInput.IsSpam));
        TrainingHelper.PrintBinaryMetrics(sdcaM, "SDCA Logistic Regression");

        // ── Algorithme 2 : FastTree ──────────────────────────────────────────
        Console.WriteLine("\n  [2/2] Entraînement FastTree (arbres de décision)...");
        var ftPipeline = featurePipeline
            .Append(ml.BinaryClassification.Trainers
                .FastTree(
                    labelColumnName:   nameof(SpamInput.IsSpam),
                    featureColumnName: "Features",
                    numberOfLeaves:    20,
                    numberOfTrees:     50));

        var ftModel = ftPipeline.Fit(split.TrainSet);
        var ftPreds = ftModel.Transform(split.TestSet);
        var ftM     = ml.BinaryClassification.Evaluate(ftPreds, nameof(SpamInput.IsSpam));
        TrainingHelper.PrintBinaryMetrics(ftM, "FastTree (Decision Trees)");

        // ── Comparaison ──────────────────────────────────────────────────────
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n  ══ Comparaison ══════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine($"  SDCA Accuracy : {sdcaM.Accuracy:P1}  |  F1 : {sdcaM.F1Score:P1}");
        Console.WriteLine($"  FastTree Acc  : {ftM.Accuracy:P1}  |  F1 : {ftM.F1Score:P1}");

        // Choisir le meilleur
        var bestModel  = ftM.F1Score >= sdcaM.F1Score ? ftModel  : sdcaModel;
        var bestName   = ftM.F1Score >= sdcaM.F1Score ? "FastTree" : "SDCA";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  ✅ Meilleur modèle : {bestName}");
        Console.ResetColor();

        // ── Inférence avec le meilleur ────────────────────────────────────────
        Console.WriteLine("\n  Prédictions spam :");
        var engine = ml.Model.CreatePredictionEngine<SpamInput, SpamPrediction>(bestModel);

        var testMails = new[]
        {
            "Réunion de projet reportée au vendredi 14h",
            "GAGNEZ 5000€ EN CLIQUANT ICI MAINTENANT !!!",
            "Votre facture du mois est disponible en ligne",
            "Offre exclusive : voiture gratuite pour les 10 premiers",
            "Bonjour, pouvez-vous me rappeler ce matin ?",
        };

        Console.WriteLine();
        foreach (var mail in testMails)
        {
            var pred  = engine.Predict(new SpamInput { Text = mail });
            var label = pred.IsSpam ? "🔴 SPAM   " : "✅ LÉGITIME";
            var color = pred.IsSpam ? ConsoleColor.Red : ConsoleColor.Green;

            Console.ForegroundColor = color;
            Console.Write($"  {label}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {pred.Probability:P0}  ");
            Console.ResetColor();
            var preview = mail.Length > 55 ? mail[..55] + "…" : mail;
            Console.WriteLine($"\"{preview}\"");
        }

        await Task.CompletedTask;
    }
}
