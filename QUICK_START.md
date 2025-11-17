# 🚀 Guide de Démarrage Rapide - Prison Island Manager 3D

## Étapes Essentielles (30 minutes)

### 1. Installer Unity (15 min)

```bash
1. Télécharger Unity Hub : https://unity.com/download
2. Installer Unity 2022.3 LTS
3. Ajouter Android Build Support
```

### 2. Ouvrir le Projet (2 min)

```bash
1. Unity Hub → Open
2. Sélectionner le dossier "mobil-game"
3. Attendre l'importation
```

### 3. Configuration Minimale (10 min)

#### A. Créer la Scène

```
File > New Scene > 3D
File > Save As... > Assets/Scenes/MainScene.unity
```

#### B. Ground + NavMesh

```
1. GameObject > 3D Object > Plane
   - Name: "Ground"
   - Position: (0, 0, 0)
   - Scale: (10, 1, 10)

2. Window > AI > Navigation
   - Sélectionner Ground
   - Cocher "Navigation Static"
   - Cliquer "Bake"
```

#### C. Managers (Drag & Drop)

```
1. Créer Empty GameObject "GameManager"
   - Attacher Assets/Scripts/Core/GameManager.cs
   - Créer 2 enfants vides:
     * BuildingParent
     * PrisonerParent
   - Assigner dans l'Inspector

2. Créer "BuildingManager"
   - Attacher Assets/Scripts/Managers/BuildingManager.cs

3. Créer "PrisonerManager"
   - Attacher Assets/Scripts/Managers/PrisonerManager.cs

4. Créer "SaveLoadSystem"
   - Attacher Assets/Scripts/Core/SaveLoadSystem.cs
```

#### D. Caméra

```
Sélectionner Main Camera:
- Position: (0, 25, -20)
- Rotation: (45, 0, 0)
- Attacher Assets/Scripts/Core/CameraController.cs
```

#### E. UI Canvas

```
GameObject > UI > Canvas
- Attacher Assets/Scripts/UI/UIManager.cs
- Créer Panel enfant "ResourceBar"
  - Ajouter TextMeshPro pour chaque ressource
```

### 4. Prefabs Minimaux (5 min)

#### Cellule

```
1. GameObject > 3D Object > Cube
2. Name: "Cell"
3. Scale: (4, 3, 4)
4. Add Component > Building.cs
5. Drag dans Assets/Prefabs/
6. Assigner dans BuildingManager.cellPrefab
```

#### Prisonnier

```
1. GameObject > 3D Object > Capsule
2. Name: "Prisoner"
3. Scale: (1, 2, 1)
4. Add Component > Nav Mesh Agent
5. Add Component > Prisoner.cs
6. Drag dans Assets/Prefabs/
7. Assigner dans PrisonerManager.prisonerPrefab
```

### 5. Test !

```
Cliquer PLAY ▶️

Vous devriez voir:
- 2 cellules
- 1 cantine
- 3 prisonniers qui se déplacent
- Ressources affichées en haut
```

## 🎯 Checklist Rapide

- [ ] Unity installé
- [ ] Projet ouvert
- [ ] Scène créée
- [ ] NavMesh baked
- [ ] 4 Managers créés et configurés
- [ ] Caméra positionnée
- [ ] 2 Prefabs créés (Cell, Prisoner)
- [ ] Prefabs assignés aux Managers
- [ ] Test en Play mode

## ❌ Si ça ne marche pas

**Erreurs de compilation ?**
```
Window > Package Manager
Installer TextMesh Pro
```

**Prisonniers ne bougent pas ?**
```
Vérifier NavMesh est baked (bleu dans Scene view)
```

**Rien ne s'affiche ?**
```
Vérifier position caméra (0, 25, -20)
Vérifier les prefabs ont un Renderer
```

**Console pleine d'erreurs ?**
```
Vérifier tous les Managers sont dans la scène
Vérifier toutes les références sont assignées
```

## 📱 Build Android (Bonus)

```
1. File > Build Settings
2. Add Open Scenes
3. Switch Platform → Android
4. Player Settings:
   - Minimum API Level: 24
5. Build and Run
```

## 🎨 Améliorer le Visuel (Optionnel)

```
1. Asset Store (gratuit):
   - Low Poly Prison Pack
   - Simple Character Pack

2. Importer:
   - Remplacer les cubes par de vrais modèles
   - Ajouter textures
   - Ajouter lumières
```

## 💡 Prochaines Étapes

Une fois que le jeu tourne :

1. **Créer plus de prefabs** (tous les 11 bâtiments)
2. **Améliorer l'UI** (boutons, panels)
3. **Ajouter des animations**
4. **Optimiser les performances**
5. **Ajouter des sons**

---

**Temps total : ~30 minutes pour une version jouable ! 🎉**
