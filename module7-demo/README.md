# Module 7 — Embeddings & RAG
## Formation IA .NET — Utopios

Projet de démonstration complet : embeddings vectoriels, similarité cosinus, indexation de documents et chatbot RAG (Retrieval-Augmented Generation).

---

## 📋 Prérequis

- .NET 8 SDK
- Ressource **Azure OpenAI** avec :
  - Un déploiement `gpt-4o` (chat)
  - Un déploiement `text-embedding-3-small` (embeddings)

---

## ⚙️ Configuration

Éditez `src/appsettings.json` :

```json
{
  "AzureOpenAI": {
    "Endpoint":            "https://VOTRE-RESSOURCE.openai.azure.com/",
    "ApiKey":              "VOTRE_CLE_API",
    "DeploymentChat":      "gpt-4o",
    "DeploymentEmbedding": "text-embedding-3-small"
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

**Ordre recommandé :** `[1]` → `[2]` → `[3]` → `[4]`

---

## 📁 Structure du projet

```
src/
├── docs/                          ← Vos documents à indexer (.txt)
│   ├── aspnetcore-guide.txt
│   ├── azure-openai-docs.txt
│   └── semantic-kernel-guide.txt
├── Models/Models.cs               ← DocChunk, VectorIndex, SearchHit, RagAnswer
├── Services/
│   ├── EmbeddingService.cs        ← Génération d'embeddings + CosineSimilarity
│   ├── ChunkService.cs            ← Découpage avec overlap
│   └── RagService.cs              ← Pipeline RAG complet
├── Demos/
│   ├── Demo1_Embeddings.cs        ← Visualisation des vecteurs
│   ├── Demo2_Indexing.cs          ← Indexation du dossier docs/
│   ├── Demo3_Search.cs            ← Recherche sémantique
│   └── Demo4_RagChatbot.cs        ← Chatbot interactif
├── Options.cs
├── appsettings.json
└── Program.cs
```

---

## 🎯 Démos disponibles

| # | Démo | Concept | Ce que vous observez |
|---|------|---------|----------------------|
| 1 | **Embeddings & Cosinus** | Vecteurs + similarité | Dimensions, scores colorés, seuils |
| 2 | **Indexation**           | Chunking + embeddings batch | Découpe les .txt, génère les vecteurs, sauvegarde `index.json` |
| 3 | **Recherche sémantique** | Cosinus sur l'index | Top-K résultats par pertinence, scores affichés |
| 4 | **Chatbot RAG**          | Pipeline complet | Q&A libre sur vos documents, sources citées |

---

## 📄 Ajouter vos propres documents

Copiez vos fichiers `.txt` ou `.md` dans `src/docs/`, relancez la démo `[2]` pour réindexer, puis interrogez avec `[4]`.

---

## 💡 Concepts démontrés

- **Embeddings** — transformer du texte en vecteurs flottants via `EmbeddingClient`
- **Cosinus** — mesurer la proximité sémantique entre vecteurs
- **Chunking avec overlap** — découper les documents sans perdre le contexte
- **Index JSON local** — stocker et recharger les embeddings sans re-vectoriser
- **RAG pipeline** : vectoriser → rechercher → augmenter le prompt → générer
- **Sources citées** — réponse ancrée dans les documents, pas inventée
