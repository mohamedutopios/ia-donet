using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Module6.SemanticKernel.Plugins;

/// <summary>
/// Plugin Email — génère des mails RH types.
/// </summary>
public class EmailPlugin
{
    [KernelFunction]
    [Description("Génère un mail de convocation à un entretien d'embauche.")]
    public string DraftInterviewEmail(
        [Description("Prénom du candidat")]
        string candidateName,
        [Description("Date et heure de l'entretien, ex: 'lundi 15 janvier à 14h'")]
        string dateTime,
        [Description("Poste visé")]
        string position)
    {
        return $"""
            Objet : Convocation entretien — {position}

            Bonjour {candidateName},

            Suite à l'étude de votre candidature pour le poste de {position},
            nous avons le plaisir de vous convier à un entretien le {dateTime}.

            L'entretien se déroulera en visioconférence (lien envoyé séparément)
            et durera environ 45 minutes.

            Merci de confirmer votre disponibilité en répondant à ce message.

            Cordialement,
            L'équipe RH
            """;
    }

    [KernelFunction]
    [Description("Génère un mail de refus de candidature, poli et professionnel.")]
    public string DraftRejectionEmail(
        [Description("Prénom du candidat")]
        string candidateName,
        [Description("Poste visé")]
        string position)
    {
        return $"""
            Objet : Réponse à votre candidature — {position}

            Bonjour {candidateName},

            Nous vous remercions de l'intérêt que vous portez à notre entreprise
            et pour le temps consacré à votre candidature pour le poste de {position}.

            Après examen attentif de votre dossier, nous sommes au regret de vous
            informer que votre profil ne correspond pas aux critères recherchés
            pour ce poste à ce jour.

            Nous conservons votre candidature et ne manquerons pas de vous recontacter
            si une opportunité correspondant à votre profil se présente.

            Nous vous souhaitons plein succès dans vos recherches.

            Cordialement,
            L'équipe RH
            """;
    }

    [KernelFunction]
    [Description("Génère un mail de bienvenue pour un nouvel employé.")]
    public string DraftWelcomeEmail(
        [Description("Prénom du nouvel employé")]
        string employeeName,
        [Description("Date de début de contrat")]
        string startDate,
        [Description("Nom du manager")]
        string managerName)
    {
        return $"""
            Objet : Bienvenue dans l'équipe !

            Bonjour {employeeName},

            Toute l'équipe est ravie de vous accueillir à partir du {startDate}.

            Votre manager, {managerName}, vous contactera prochainement
            pour organiser votre première journée et vous présenter l'équipe.

            En attendant, n'hésitez pas à nous contacter pour toute question.

            À très bientôt !
            L'équipe RH
            """;
    }
}
