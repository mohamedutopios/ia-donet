using Module6.SemanticKernel.Services;

namespace Module6.SemanticKernel.Demos;

/// <summary>
/// Démo 4 — Assistant RH interactif : boucle de conversation libre.
/// </summary>
public class Demo4_InteractiveAssistant
{
    private readonly HrAssistantService _assistant;

    public Demo4_InteractiveAssistant(HrAssistantService assistant)
        => _assistant = assistant;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 4 : Assistant RH Interactif ─────────────────────");
        Console.ResetColor();

        Console.WriteLine("""

  Exemples de questions :
    - "Cherche des candidats en Docker"
    - "Combien de jours de congés maternité ?"
    - "Planifie un entretien avec Bob Durand le 10 avril à 9h"
    - "Rédige un mail de refus pour Eva Rousseau pour le poste de Dev C#"
    - "Donne-moi le profil complet de Claire Petit"
    - "reset" pour vider l'historique
    - "exit"  pour quitter

""");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  👤 [{_assistant.HistoryCount} msgs] > ");
            Console.ResetColor();

            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            if (input.Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                _assistant.Reset();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  🔄 Historique réinitialisé.\n");
                Console.ResetColor();
                continue;
            }

            try
            {
                var reply = await _assistant.ChatAsync(input);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  🤖 {reply}\n");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ❌ Erreur : {ex.Message}\n");
                Console.ResetColor();
            }
        }
    }
}
