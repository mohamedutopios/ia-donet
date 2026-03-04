using Module7.EmbeddingsRag.Services;

namespace Module7.EmbeddingsRag.Demos;

/// <summary>
/// Démo 4 — Chatbot RAG interactif : pose des questions sur tes documents.
/// </summary>
public class Demo4_RagChatbot
{
    private readonly RagService _rag;

    public Demo4_RagChatbot(RagService rag) => _rag = rag;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 4 : Chatbot RAG Interactif ──────────────────────");
        Console.ResetColor();

        await _rag.LoadIndexAsync();

        Console.WriteLine("""

  Posez des questions sur vos documents indexés.
  Commandes :
    "exit"   → quitter
    "search" → voir les chunks trouvés pour la prochaine question

""");

        bool showSearch = false;

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("  ❓ > ");
            Console.ResetColor();

            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) continue;
            if (input.Equals("exit",   StringComparison.OrdinalIgnoreCase)) break;
            if (input.Equals("search", StringComparison.OrdinalIgnoreCase))
            {
                showSearch = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Mode debug activé : les chunks seront affichés.\n");
                Console.ResetColor();
                continue;
            }

            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                // Afficher les chunks si mode debug
                if (showSearch)
                {
                    var hits = await _rag.SearchAsync(input);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"\n  [{hits.Count} chunks trouvés]");
                    foreach (var h in hits)
                        Console.WriteLine($"  [{h.Similarity:F3}] {h.Chunk.Source} — {h.Chunk.Text[..Math.Min(80, h.Chunk.Text.Length)]}...");
                    Console.ResetColor();
                    showSearch = false;
                }

                var result = await _rag.AskAsync(input);
                sw.Stop();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  🤖 {result.Answer}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n  Sources : {string.Join(", ", result.Sources)}");
                Console.WriteLine($"  Scores  : {string.Join(", ", result.Similarities.Select(s => $"{s:F3}"))}");
                Console.WriteLine($"  Temps   : {sw.ElapsedMilliseconds}ms\n");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  ❌ {ex.Message}\n");
                Console.ResetColor();
            }
        }
    }
}
