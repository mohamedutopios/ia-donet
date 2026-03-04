using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Module6.SemanticKernel.Plugins;

/// <summary>
/// Plugin RH — expose des fonctions de gestion des ressources humaines.
/// Le LLM appelle ces méthodes automatiquement selon le contexte.
/// </summary>
public class HrPlugin
{
    // Base de données simulée
    private static readonly List<Candidate> _candidates =
    [
        new("Alice Martin",  "C#, Azure, ASP.NET Core", 5, "Paris"),
        new("Bob Durand",    "C#, SQL Server, WPF",     3, "Lyon"),
        new("Claire Petit",  "Python, ML.NET, Azure",   7, "Paris"),
        new("David Morel",   "Java, Spring Boot, Docker",4, "Bordeaux"),
        new("Eva Rousseau",  "C#, Blazor, React",       2, "Nantes"),
        new("Franck Lambert","DevOps, Kubernetes, CI/CD",6, "Paris"),
    ];

    private static readonly Dictionary<string, int> _leavePolicies = new()
    {
        ["rtt"]            = 10,
        ["conges_payes"]   = 25,
        ["maladie"]        = 30,
        ["maternite"]      = 112,
        ["formation"]      = 5,
    };

    [KernelFunction]
    [Description("Recherche des candidats par compétence technique. Retourne leur nom, compétences, années d'expérience et ville.")]
    public string SearchCandidates(
        [Description("Compétence ou technologie recherchée, ex: 'C#', 'Azure', 'Docker'")]
        string skill)
    {
        var matches = _candidates
            .Where(c => c.Skills.Contains(skill, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!matches.Any())
            return $"Aucun candidat trouvé pour la compétence '{skill}'.";

        var lines = matches.Select(c =>
            $"• {c.Name} — {c.Skills} ({c.Years} ans exp.) — {c.City}");

        return $"Candidats pour '{skill}' ({matches.Count} résultats) :\n" +
               string.Join("\n", lines);
    }

    [KernelFunction]
    [Description("Retourne le nombre de jours de congés disponibles pour un type donné. Types valides : rtt, conges_payes, maladie, maternite, formation.")]
    public string GetLeavePolicy(
        [Description("Type de congé : rtt, conges_payes, maladie, maternite, formation")]
        string leaveType)
    {
        var key = leaveType.ToLower().Replace(" ", "_").Replace("é", "e").Replace("è", "e");

        return _leavePolicies.TryGetValue(key, out var days)
            ? $"Politique {leaveType} : {days} jours par an."
            : $"Type de congé '{leaveType}' inconnu. Types disponibles : {string.Join(", ", _leavePolicies.Keys)}.";
    }

    [KernelFunction]
    [Description("Retourne le profil complet d'un candidat par son prénom ou nom.")]
    public string GetCandidateProfile(
        [Description("Prénom ou nom du candidat")]
        string name)
    {
        var candidate = _candidates
            .FirstOrDefault(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        return candidate is null
            ? $"Candidat '{name}' introuvable."
            : $"Profil de {candidate.Name} :\n" +
              $"  Compétences : {candidate.Skills}\n" +
              $"  Expérience  : {candidate.Years} ans\n" +
              $"  Ville       : {candidate.City}";
    }

    private record Candidate(string Name, string Skills, int Years, string City);
}
