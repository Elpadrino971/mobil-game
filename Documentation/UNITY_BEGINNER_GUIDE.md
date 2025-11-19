# 🎮 Guide Unity Pour Débutants Complets

## 📥 ÉTAPE 1 : Installer Unity (Si pas encore fait)

### 1.1 Télécharger Unity Hub

1. Allez sur : https://unity.com/download
2. Cliquez sur **"Download Unity Hub"** (bouton vert)
3. Installez Unity Hub (c'est comme un lanceur pour Unity)

### 1.2 Installer Unity Editor

1. Ouvrez **Unity Hub**
2. En haut à gauche, cliquez sur **"Installs"**
3. Cliquez sur **"Install Editor"** (bouton bleu en haut à droite)
4. Choisissez **"Unity 2022.3 LTS"** (Long Term Support = stable)
5. Cliquez sur **"Next"**
6. Cochez ces modules :
   - ✅ **Android Build Support** (si vous voulez publier sur Android)
   - ✅ **iOS Build Support** (si vous voulez publier sur iPhone)
   - ✅ **Documentation**
7. Cliquez sur **"Install"** (ça prend 30-60 minutes)

---

## 📂 ÉTAPE 2 : Ouvrir Votre Projet

### 2.1 Ajouter le Projet dans Unity Hub

1. Ouvrez **Unity Hub**
2. Cliquez sur l'onglet **"Projects"** (à gauche)
3. Cliquez sur **"Add"** (bouton en haut à droite)
4. Naviguez vers votre dossier : `/home/user/mobil-game`
5. Sélectionnez le dossier **mobil-game**
6. Cliquez sur **"Add Project"**

### 2.2 Ouvrir le Projet

1. Dans Unity Hub, vous voyez maintenant **"mobil-game"** dans la liste
2. Cliquez sur le nom du projet
3. Unity s'ouvre (ça prend 2-5 minutes la première fois)

---

## 🖥️ ÉTAPE 3 : Comprendre l'Interface Unity

Quand Unity s'ouvre, vous voyez 5 zones principales :

```
┌──────────────────────────────────────────────────┐
│  Barre de Menu (File, Edit, Assets, etc.)       │
├──────────┬────────────────────────┬───────────────┤
│          │                        │               │
│ Hierarchy│      Scene View        │   Inspector   │
│          │    (Vue 3D du jeu)     │  (Propriétés) │
│ (Liste   │                        │               │
│  objets) │                        │               │
│          ├────────────────────────┤               │
│          │      Game View         │               │
│          │  (Vue joueur final)    │               │
├──────────┴────────────────────────┴───────────────┤
│              Project (Fichiers)                   │
│          Console (Messages/Erreurs)               │
└──────────────────────────────────────────────────┘
```

**Les 5 zones :**
1. **Hierarchy** (gauche) = Liste de tous les objets dans votre scène
2. **Scene View** (centre) = Vue 3D où vous placez les objets
3. **Inspector** (droite) = Propriétés de l'objet sélectionné
4. **Project** (bas) = Vos fichiers (scripts, images, etc.)
5. **Console** (bas) = Messages d'erreur et debug

---

## 🎯 ÉTAPE 4 : Créer une Scène de Test (SIMPLE)

### 4.1 Créer une Nouvelle Scène

1. Dans la barre de menu en haut : **File → New Scene**
2. Choisissez **"3D (Built-in Render Pipeline)"**
3. Cliquez sur **"Create"**
4. Sauvegardez : **File → Save As...**
5. Appelez-la **"TestScene"**
6. Cliquez sur **"Save"**

Vous avez maintenant une scène vide !

### 4.2 Comprendre Ce Qu'il y a Déjà

Dans **Hierarchy** (à gauche), vous voyez :
- **Main Camera** = la caméra du joueur
- **Directional Light** = lumière qui éclaire la scène

C'est tout ! Maintenant on va ajouter nos systèmes.

---

## ⚙️ ÉTAPE 5 : Ajouter les Nouveaux Systèmes (CORE)

### 5.1 Créer un GameObject pour LeaderboardManager

**Un GameObject = un objet dans votre jeu (invisible ou visible)**

1. Dans **Hierarchy** (fenêtre de gauche), **clic droit** dans le vide
2. Dans le menu qui s'ouvre : **Create Empty**
3. Un objet "GameObject" apparaît
4. Il est sélectionné (surligné en bleu)
5. Regardez **Inspector** (fenêtre de droite) → en haut vous voyez son nom
6. Changez le nom de "GameObject" à **"LeaderboardManager"**
7. Appuyez sur **Entrée**

✅ Vous avez créé un objet vide appelé LeaderboardManager !

### 5.2 Ajouter le Script LeaderboardManager

Maintenant on va "attacher" le code au GameObject :

1. **LeaderboardManager** est toujours sélectionné dans Hierarchy
2. Regardez **Inspector** (droite) → en bas vous voyez **"Add Component"**
3. Cliquez sur **"Add Component"**
4. Dans la barre de recherche qui apparaît, tapez : **"leaderboard"**
5. Vous voyez apparaître **"LeaderboardManager"** dans la liste
6. Cliquez dessus

✅ Le script est maintenant attaché ! Vous voyez maintenant toutes ses propriétés dans Inspector !

### 5.3 Configurer LeaderboardManager

Dans **Inspector**, vous voyez le script avec des paramètres :

1. Cherchez la case **"Enable Local Leaderboards"**
2. **Cochez cette case** (✅) pour tester sans serveur
3. Dans le champ **"Player Name"**, entrez votre pseudo (ex: "Player1")

✅ LeaderboardManager est configuré !

### 5.4 Répéter pour QuestManager

**Exactement la même procédure :**

1. **Clic droit dans Hierarchy** → **Create Empty**
2. Renommez en **"QuestManager"**
3. Avec QuestManager sélectionné → **Add Component** dans Inspector
4. Tapez **"quest"** → Cliquez sur **"QuestManager"**

✅ QuestManager ajouté !

### 5.5 Répéter pour LocalizationManager

1. **Clic droit dans Hierarchy** → **Create Empty**
2. Renommez en **"LocalizationManager"**
3. **Add Component** → Tapez **"local"** → Cliquez sur **"LocalizationManager"**

✅ LocalizationManager ajouté !

### 5.6 Ajouter GameEvents (Obligatoire)

Les nouveaux systèmes ont besoin de GameEvents :

1. **Clic droit dans Hierarchy** → **Create Empty**
2. Renommez en **"GameEvents"**
3. **Add Component** → Tapez **"gameevents"** → Cliquez sur **"GameEvents"**

✅ GameEvents ajouté !

---

## 🎮 ÉTAPE 6 : Ajouter le Script de Test

### 6.1 Créer le Fichier de Test

1. En bas, dans **Project** (fenêtre des fichiers), naviguez vers **Assets → Scripts**
2. **Clic droit** dans la zone vide
3. **Create → C# Script**
4. Appelez-le **"TestNewFeatures"** (exactement comme ça)
5. **Double-clic** sur le fichier pour l'ouvrir

### 6.2 Copier le Code de Test

Votre éditeur de code s'ouvre (Visual Studio ou VS Code).

**EFFACEZ TOUT** ce qu'il y a dedans et **COLLEZ** ce code :

```csharp
using UnityEngine;
using PrisonIsland.Core;
using System.Collections.Generic;

public class TestNewFeatures : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== TEST DES NOUVEAUX SYSTÈMES ===");
        Invoke("TestSystems", 1f); // Attendre 1 seconde
    }

    void TestSystems()
    {
        Debug.Log("--- Test démarré ---");

        // Test 1: Leaderboards
        if (LeaderboardManager.Instance != null)
        {
            Debug.Log("✅ LeaderboardManager OK");

            // Soumettre un score test
            LeaderboardManager.Instance.SubmitScore(LeaderboardType.TotalMoney, 50000);
            Debug.Log("📊 Score soumis: 50,000$");

            // Voir le classement
            var entries = LeaderboardManager.Instance.GetLeaderboard(LeaderboardType.TotalMoney, 10);
            Debug.Log($"📋 Classement: {entries.Count} entrées");
        }
        else
        {
            Debug.LogError("❌ LeaderboardManager manquant!");
        }

        // Test 2: Quests
        if (QuestManager.Instance != null)
        {
            Debug.Log("✅ QuestManager OK");

            var quests = QuestManager.Instance.GetActiveQuests();
            Debug.Log($"📋 Quêtes actives: {quests.Count}");

            foreach (var quest in quests)
            {
                Debug.Log($"  • {quest.questName}: {quest.currentProgress}/{quest.targetValue}");
            }
        }
        else
        {
            Debug.LogError("❌ QuestManager manquant!");
        }

        // Test 3: Localization
        if (LocalizationManager.Instance != null)
        {
            Debug.Log("✅ LocalizationManager OK");
            Debug.Log($"🌍 Langue actuelle: {LocalizationManager.Instance.GetCurrentLanguage()}");

            string playBtn = LocalizationManager.Instance.GetText("ui_play");
            Debug.Log($"🔤 Bouton Play: '{playBtn}'");
        }
        else
        {
            Debug.LogError("❌ LocalizationManager manquant!");
        }

        Debug.Log("--- Test terminé ---");
    }

    void Update()
    {
        // RACCOURCIS CLAVIER

        // Touche L = Leaderboards
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (LeaderboardManager.Instance != null)
            {
                LeaderboardManager.Instance.SubmitScore(LeaderboardType.TotalMoney, Random.Range(10000, 100000));
                Debug.Log("📊 Nouveau score aléatoire soumis!");

                var entries = LeaderboardManager.Instance.GetLeaderboard(LeaderboardType.TotalMoney, 10);
                Debug.Log("=== TOP 10 ===");
                for (int i = 0; i < entries.Count; i++)
                {
                    Debug.Log($"#{i+1} {entries[i].playerName}: {entries[i].score:N0}");
                }
            }
        }

        // Touche Q = Quests
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (QuestManager.Instance != null)
            {
                var quests = QuestManager.Instance.GetActiveQuests();
                Debug.Log($"=== QUÊTES ACTIVES ({quests.Count}) ===");
                foreach (var quest in quests)
                {
                    float percent = quest.GetProgressPercentage();
                    Debug.Log($"• {quest.questName}: {percent:F1}% complété");
                }
            }
        }

        // Touche F = Français
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.SetLanguage(SystemLanguage.French);
                Debug.Log("🇫🇷 Langue changée: FRANÇAIS");
                Debug.Log($"  ui_play = {LocalizationManager.Instance.GetText("ui_play")}");
                Debug.Log($"  resource_money = {LocalizationManager.Instance.GetText("resource_money")}");
            }
        }

        // Touche E = English
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.SetLanguage(SystemLanguage.English);
                Debug.Log("🇬🇧 Language changed: ENGLISH");
                Debug.Log($"  ui_play = {LocalizationManager.Instance.GetText("ui_play")}");
                Debug.Log($"  resource_money = {LocalizationManager.Instance.GetText("resource_money")}");
            }
        }

        // Touche S = Spanish
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.SetLanguage(SystemLanguage.Spanish);
                Debug.Log("🇪🇸 Idioma cambiado: ESPAÑOL");
            }
        }

        // Touche P = Progresser une quête test
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestTargetType.BuildAny, 1);
                Debug.Log("📈 Progression +1 sur BuildAny");
            }
        }
    }
}
```

**Sauvegardez** (Ctrl+S ou Cmd+S) et **fermez** l'éditeur.

### 6.3 Attacher le Script de Test

Retour dans Unity :

1. Dans **Hierarchy**, **clic droit** → **Create Empty**
2. Renommez en **"Tester"**
3. **Add Component** → Tapez **"test"** → Cliquez sur **"TestNewFeatures"**

✅ Script de test prêt !

---

## ▶️ ÉTAPE 7 : LANCER LE TEST

### 7.1 Vérifier Votre Scène

Dans **Hierarchy**, vous devriez avoir :
```
✅ Main Camera
✅ Directional Light
✅ LeaderboardManager
✅ QuestManager
✅ LocalizationManager
✅ GameEvents
✅ Tester
```

### 7.2 Sauvegarder

**File → Save** (ou Ctrl+S)

### 7.3 Lancer le Jeu

**EN HAUT AU CENTRE**, vous voyez 3 boutons :
- ▶️ **Play** (triangle)
- ⏸️ Pause
- ⏭️ Step

1. Cliquez sur **▶️ Play**
2. Unity entre en "mode jeu" (tout devient un peu bleu)
3. Regardez en BAS la fenêtre **Console**

### 7.4 Voir les Résultats

Dans la **Console** (en bas), vous devriez voir :

```
=== TEST DES NOUVEAUX SYSTÈMES ===
--- Test démarré ---
✅ LeaderboardManager OK
📊 Score soumis: 50,000$
📋 Classement: 1 entrées
✅ QuestManager OK
📋 Quêtes actives: 3
  • Build Something: 0/2
  • Make Money: 0/5000
  • Stay Safe: 0/60
✅ LocalizationManager OK
🌍 Langue actuelle: French
🔤 Bouton Play: 'Jouer'
--- Test terminé ---
```

✅ **SI VOUS VOYEZ ÇA = TOUT FONCTIONNE !** 🎉

### 7.5 Si vous voyez des erreurs rouges

Ne paniquez pas ! Copiez-moi l'erreur complète et je vous aide.

---

## 🎮 ÉTAPE 8 : Tester les Raccourcis Clavier

Pendant que le jeu tourne (bouton Play activé) :

1. **Pressez L** sur votre clavier
   - → Vous voyez le top 10 des scores dans Console

2. **Pressez Q**
   - → Vous voyez toutes les quêtes actives

3. **Pressez F**
   - → Langue change en Français
   - → Vous voyez les textes traduits

4. **Pressez E**
   - → Langue change en Anglais

5. **Pressez S**
   - → Langue change en Espagnol

6. **Pressez P**
   - → Fait progresser une quête de test

**Tout s'affiche dans la fenêtre Console en bas !**

### 7.6 Arrêter le Test

Cliquez à nouveau sur **▶️ Play** pour arrêter le jeu.

⚠️ **IMPORTANT** : Tout ce que vous modifiez pendant que le jeu tourne est perdu quand vous arrêtez !

---

## 📊 ÉTAPE 9 : Comprendre Ce Qui Se Passe

### Ce que vous avez créé :

1. **LeaderboardManager** = Gère les classements
   - Enregistre les scores localement
   - Peut être connecté à Firebase/PlayFab plus tard

2. **QuestManager** = Gère les missions
   - 3 quêtes quotidiennes (reset tous les jours)
   - 5 missions hebdomadaires (reset toutes les semaines)
   - Quêtes d'histoire permanentes

3. **LocalizationManager** = Gère les langues
   - 7 langues supportées
   - Change automatiquement selon le système

4. **GameEvents** = Système d'événements
   - Permet aux systèmes de communiquer entre eux

### Où sont stockées les données ?

Sur Windows : `C:\Users\VotreNom\AppData\LocalLow\DefaultCompany\mobil-game`
Sur Mac : `~/Library/Application Support/DefaultCompany/mobil-game`
Sur Linux : `~/.config/unity3d/DefaultCompany/mobil-game`

---

## 🔍 ÉTAPE 10 : Inspecter les Données (Avancé)

### Voir les données en temps réel

1. Lancez le jeu (▶️ Play)
2. Dans **Hierarchy**, cliquez sur **LeaderboardManager**
3. Dans **Inspector** (droite), vous voyez toutes les variables
4. Pressez **L** pendant que le jeu tourne
5. Regardez les variables changer en temps réel !

### Réinitialiser toutes les données

1. Arrêtez le jeu
2. Dans **Hierarchy**, sélectionnez **LeaderboardManager**
3. Dans **Inspector**, cherchez le menu **⋮** (3 points verticaux) à côté du nom du script
4. **Clic droit** sur le nom du script
5. Vous voyez **"Reset All Leaderboards"** - cliquez dessus

Même chose pour **QuestManager** → "Reset All Quests"

---

## 🎨 ÉTAPE 11 : Prochaines Étapes (Optionnel)

### Vous voulez créer l'interface utilisateur (UI) ?

C'est plus complexe mais voici le principe :

1. **Clic droit dans Hierarchy** → **UI → Canvas**
   - Ça crée un Canvas (surface pour l'UI)

2. **Clic droit sur Canvas** → **UI → Panel**
   - Ça crée un panneau pour le leaderboard

3. **Ajouter des textes, boutons**, etc.

Mais AVANT de faire ça, je vous recommande de :
1. Finir le reste de votre jeu
2. Puis revenir à l'UI

L'UI est la dernière chose à faire normalement !

---

## 🐛 DÉPANNAGE

### "The type or namespace name 'PrisonIsland' could not be found"

**Solution :**
1. Fermez Unity
2. Supprimez le dossier `Library` dans votre projet
3. Rouvrez Unity (il recompile tout)

### "LeaderboardManager.Instance is null"

**Solution :** Vérifiez que LeaderboardManager existe dans Hierarchy ET a le script attaché

### "NullReferenceException"

**Solution :** Un manager manque. Vérifiez que vous avez bien créé :
- LeaderboardManager
- QuestManager
- LocalizationManager
- GameEvents

### La Console est remplie d'erreurs jaunes

Les **warnings jaunes** ne sont pas graves, ignorez-les pour l'instant.
Seules les **erreurs rouges** sont bloquantes.

---

## 📝 CHECKLIST FINALE

Avant de dire "ça marche" :

```
□ Unity 2022.3 LTS installé
□ Projet mobil-game ouvert dans Unity
□ Scène TestScene créée et sauvegardée
□ 4 GameObjects créés (Leaderboard, Quest, Localization, GameEvents)
□ 4 scripts attachés aux GameObjects
□ Script TestNewFeatures créé et attaché à "Tester"
□ Scène sauvegardée (Ctrl+S)
□ Play pressé - pas d'erreurs rouges dans Console
□ Touches L, Q, F, E testées → messages dans Console
```

✅ Si tout est coché = **VOUS AVEZ RÉUSSI !** 🎉

---

## 🎯 RÉSUMÉ ULTRA RAPIDE

**Pour quelqu'un qui revient plus tard :**

1. Ouvrir Unity Hub → Ouvrir projet mobil-game
2. File → Open Scene → TestScene
3. Vérifier que Hierarchy a 7 objets
4. ▶️ Play
5. Presser L, Q, F, E pour tester
6. Regarder Console pour les résultats

---

## 📞 BESOIN D'AIDE ?

**Si vous êtes bloqué, donnez-moi :**
1. À quelle étape vous êtes (numéro)
2. Le message d'erreur COMPLET (copier-coller depuis Console)
3. Une description de ce qui ne marche pas

Je vous aide immédiatement ! 💪

---

**Bon courage ! Unity est intimidant au début, mais vous allez y arriver !** 🚀
