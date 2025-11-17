# 🏝️⛓️ Prison Island Manager - Unity 3D

Un jeu de gestion de prison en **3D** inspiré de **Land of Jail** et **Alcatraz**, développé avec **Unity**.

![Unity](https://img.shields.io/badge/Unity-2022.3+-blue.svg)
![C#](https://img.shields.io/badge/C%23-11.0-blue.svg)
![Platform](https://img.shields.io/badge/Platform-Android%20%7C%20iOS-green.svg)

## 📖 Description

**Prison Island Manager** est un jeu de simulation et gestion 3D où vous dirigez une prison sur une île isolée. Construisez des infrastructures en 3D, gérez vos détenus avec une IA avancée, et maintenez la sécurité de votre établissement pénitentiaire.

## ✨ Caractéristiques

### 🎮 Gameplay 3D Complet
- **Environnement 3D immersif** avec caméra libre
- **Construction en temps réel** avec placement visuel
- **IA des prisonniers** avec NavMesh et pathfinding
- **Animations et états** des personnages
- **Interactions 3D** (clic sur bâtiments/détenus)

### 🏗️ Système de Construction
- **11 types de bâtiments 3D**
- Placement en grille avec aperçu visuel
- Validation de placement
- Système de destruction
- Matériaux et couleurs selon le danger

### 👤 IA des Prisonniers
- **NavMesh Agent** pour les déplacements
- **États dynamiques** (idle, sleeping, working, plotting, etc.)
- **Pathfinding automatique** vers les bâtiments
- **Comportement intelligent** selon les besoins
- **Calcul du risque d'évasion**
- **Tentatives d'évasion** avec système de sécurité

### 📊 Gestion Complète
- **5 ressources** (argent, nourriture, matériaux, sécurité, réputation)
- **Économie dynamique** avec revenus/dépenses
- **Cycle jour/nuit**
- **Statistiques détaillées**
- **Système de sauvegarde** automatique

### 🎨 Interface UI
- **UI Canvas** avec TextMeshPro
- Affichage des ressources en temps réel
- Panels d'information (détenus, bâtiments)
- Menu de construction
- Statistiques et progression

## 🛠️ Installation et Configuration

### Prérequis

1. **Unity Hub** (dernière version)
2. **Unity Editor 2022.3 LTS** ou supérieur
3. **Visual Studio 2022** ou **Rider** (IDE C#)
4. **Android Build Support** (pour mobile Android)
5. **iOS Build Support** (pour mobile iOS - Mac uniquement)

### Étape 1 : Installation de Unity

1. **Télécharger Unity Hub**
   - Site officiel : https://unity.com/download
   - Installer Unity Hub

2. **Installer Unity Editor**
   - Ouvrir Unity Hub
   - Aller dans "Installs"
   - Cliquer "Install Editor"
   - Choisir **Unity 2022.3 LTS** (recommandé)
   - Cocher les modules :
     - ✅ Android Build Support (SDK & NDK Tools)
     - ✅ iOS Build Support (si sur Mac)
     - ✅ Documentation
     - ✅ Language Pack (Français)

### Étape 2 : Ouvrir le Projet

```bash
# Cloner le repository
git clone <repository-url>
cd mobil-game

# Ouvrir Unity Hub
# Cliquer "Open" → Sélectionner le dossier "mobil-game"
```

### Étape 3 : Configuration du Projet dans Unity

Une fois le projet ouvert dans Unity :

#### A. Créer la Scène Principale

1. **Créer une nouvelle scène** : `File > New Scene > 3D (Built-in Render Pipeline)`
2. **Sauvegarder** : `File > Save As...` → `Assets/Scenes/MainScene.unity`

#### B. Configurer le Terrain

1. **Créer un Plane** :
   - `GameObject > 3D Object > Plane`
   - Renommer en "Ground"
   - Position : (0, 0, 0)
   - Scale : (10, 1, 10)

2. **Ajouter un NavMesh** :
   - Sélectionner "Ground"
   - `Window > AI > Navigation`
   - Onglet "Bake"
   - Cliquer "Bake"

#### C. Créer les GameObjects Managers

1. **GameManager** :
   - `GameObject > Create Empty` → Nommer "GameManager"
   - Attacher le script `GameManager.cs`
   - Créer 2 Empty enfants :
     - "BuildingParent"
     - "PrisonerParent"
   - Assigner les références dans l'Inspector

2. **BuildingManager** :
   - `GameObject > Create Empty` → Nommer "BuildingManager"
   - Attacher le script `BuildingManager.cs`

3. **PrisonerManager** :
   - `GameObject > Create Empty` → Nommer "PrisonerManager"
   - Attacher le script `PrisonerManager.cs`

4. **UIManager** :
   - `GameObject > UI > Canvas` → Nommer "UICanvas"
   - Attacher le script `UIManager.cs`
   - Créer les panels UI (voir section UI ci-dessous)

5. **SaveLoadSystem** :
   - `GameObject > Create Empty` → Nommer "SaveLoadSystem"
   - Attacher le script `SaveLoadSystem.cs`

#### D. Configurer la Caméra

1. **Main Camera** :
   - Position : (0, 25, -20)
   - Rotation : (45, 0, 0)
   - Attacher le script `CameraController.cs`

#### E. Créer les Prefabs

##### Prefab : Cellule (Cell)

```
1. GameObject > 3D Object > Cube
2. Renommer "Cell"
3. Scale : (4, 3, 4)
4. Créer un matériau gris : Assets/Materials/CellMaterial
5. Attacher le script Building.cs
6. Ajouter un BoxCollider
7. Drag dans Assets/Prefabs/
```

##### Prefab : Prisonnier (Prisoner)

```
1. GameObject > 3D Object > Capsule
2. Renommer "Prisoner"
3. Scale : (1, 2, 1)
4. Ajouter NavMeshAgent component
5. Ajouter le script Prisoner.cs
6. Créer 4 matériaux (vert, jaune, orange, rouge)
7. Drag dans Assets/Prefabs/
```

**Répéter pour tous les types de bâtiments** (Cafeteria, Yard, Infirmary, etc.)

#### F. Créer l'Interface UI

1. **Resource Bar** (en haut) :
   - Panel → TextMeshPro pour chaque ressource
   - Icônes emojis ou sprites

2. **Build Menu** :
   - Panel avec boutons pour chaque type de bâtiment
   - Scripts attachés aux boutons

3. **Prisoner Info Panel** :
   - Panel avec textes et sliders
   - Boutons Feed et Release

4. **Building Info Panel** :
   - Panel avec infos du bâtiment
   - Bouton Demolish

### Étape 4 : Build pour Mobile

#### Android

```
1. File > Build Settings
2. Switch Platform → Android
3. Player Settings :
   - Company Name : Votre nom
   - Product Name : Prison Island Manager
   - Package Name : com.yourcompany.prisonisland
   - Minimum API Level : 24 (Android 7.0)
   - Target API Level : 33 (Android 13)
4. Build and Run
```

#### iOS (Mac uniquement)

```
1. File > Build Settings
2. Switch Platform → iOS
3. Player Settings :
   - Company Name
   - Product Name
   - Bundle Identifier
4. Build → Ouvrir dans Xcode → Run
```

## 🎯 Architecture du Code

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs          # Gestionnaire principal du jeu
│   │   ├── CameraController.cs     # Contrôle de la caméra
│   │   └── SaveLoadSystem.cs       # Sauvegarde/chargement
│   ├── Data/
│   │   ├── GameResources.cs        # Classe des ressources
│   │   ├── BuildingData.cs         # Données des bâtiments
│   │   └── PrisonerData.cs         # Données des prisonniers
│   ├── Managers/
│   │   ├── BuildingManager.cs      # Gestion construction 3D
│   │   └── PrisonerManager.cs      # Spawn et gestion prisonniers
│   ├── Buildings/
│   │   └── Building.cs             # Comportement bâtiment
│   ├── Prisoners/
│   │   └── Prisoner.cs             # IA et comportement prisonnier
│   └── UI/
│       └── UIManager.cs            # Gestion interface
├── Prefabs/                        # Prefabs des bâtiments/prisonniers
├── Materials/                      # Matériaux 3D
├── Scenes/                         # Scènes Unity
└── Models/                         # Modèles 3D (optionnel)
```

## 🎮 Contrôles

### PC
- **WASD** ou **Flèches** : Déplacer la caméra
- **Q/E** : Rotation de la caméra
- **Molette** : Zoom
- **Clic gauche** : Sélectionner bâtiment/prisonnier
- **Clic molette + drag** : Déplacer la caméra

### Mobile
- **1 doigt** : Déplacer la caméra
- **2 doigts** : Zoom (pinch)
- **Tap** : Sélectionner

## 📋 Types de Bâtiments 3D

| Bâtiment | Coût | Capacité | Bonus |
|----------|------|----------|-------|
| Cellule | 💰100 🧱50 | 2 | - |
| Cantine | 💰200 🧱100 | 20 | - |
| Cour | 💰150 🧱80 | 30 | - |
| Infirmerie | 💰250 🧱120 | 5 | +5 santé |
| Atelier | 💰180 🧱90 | 10 | +2💰/jour |
| Tour de Garde | 💰300 🧱150 | 0 | +10% sécurité |
| Douches | 💰120 🧱60 | 8 | +hygiène |
| Cuisine | 💰220 🧱110 | 0 | +3🍞/jour |
| Bibliothèque | 💰160 🧱80 | 15 | +moral |
| Parloir | 💰140 🧱70 | 10 | - |
| Cachot | 💰180 🧱100 | 1 | - |

## 🚀 Fonctionnalités Clés

### IA des Prisonniers (NavMesh)
```csharp
// Le prisonnier se déplace automatiquement
agent.SetDestination(target.position);

// États intelligents
switch (currentState) {
    case PrisonerState.Exercising:
        // Va automatiquement à la cour
        MoveToBuilding(FindNearestBuilding(BuildingType.Yard));
        break;
    case PrisonerState.Sleeping:
        // Retourne à sa cellule
        MoveToBuilding(assignedBuilding);
        break;
}
```

### Système de Construction 3D
```csharp
// Aperçu visuel en temps réel
currentBuildingPreview.transform.position = mouseWorldPosition;

// Validation du placement
bool isValid = IsValidPlacement(position);

// Placement confirmé
PlaceBuildingAt(type, position);
```

### Calcul d'Évasion
```csharp
float escapeRisk = dangerLevel * 10 + (100 - morale) * 0.3;
if (escapeRisk > security) {
    // Tentative d'évasion !
}
```

## 📦 Assets Recommandés (Gratuits)

Pour améliorer le visuel 3D :

1. **Low Poly Prison Pack** (Asset Store)
2. **Simple Character Pack** (pour prisonniers)
3. **Modular Building Pack** (bâtiments)
4. **TextMesh Pro** (déjà inclus)

## 🐛 Problèmes Courants

### NavMesh ne fonctionne pas
```
Solution :
1. Window > AI > Navigation
2. Sélectionner le Ground
3. Cocher "Navigation Static"
4. Bake
```

### Les scripts ne compilent pas
```
Solution :
1. Vérifier TextMeshPro est installé (Package Manager)
2. Edit > Preferences > External Tools → Regenerate project files
```

### Prefabs ne s'affichent pas
```
Solution :
1. Vérifier que les matériaux sont assignés
2. Vérifier l'échelle (pas trop petite)
3. Vérifier la position de la caméra
```

## 🎯 Prochaines Étapes

Après avoir configuré le projet de base :

1. **Créer de beaux modèles 3D** (ou importer de l'Asset Store)
2. **Ajouter des animations** (prisonniers qui marchent, travaillent)
3. **Améliorer l'UI** (menus plus beaux)
4. **Ajouter des sons** (bruits de prison, alarmes)
5. **Optimiser les performances** (object pooling)
6. **Ajouter des événements** (émeutes, inspections)

## 📚 Ressources

- [Unity Documentation](https://docs.unity3d.com/)
- [NavMesh Tutorial](https://learn.unity.com/tutorial/unity-navmesh)
- [C# Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [TextMeshPro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/)

## 📄 Licence

MIT License

## 👨‍💻 Développement

Tous les scripts C# sont prêts et fonctionnels. Il ne reste plus qu'à :
1. Créer les prefabs 3D dans Unity
2. Configurer l'UI
3. Bake le NavMesh
4. Build !

---

**Bon développement et ne laissez pas vos prisonniers s'échapper en 3D ! 🏃⛓️**
