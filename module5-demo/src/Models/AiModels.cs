using System.ComponentModel.DataAnnotations;

namespace Module5.ApiIA.Models;

// ── /api/ai/chat ──────────────────────────────────────────────────────────────

public class ChatRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Le message ne peut pas être vide.")]
    [MaxLength(4000, ErrorMessage = "Le message ne peut pas dépasser 4000 caractères.")]
    public string Message { get; set; } = string.Empty;

    [Range(0.0, 2.0, ErrorMessage = "La température doit être entre 0 et 2.")]
    public float Temperature { get; set; } = 0.7f;

    [Range(1, 2000, ErrorMessage = "MaxTokens doit être entre 1 et 2000.")]
    public int MaxTokens { get; set; } = 300;
}

public class ChatResponse
{
    public string Reply          { get; set; } = string.Empty;
    public int    PromptTokens   { get; set; }
    public int    ReplyTokens    { get; set; }
    public int    TotalTokens    { get; set; }
    public long   ElapsedMs      { get; set; }
}

// ── /api/ai/summarize ─────────────────────────────────────────────────────────

public class SummarizeRequest
{
    [Required]
    [MinLength(50, ErrorMessage = "Le texte doit faire au moins 50 caractères.")]
    [MaxLength(8000, ErrorMessage = "Le texte ne peut pas dépasser 8000 caractères.")]
    public string Text { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Le nombre de points doit être entre 1 et 10.")]
    public int MaxPoints { get; set; } = 3;

    public SummaryFormat Format { get; set; } = SummaryFormat.Bullets;
}

public enum SummaryFormat { Bullets, Paragraph, Json }

public class SummarizeResponse
{
    public string   Summary      { get; set; } = string.Empty;
    public int      WordCount    { get; set; }
    public int      TotalTokens  { get; set; }
    public long     ElapsedMs    { get; set; }
}
