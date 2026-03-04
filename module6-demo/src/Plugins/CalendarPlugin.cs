using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Module6.SemanticKernel.Plugins;

/// <summary>
/// Plugin Calendrier — simule la gestion d'événements.
/// </summary>
public class CalendarPlugin
{
    // Agenda simulé
    private static readonly List<CalendarEvent> _events = [];

    [KernelFunction]
    [Description("Crée un événement dans le calendrier RH.")]
    public string CreateEvent(
        [Description("Titre de l'événement")]
        string title,
        [Description("Date et heure, ex: '2024-02-15 14:00'")]
        string dateTime,
        [Description("Durée en minutes")]
        int durationMinutes,
        [Description("Participants (noms séparés par des virgules)")]
        string participants)
    {
        var ev = new CalendarEvent(
            Id:           Guid.NewGuid().ToString("N")[..8].ToUpper(),
            Title:        title,
            DateTime:     dateTime,
            Duration:     durationMinutes,
            Participants: participants);

        _events.Add(ev);

        return $"✅ Événement créé :\n" +
               $"  ID          : {ev.Id}\n" +
               $"  Titre       : {ev.Title}\n" +
               $"  Date/heure  : {ev.DateTime}\n" +
               $"  Durée       : {ev.Duration} min\n" +
               $"  Participants: {ev.Participants}";
    }

    [KernelFunction]
    [Description("Liste tous les événements planifiés dans le calendrier.")]
    public string ListEvents()
    {
        if (!_events.Any())
            return "Aucun événement planifié.";

        var lines = _events.Select(e =>
            $"  [{e.Id}] {e.Title} — {e.DateTime} ({e.Duration} min) — {e.Participants}");

        return $"Événements ({_events.Count}) :\n" + string.Join("\n", lines);
    }

    [KernelFunction]
    [Description("Supprime un événement par son ID.")]
    public string DeleteEvent(
        [Description("ID de l'événement à supprimer")]
        string eventId)
    {
        var ev = _events.FirstOrDefault(e =>
            e.Id.Equals(eventId, StringComparison.OrdinalIgnoreCase));

        if (ev is null)
            return $"Événement '{eventId}' introuvable.";

        _events.Remove(ev);
        return $"✅ Événement '{ev.Title}' supprimé.";
    }

    private record CalendarEvent(
        string Id, string Title, string DateTime, int Duration, string Participants);
}
