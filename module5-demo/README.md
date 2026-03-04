# Module 5 — API IA avec ASP.NET Core
## Formation IA .NET — Utopios

API REST complète exposant des endpoints IA construits avec ASP.NET Core 8 : chat conversationnel, résumé de texte, injection de dépendances, validation des entrées, middleware d'erreurs global et documentation Swagger interactive.

---

## 📋 Prérequis

| Outil | Version | Vérification |
|-------|---------|-------------|
| .NET SDK | 8.0+ | `dotnet --version` |
| Azure OpenAI | Déploiement actif | Portail Azure → AI Studio |

Informations Azure nécessaires :
- **Endpoint** — ex : `https://mon-instance.openai.azure.com/`
- **API Key** — dans Azure AI Studio → Keys and Endpoint
- **Nom du déploiement** — le nom exact donné au modèle lors de sa création (ex : `gpt-4o`, `my-gpt4`)

> ⚠️ Le nom du déploiement n'est pas le nom du modèle. Vérifiez dans Azure AI Studio → **Deployments** le nom exact de votre déploiement.

---

## ⚙️ Configuration

Éditez `src/appsettings.json` :

```json
{
  "AzureOpenAI": {
    "Endpoint":       "https://VOTRE-RESSOURCE.openai.azure.com/",
    "ApiKey":         "VOTRE_CLE_API",
    "DeploymentChat": "VOTRE_NOM_DEPLOIEMENT"
  }
}
```

**Ne jamais commiter `appsettings.json` avec vos clés** — en production, utilisez des variables d'environnement ou Azure Key Vault :

```bash
# Alternative via variable d'environnement
export AzureOpenAI__ApiKey="votre-cle"
export AzureOpenAI__Endpoint="https://..."
export AzureOpenAI__DeploymentChat="gpt-4o"
```

---

## 🚀 Lancer le projet

```bash
cd src
dotnet restore
dotnet run
```

L'application démarre sur `http://localhost:5000`.
La **Swagger UI** est accessible directement à la racine : **http://localhost:5000**

Pour changer le port :
```bash
dotnet run --urls "http://localhost:7000"
```

---

## 📁 Structure du projet

```
src/
├── Controllers/
│   └── AiController.cs                ← 3 endpoints : POST /chat, POST /summarize, GET /health
├── Middleware/
│   └── ErrorHandlingMiddleware.cs     ← Gestion globale des erreurs (RFC 7807 Problem Details)
├── Models/
│   └── AiModels.cs                    ← DTOs : ChatRequest/Response, SummarizeRequest/Response
├── Services/
│   └── AiService.cs                   ← Interface IAiService + implémentation Azure OpenAI
├── AzureOpenAIOptions.cs              ← Classe de configuration typée
├── appsettings.json                   ← Configuration (Endpoint, ApiKey, Deployment)
└── Program.cs                         ← Bootstrap : DI, middleware pipeline, Swagger
```

---

## 🎯 Endpoints

### `POST /api/ai/chat`

Envoie un message à l'IA et retourne sa réponse avec les métriques de consommation.

**Requête :**
```json
{
  "message":     "Qu'est-ce que l'injection de dépendances ?",
  "temperature": 0.7,
  "maxTokens":   300
}
```

| Champ | Type | Contraintes | Défaut |
|-------|------|------------|--------|
| `message` | string | 1–4000 caractères | — |
| `temperature` | float | 0.0–2.0 | 0.7 |
| `maxTokens` | int | 1–2000 | 500 |

**Réponse 200 :**
```json
{
  "reply":        "L'injection de dépendances est un patron de conception...",
  "promptTokens": 45,
  "replyTokens":  120,
  "totalTokens":  165,
  "elapsedMs":    843
}
```

---

### `POST /api/ai/summarize`

Résume un texte en bullets, paragraphe ou JSON structuré.

**Requête :**
```json
{
  "text":      "Texte à résumer, minimum 50 caractères...",
  "maxPoints": 3,
  "format":    0
}
```

