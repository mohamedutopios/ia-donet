# Module 4 — Prompt Engineering
## Formation IA .NET — Utopios

Projet de démonstration des techniques fondamentales de Prompt Engineering en C#.

---

## 📋 Prérequis

### 1. Installer .NET 8 SDK
Téléchargez et installez le SDK .NET 8 :  
→ https://dotnet.microsoft.com/download/dotnet/8.0

Vérifiez l'installation :
```bash
dotnet --version
# Doit afficher 8.x.x
```

### 2. Avoir une ressource Azure OpenAI configurée
Si ce n'est pas encore fait, suivez le **GUIDE-AZURE-SETUP.md** fourni avec la formation. En résumé :

1. Créez une ressource **Azure OpenAI** sur https://portal.azure.com
2. Récupérez votre **Endpoint** et votre **Clé API** :  
   → Portail Azure → votre ressource → **Clés et point de terminaison**
3. Créez un déploiement **gpt-4o** dans Azure OpenAI Studio (https://oai.azure.com)  
   → Menu gauche : **Deployments** → **+ Create new deployment**  
   → Notez le **nom exact** du déploiement (ex: `gpt-4o`, `mon-gpt4`, etc.)

---

## 📁 Structure du projet

Après avoir dézippé `module4-demo.zip`, vous obtenez :

```
module4-demo/
├── README.md               ← ce fichier
└── src/
    ├── Module4.PromptEngineering.Demo.csproj
    ├── appsettings.json    ← À CONFIGURER
    ├── AzureOpenAIOptions.cs
    ├── AiClient.cs
    ├── Demo1_SystemPrompt.cs
    ├── Demo2_Temperature.cs
    ├── Demo3_Tokens.cs
    ├── Demo4_FewShot.cs
    ├── Demo5_JsonStructured.cs
    ├── Demo6_ChainOfThought.cs
    └── Program.cs
```

---

## ⚙️ Configuration

### Étape 1 — Ouvrir le fichier de configuration

Ouvrez le fichier `src/appsettings.json` avec votre éditeur et remplacez les 3 valeurs :

```json
{
  "AzureOpenAI": {
    "Endpoint":       "https://VOTRE-RESSOURCE.openai.azure.com/",
    "ApiKey":         "VOTRE_CLE_API",
    "DeploymentChat": "gpt-4o"
  }
}
```

| Champ | Où le trouver | Exemple |
|-------|--------------|---------|
| `Endpoint` | Portail Azure → ressource → Clés et point de terminaison | `https://openai-mohamed.openai.azure.com/` |
| `ApiKey` | Portail Azure → ressource → Clés et point de terminaison → Clé 1 | `a1b2c3d4e5f6...` |
| `DeploymentChat` | Azure OpenAI Studio → Deployments → colonne "Name" | `gpt-4o` |

> ⚠️ **Point critique** : `DeploymentChat` doit correspondre au nom **exact** de votre déploiement dans Azure OpenAI Studio, pas au nom du modèle. Copiez-le depuis la liste des déploiements pour éviter toute erreur de casse.

### Étape 2 — Vérifier le fichier .csproj

Ouvrez `src/Module4.PromptEngineering.Demo.csproj` et assurez-vous que le package `Configuration.Binder` est bien présent :

```xml
<ItemGroup>
  <PackageReference Include="Azure.AI.OpenAI"                          Version="1.0.0-beta.17" />
  <PackageReference Include="Microsoft.Extensions.Configuration"       Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Configuration.Json"  Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="8.0.0" />
</ItemGroup>
```

Si la dernière ligne manque, ajoutez-la avant de continuer.

---

## 🚀 Lancer le projet

```bash
# 1. Se placer dans le dossier src
cd module4-demo/src

# 2. Restaurer les packages NuGet (télécharge les dépendances)
dotnet restore

# 3. Lancer le projet
dotnet run
```

Le menu s'affiche, choisissez une démo en tapant son numéro :

```
[1] System Prompt     — impact de 3 personnalités différentes
[2] Température       — déterminisme vs créativité
[3] Tokens & Coût     — max_tokens et estimation financière
[4] Few-Shot          — zero-shot vs 3-shot sur classification
[5] JSON Structuré    — sortie typée avec retry automatique
[6] Chain of Thought  — raisonnement étape par étape
[0] Toutes les démos
```

---

## 🎯 Démos disponibles

| # | Fichier | Concept | Ce que vous observez |
|---|---------|---------|----------------------|
| 1 | `Demo1_SystemPrompt.cs`   | **System Prompt**    | 3 personnalités → 3 styles de réponse radicalement différents |
| 2 | `Demo2_Temperature.cs`    | **Température**      | Temp=0 → déterministe, Temp=1.2 → créatif |
| 3 | `Demo3_Tokens.cs`         | **Tokens & Coût**    | max_tokens 50/150/400 + estimation financière |
| 4 | `Demo4_FewShot.cs`        | **Few-Shot**         | Zero-shot vs 3-shot sur classification de sentiment |
| 5 | `Demo5_JsonStructured.cs` | **JSON + Retry**     | Sortie JSON typée, validation, retry automatique |
| 6 | `Demo6_ChainOfThought.cs` | **Chain of Thought** | Raisonnement étape par étape vs réponse directe |

Entrez `0` pour enchaîner toutes les démos automatiquement.

---

## 🛠️ Dépannage

### `404 DeploymentNotFound`
```
❌ Erreur : Response status code does not indicate success: 404 (DeploymentNotFound)
```
→ Le champ `DeploymentChat` dans `appsettings.json` ne correspond pas au nom de votre déploiement.  
→ Allez sur https://oai.azure.com → **Deployments** → copiez le nom dans la colonne **Name**.

### `401 Unauthorized`
```
❌ Erreur : Response status code does not indicate success: 401 (Unauthorized)
```
→ La clé API est incorrecte ou contient des espaces.  
→ Retournez dans le portail Azure → votre ressource → **Clés et point de terminaison** → copiez à nouveau la Clé 1.

### `error CS1061 : 'IConfigurationSection' does not contain a definition for 'Get'`
→ Le package `Microsoft.Extensions.Configuration.Binder` est manquant dans le `.csproj`.  
→ Ajoutez la ligne suivante dans le bloc `<ItemGroup>` de `Module4.PromptEngineering.Demo.csproj` :
```xml
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="8.0.0" />
```
Puis relancez `dotnet restore` et `dotnet run`.

### `429 TooManyRequests`
```
❌ Erreur : Response status code does not indicate success: 429 (TooManyRequests)
```
→ Quota de tokens par minute dépassé.  
→ Attendez 30 secondes et réessayez.  
→ Ou augmentez le quota : Azure OpenAI Studio → Deployments → votre déploiement → **Edit** → augmentez les **Tokens per Minute**.

---

## 💡 Points clés du module

- Le **system prompt** est le levier le plus puissant — il définit le comportement global
- **Température 0** pour les tâches déterministes (JSON, calcul, classification)
- **Few-shot** améliore la cohérence du format et la précision
- **Chain of Thought** réduit les erreurs sur les problèmes complexes
- Toujours **valider** la sortie JSON et prévoir un **retry**
