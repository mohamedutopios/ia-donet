using Module8.MlNet.Demos;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("""
 ╔══════════════════════════════════════════════════╗
 ║   Module 8 — IA locale avec ML.NET               ║
 ║   Formation IA .NET — Utopios                    ║
 ╚══════════════════════════════════════════════════╝

 Aucune clé API requise — tout tourne en local !
 """);
Console.ResetColor();

Console.WriteLine(" Choisissez une démo :\n");
Console.WriteLine("  [1] Sentiment (Binaire)    — positif / négatif");
Console.WriteLine("  [2] Intentions (Multi-cl.) — commande / question / plainte");
Console.WriteLine("  [3] Spam (Comparaison)     — SDCA vs FastTree");
Console.WriteLine("  [4] Cross-Validation       — k-fold + sauvegarde/rechargement");
Console.WriteLine("  [5] Classifier Interactif  — 3 modèles simultanés");
Console.WriteLine("  [0] Démos 1 → 4 automatiques\n");
Console.Write(" Votre choix : ");

var choice = Console.ReadLine()?.Trim();

try
{
    switch (choice)
    {
        case "1": await new Demo1_SentimentAnalysis().RunAsync();    break;
        case "2": await new Demo2_IntentClassification().RunAsync(); break;
        case "3": await new Demo3_SpamDetection().RunAsync();        break;
        case "4": await new Demo4_CrossValidation().RunAsync();      break;
        case "5": await new Demo5_InteractiveClassifier().RunAsync(); break;
        case "0":
        default:
            await new Demo1_SentimentAnalysis().RunAsync();
            await new Demo2_IntentClassification().RunAsync();
            await new Demo3_SpamDetection().RunAsync();
            await new Demo4_CrossValidation().RunAsync();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ℹ️  Démo 5 (interactif) disponible via [5].");
            Console.ResetColor();
            break;
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n✅ Démo terminée.");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ Erreur : {ex.Message}");
    Console.ResetColor();
}
