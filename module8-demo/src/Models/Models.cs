using Microsoft.ML.Data;

namespace Module8.MlNet.Models;

// ── Sentiment (binaire) ────────────────────────────────────────────────────────

public class SentimentInput
{
    [LoadColumn(0)] public string Text  { get; set; } = string.Empty;
    [LoadColumn(1)] public bool   Label { get; set; }
}

public class SentimentPrediction
{
    [ColumnName("PredictedLabel")] public bool   Prediction { get; set; }
    public float   Probability { get; set; }
    public float   Score       { get; set; }
}

// ── Intention (multi-classe) ───────────────────────────────────────────────────

public class IntentInput
{
    [LoadColumn(0)] public string Text  { get; set; } = string.Empty;
    [LoadColumn(1)] public string Label { get; set; } = string.Empty;
}

public class IntentPrediction
{
    [ColumnName("PredictedLabel")] public string   Category   { get; set; } = string.Empty;
    public float[]  Score      { get; set; } = [];
    public float    Confidence => Score.Length > 0 ? Score.Max() : 0f;
}

// ── Spam (binaire avec features supplémentaires) ──────────────────────────────

public class SpamInput
{
    [LoadColumn(0)] public string Text   { get; set; } = string.Empty;
    [LoadColumn(1)] public bool   IsSpam { get; set; }
}

public class SpamPrediction
{
    [ColumnName("PredictedLabel")] public bool  IsSpam      { get; set; }
    public float Probability { get; set; }
}
