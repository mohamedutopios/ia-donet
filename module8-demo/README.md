# Module 8 — IA locale avec ML.NET
## Formation IA .NET — Utopios

> ⚡ **Aucune clé API requise** — tout tourne 100% en local, sans Azure, sans réseau.

Projet de démonstration complet de ML.NET : classification binaire, multi-classe, comparaison d'algorithmes, cross-validation et classifier interactif.

---

## 📋 Prérequis

- .NET 8 SDK uniquement
- **Aucune clé Azure OpenAI** n'est nécessaire

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
├── data/
│   ├── sentiment.csv       ← 30 avis produits labellisés (positif/négatif)
│   ├── intentions.csv      ← 24 phrases labellisées (commande/question/plainte)
│   └── spam.csv            ← 20 emails labellisés (spam/légitime)
├── Models/Models.cs        ← SentimentInput/Prediction, IntentInput/Prediction, SpamInput/Prediction
├── Services/
│   └── TrainingHelper.cs   ← Affichage des métriques (binaire + multi-classe)
├── Demos/
│   ├── Demo1_SentimentAnalysis.cs
│   ├── Demo2_IntentClassification.cs
│   ├── Demo3_SpamDetection.cs
│   ├── Demo4_CrossValidation.cs
│   └── Demo5_InteractiveClassifier.cs
└── Program.cs
```

---

## 🎯 Démos disponibles

| # | Démo | Algorithme | Ce que vous observez |
|---|------|-----------|----------------------|
| 1 | **Sentiment** | SDCA Logistic Regression | Accuracy, F1, prédictions avec probabilités |
| 2 | **Intentions** | SDCA MaximumEntropy | 3 classes, confidence score par catégorie |
| 3 | **Spam** | SDCA **vs** FastTree | Comparaison métriques, meilleur modèle sélectionné auto |
| 4 | **Cross-Validation** | SDCA + k-fold | 5 folds, écart-type, sauvegarde + rechargement `.zip` |
| 5 | **Interactif** | 3 modèles simultanés | Tapez n'importe quelle phrase → 3 classifications en temps réel |

---

## 🔌 Ajouter vos propres données

Enrichissez les CSV dans `src/data/` pour améliorer les modèles. Règle : minimum 10 exemples par classe pour des résultats fiables. Plus il y a d'exemples, meilleure sera la précision.

---

## 💡 Concepts démontrés

- `MLContext` — point d'entrée unique, seed reproductible
- `FeaturizeText` — TF-IDF automatique sans tokenisation manuelle
- `MapValueToKey` / `MapKeyToValue` — encodage des labels multi-classe
- `TrainTestSplit` — 80/20 reproducible
- `CrossValidate` — évaluation k-fold fiable
- `Model.Save` / `Model.Load` — persistance du modèle en `.zip`
- `PredictionEngine<T>` — inférence unitaire type-safe
