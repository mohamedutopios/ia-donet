# Module 6 — Semantic Kernel
## Formation IA .NET — Utopios

Projet de démonstration complet de **Semantic Kernel** : plugins natifs, auto function calling, historique multi-tour et assistant RH interactif.

---

## 📋 Prérequis

- .NET 8 SDK
- Ressource **Azure OpenAI** avec un déploiement `gpt-4o`
- Endpoint et API Key Azure

---

## ⚙️ Configuration

Éditez `src/appsettings.json` :

```json
{
  "AzureOpenAI": {
    "Endpoint":       "https://VOTRE-RESSOURCE.openai.azure.com/",
    "ApiKey":         "VOTRE_CLE_API",
    "DeploymentChat": "gpt-4o"
  }
}
```

---

## 🚀 Lancer

```bash
cd src
dotnet restore
dotnet run
```

---

## 📁 Structure du projet

```
src/
├── Plugins/
│   ├── HrPlugin.cs          ← SearchCandidates, GetLeavePolicy, GetCandidateProfile
│   ├── EmailPlugin.cs       ← DraftInterviewEmail, DraftRejectionEmail, DraftWelcomeEmail
│   └── CalendarPlugin.cs    ← CreateEvent, ListEvents, DeleteEvent
├── Services/
│   └── HrAssistantService.cs ← Kernel + plugins + ChatHistory
├── Demos/
│   ├── Demo1_KernelMinimal.cs
│   ├── Demo2_Plugins.cs
│   ├── Demo3_MultiTurn.cs
│   └── Demo4_InteractiveAssistant.cs
├── AzureOpenAIOptions.cs
├── appsettings.json
└── Program.cs
```

---

## 🎯 Démos disponibles

| # | Fichier | Concept | Ce que vous observez |
|---|---------|---------|----------------------|
| 1 | `Demo1_KernelMinimal.cs`        | **Kernel + templates**     | `InvokePromptAsync` avec `{{$variable}}` sur 3 sujets |
| 2 | `Demo2_Plugins.cs`              | **Auto function calling**  | Le LLM choisit seul HrPlugin ou EmailPlugin selon la question |
| 3 | `Demo3_MultiTurn.cs`            | **Multi-tour + mémoire**   | 5 messages enchaînés, le LLM se souvient des réponses précédentes |
| 4 | `Demo4_InteractiveAssistant.cs` | **Assistant interactif**   | Boucle libre — testez vos propres questions |

---

## 🔌 Plugins disponibles

### HrPlugin
| Fonction | Description |
|----------|-------------|
| `SearchCandidates(skill)` | Recherche par compétence |
| `GetLeavePolicy(leaveType)` | RTT, congés payés, maladie… |
| `GetCandidateProfile(name)` | Profil complet d'un candidat |

### EmailPlugin
| Fonction | Description |
|----------|-------------|
| `DraftInterviewEmail(name, date, position)` | Mail de convocation |
| `DraftRejectionEmail(name, position)` | Mail de refus |
| `DraftWelcomeEmail(name, startDate, manager)` | Mail de bienvenue |

### CalendarPlugin
| Fonction | Description |
|----------|-------------|
| `CreateEvent(title, dateTime, duration, participants)` | Créer un événement |
| `ListEvents()` | Voir l'agenda |
| `DeleteEvent(eventId)` | Supprimer un événement |

---

## 💡 Concepts démontrés

- `Kernel.CreateBuilder()` — construction et configuration du Kernel
- `[KernelFunction]` + `[Description]` — exposition de méthodes C# au LLM
- `FunctionChoiceBehavior.Auto()` — orchestration automatique des plugins
- `ChatHistory` — maintien du contexte multi-tour
- Séparation `IAiService` / `HrAssistantService` — architecture propre