| Champ | Type | Contraintes | Défaut |
|-------|------|------------|--------|
| `text` | string | 50–8000 caractères | — |
| `maxPoints` | int | 1–10 | 3 |
| `format` | enum | `0`=Bullets, `1`=Paragraph, `2`=Json | 0 |

**Réponse 200 :**
```json
{
  "summary":     "• Point clé 1\n• Point clé 2\n• Point clé 3",
  "wordCount":   18,
  "totalTokens": 210,
  "elapsedMs":   621
}
```

---

### `GET /api/ai/health`

Vérifie que l'API est opérationnelle (sans appel Azure OpenAI).

**Réponse 200 :**
```json
{ "status": "ok", "timestamp": "2024-01-15T10:30:00Z" }
```

---

## ❌ Gestion des erreurs

Toutes les erreurs retournent un objet **Problem Details** (RFC 7807) :

```json
{
  "type":     "https://httpstatuses.com/401",
  "title":    "Clé API invalide",
  "status":   401,
  "detail":   "La clé API Azure OpenAI est incorrecte ou expirée.",
  "instance": "/api/ai/chat"
}
```

| Code HTTP | Cause | Solution |
|-----------|-------|---------|
| `400` | Champs invalides (message vide, temp > 2…) | Corriger la requête |
| `401` | Clé API incorrecte | Vérifier `ApiKey` dans appsettings.json |
| `404` | Déploiement introuvable | Vérifier `DeploymentChat` dans appsettings.json |
| `422` | Validation échouée | Vérifier les contraintes des champs |
| `429` | Quota Azure dépassé | Attendre ou augmenter le quota |
| `500` | Erreur interne | Voir les logs |

---

## 🧪 Tester l'API

### Option 1 — Swagger UI (recommandé)
Ouvrez **http://localhost:5000** dans votre navigateur. Tous les endpoints sont testables directement.

### Option 2 — Fichier `tests.http`
Ouvrez `tests.http` avec l'extension **REST Client** dans VSCode (clic sur `Send Request` au-dessus de chaque requête).

### Option 3 — curl

```bash
# Health check
curl http://localhost:5000/api/ai/health

# Chat
curl -X POST http://localhost:5000/api/ai/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"Explique les microservices en 3 points","temperature":0.7,"maxTokens":200}'

# Résumé en bullets
curl -X POST http://localhost:5000/api/ai/summarize \
  -H "Content-Type: application/json" \
  -d '{"text":"ASP.NET Core est un framework web open-source...","maxPoints":3,"format":0}'
```

---

## 💡 Concepts démontrés

### Injection de dépendances
```csharp
// Program.cs — enregistrement
builder.Services.AddScoped<IAiService, AiService>();

// AiController.cs — injection par constructeur
public AiController(IAiService ai, ILogger<AiController> logger) { ... }
```

### Configuration typée avec IOptions\<T\>
```csharp
// AzureOpenAIOptions.cs — classe de configuration
public class AzureOpenAIOptions { public string Endpoint ... }

// Program.cs — binding automatique depuis appsettings.json
builder.Services.Configure<AzureOpenAIOptions>(
    builder.Configuration.GetSection("AzureOpenAI"));

// AiService.cs — injection
public AiService(IOptions<AzureOpenAIOptions> opts) { ... }
```

### Validation automatique avec Data Annotations
```csharp
public class ChatRequest
{
    [Required]
    [MinLength(1), MaxLength(4000)]
    public string Message { get; set; }

    [Range(0.0, 2.0)]
    public float Temperature { get; set; } = 0.7f;
}
// ASP.NET Core retourne automatiquement un 400 si les contraintes ne sont pas respectées
```

### Middleware d'erreurs global (RFC 7807)
```csharp
// Program.cs — enregistré en premier dans le pipeline
app.UseMiddleware<ErrorHandlingMiddleware>();

// Intercepte toutes les exceptions non gérées
// et retourne un Problem Details standardisé
```
