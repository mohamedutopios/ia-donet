# Formation IA .NET — Guide de démarrage Azure
## Utopios — Prérequis et configuration pas à pas

---

## 📋 Ce dont vous avez besoin

- Un **compte Azure** avec un abonnement actif  
  → [Créer un compte gratuit](https://azure.microsoft.com/fr-fr/free/) (200$ de crédit offert)
- **.NET 8 SDK** installé → [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Visual Studio Code** ou Visual Studio 2022

---

## ÉTAPE 1 — Créer une ressource Azure OpenAI

> ⚠️ Azure OpenAI est soumis à approbation. Si vous n'y avez pas encore accès, faites la demande ici : https://aka.ms/oai/access (délai 1-3 jours)

1. Connectez-vous sur **https://portal.azure.com**
2. Cliquez sur **"Créer une ressource"** (bouton `+` en haut à gauche)
3. Recherchez **"Azure OpenAI"** et cliquez sur **Créer**
4. Remplissez le formulaire :

| Champ | Valeur recommandée |
|-------|-------------------|
| Abonnement | Votre abonnement Azure |
| Groupe de ressources | `rg-formation-ia` (créer nouveau) |
| Région | **France Central** ou **Sweden Central** |
| Nom | `openai-formation-[votreprenom]` |
| Niveau tarifaire | **Standard S0** |

5. Cliquez sur **Vérifier + créer** puis **Créer**
6. Attendez la fin du déploiement (1-2 minutes) puis cliquez sur **Accéder à la ressource**

---

## ÉTAPE 2 — Récupérer l'Endpoint et la clé API

1. Dans votre ressource Azure OpenAI, cliquez sur **"Clés et point de terminaison"** (menu gauche)
2. Notez les valeurs suivantes :

```
Endpoint  : https://openai-formation-[votreprenom].openai.azure.com/
Clé 1     : xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx  (32 caractères)
```

> 💡 La clé 1 et la clé 2 fonctionnent toutes les deux. Utilisez la clé 1.

---

## ÉTAPE 3 — Créer un déploiement GPT-4o (modèle de chat)

> ⚠️ Le **nom du déploiement** est différent du nom du modèle. C'est ce nom que vous mettrez dans `appsettings.json`.

1. Dans votre ressource Azure OpenAI, cliquez sur **"Accéder à Azure OpenAI Studio"**  
   (ou allez sur https://oai.azure.com)
2. Dans le menu gauche, cliquez sur **"Deployments"** (Déploiements)
3. Cliquez sur **"+ Create new deployment"**
4. Remplissez :

| Champ | Valeur |
|-------|--------|
| Modèle | **gpt-4o** |
| Version du modèle | Dernière version disponible |
| Nom du déploiement | `gpt-4o` ← **notez ce nom exactement** |
| Tokens par minute | 10 000 (suffisant pour la formation) |

5. Cliquez sur **Créer**

> ℹ️ Si `gpt-4o` n'est pas disponible dans votre région, choisissez `gpt-4` ou `gpt-35-turbo` et adaptez le nom du déploiement en conséquence.

---

## ÉTAPE 4 — Créer un déploiement text-embedding (module 7 uniquement)

> Nécessaire uniquement pour le **Module 7 — Embeddings & RAG**

1. Dans Azure OpenAI Studio → **Deployments** → **+ Create new deployment**
2. Remplissez :

| Champ | Valeur |
|-------|--------|
| Modèle | **text-embedding-3-small** |
| Nom du déploiement | `text-embedding-3-small` ← **notez ce nom exactement** |
| Tokens par minute | 10 000 |

3. Cliquez sur **Créer**

---

## ÉTAPE 5 — Configurer appsettings.json dans chaque projet

Ouvrez le fichier `src/appsettings.json` du module que vous souhaitez utiliser et remplacez les valeurs :

```json
{
  "AzureOpenAI": {
    "Endpoint":            "https://openai-formation-[votreprenom].openai.azure.com/",
    "ApiKey":              "votre-cle-api-ici",
    "DeploymentChat":      "gpt-4o",
    "DeploymentEmbedding": "text-embedding-3-small"
  }
}
```

> ⚠️ **Erreurs fréquentes :**
> - `404 DeploymentNotFound` → Le champ `DeploymentChat` ne correspond pas au nom exact de votre déploiement dans Azure OpenAI Studio
> - `401 Unauthorized` → La clé API est incorrecte ou copiée avec des espaces
> - `403 Forbidden` → Votre abonnement n'a pas accès à Azure OpenAI (voir étape 1)

---

## ÉTAPE 6 — Lancer un projet

```bash
# Se placer dans le dossier src du module souhaité
cd module3-demo/src

# Restaurer les packages NuGet
dotnet restore

# Lancer le projet
dotnet run
```

---

## 🗺️ Récapitulatif des modules et dépendances Azure

| Module | Titre | DeploymentChat | DeploymentEmbedding | Clé requise |
|--------|-------|:--------------:|:-------------------:|:-----------:|
| 3 | Appeler l'IA depuis C# | ✅ | ❌ | ✅ |
| 4 | Prompt Engineering | ✅ | ❌ | ✅ |
| 5 | API IA avec ASP.NET Core | ✅ | ❌ | ✅ |
| 6 | Semantic Kernel | ✅ | ❌ | ✅ |
| 7 | Embeddings & RAG | ✅ | ✅ | ✅ |
| 8 | IA locale ML.NET | ❌ | ❌ | ❌ |
| 9 | Bonnes pratiques & Sécurité | ✅ | ❌ | ✅ |

> ✅ = requis  ❌ = non nécessaire  
> **Le module 8 fonctionne 100% en local, sans Azure.**

---

## 💰 Estimation des coûts Azure OpenAI

| Usage | Coût estimé |
|-------|-------------|
| Formation complète (modules 3-7, 9) | < 1 € |
| 1 000 requêtes GPT-4o | ~0,05 € |
| 1 000 embeddings text-embedding-3-small | < 0,01 € |

> Pour éviter les surprises, configurez une **alerte de budget** dans le portail Azure :  
> Accueil → Abonnements → Budgets → Ajouter → Seuil 10 € / alerte à 80 %

---

## 🛠️ Dépannage

### `404 DeploymentNotFound`
Le nom dans `DeploymentChat` ne correspond pas à votre déploiement Azure.  
→ Vérifiez dans Azure OpenAI Studio → Deployments et copiez le nom **exactement** (respectez la casse).

### `401 Unauthorized`
La clé API est incorrecte.  
→ Retournez dans le portail Azure → votre ressource → **Clés et point de terminaison** → copiez à nouveau la Clé 1.

### `429 TooManyRequests`
Quota de tokens par minute dépassé.  
→ Attendez quelques secondes et réessayez, ou augmentez le quota dans Azure OpenAI Studio → Deployments → votre déploiement → Edit → TPM.

### `dotnet : commande introuvable`
Le SDK .NET 8 n'est pas installé.  
→ Téléchargez-le sur https://dotnet.microsoft.com/download/dotnet/8.0

### `error CS1061 : Get introuvable`
Package NuGet manquant dans le `.csproj`.  
→ Ajoutez cette ligne dans le bloc `<ItemGroup>` du fichier `.csproj` :
```xml
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="8.0.0" />
```
