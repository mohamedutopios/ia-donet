namespace Module7.EmbeddingsRag.Services;

/// <summary>
/// Découpe les textes en chunks avec chevauchement.
/// </summary>
public class ChunkService
{
    private readonly RagOptions _opts;

    public ChunkService(RagOptions opts) => _opts = opts;

    /// <summary>Découpe un texte en chunks avec overlap.</summary>
    public IEnumerable<string> Chunk(string text)
    {
        text = NormalizeWhitespace(text);
        var chunks = new List<string>();
        int start  = 0;

        while (start < text.Length)
        {
            int end = Math.Min(start + _opts.ChunkSize, text.Length);

            // Couper sur un espace pour ne pas trancher un mot
            if (end < text.Length)
            {
                int lastSpace = text.LastIndexOf(' ', end);
                if (lastSpace > start) end = lastSpace;
            }

            var chunk = text[start..end].Trim();
            if (!string.IsNullOrWhiteSpace(chunk))
                chunks.Add(chunk);

            start = end - _opts.ChunkOverlap;
        }

        return chunks;
    }

    /// <summary>Lit et chunke tous les .txt d'un dossier.</summary>
    public IEnumerable<(string FileName, string ChunkText)> ChunkDirectory(string dir)
    {
        var files = Directory.GetFiles(dir, "*.txt")
            .Concat(Directory.GetFiles(dir, "*.md"))
            .ToArray();

        foreach (var file in files)
        {
            var text   = File.ReadAllText(file);
            var chunks = Chunk(text);
            foreach (var chunk in chunks)
                yield return (Path.GetFileName(file), chunk);
        }
    }

    private static string NormalizeWhitespace(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.Replace("\r\n", "\n"), @"\n{3,}", "\n\n").Trim();
}
