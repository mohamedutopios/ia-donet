using Microsoft.ML;
using Microsoft.ML.Data;

namespace Module8.MlNet.Services;

/// <summary>
/// Service générique d'entraînement ML.NET.
/// Gère le cycle complet : load → pipeline → train → evaluate → save.
/// </summary>
public static class TrainingHelper
{
    /// <summary>Affiche les métriques d'une classification binaire.</summary>
    public static void PrintBinaryMetrics(CalibratedBinaryClassificationMetrics m, string label)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  📊 Évaluation — {label}");
        Console.ResetColor();

        var accColor  = m.Accuracy        >= 0.85 ? ConsoleColor.Green : ConsoleColor.Yellow;
        var f1Color   = m.F1Score         >= 0.80 ? ConsoleColor.Green : ConsoleColor.Yellow;
        var aucColor  = m.AreaUnderRocCurve >= 0.90 ? ConsoleColor.Green : ConsoleColor.Yellow;

        PrintMetric("Accuracy   ", $"{m.Accuracy:P1}",          accColor);
        PrintMetric("F1-Score   ", $"{m.F1Score:P1}",           f1Color);
        PrintMetric("AUC-ROC    ", $"{m.AreaUnderRocCurve:P1}", aucColor);
        PrintMetric("Precision  ", $"{m.PositivePrecision:P1}", ConsoleColor.Gray);
        PrintMetric("Recall     ", $"{m.PositiveRecall:P1}",    ConsoleColor.Gray);
    }

    /// <summary>Affiche les métriques d'une classification multi-classe.</summary>
    public static void PrintMulticlassMetrics(MulticlassClassificationMetrics m, string label)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  📊 Évaluation — {label}");
        Console.ResetColor();

        var accColor = m.MicroAccuracy >= 0.85 ? ConsoleColor.Green : ConsoleColor.Yellow;
        var f1Color  = m.MacroAccuracy >= 0.80 ? ConsoleColor.Green : ConsoleColor.Yellow;

        PrintMetric("Micro Accuracy", $"{m.MicroAccuracy:P1}", accColor);
        PrintMetric("Macro F1      ", $"{m.MacroAccuracy:P1}", f1Color);
        PrintMetric("Log-Loss      ", $"{m.LogLoss:F4}",       ConsoleColor.Gray);
    }

    /// <summary>Cross-validation binaire — retourne l'accuracy moyenne.</summary>
    public static double CrossValidateBinary<TInput>(
        MLContext ml,
        IDataView data,
        IEstimator<ITransformer> pipeline,
        int folds = 5)
        where TInput : class
    {
        var results = ml.BinaryClassification.CrossValidate(
            data, pipeline, numberOfFolds: folds);
        return results.Average(r => r.Metrics.Accuracy);
    }

    private static void PrintMetric(string name, string value, ConsoleColor color)
    {
        Console.Write($"    {name} : ");
        Console.ForegroundColor = color;
        Console.WriteLine(value);
        Console.ResetColor();
    }
}
