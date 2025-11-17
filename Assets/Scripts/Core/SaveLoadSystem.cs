using UnityEngine;
using System.IO;
using System.Collections.Generic;
using PrisonIsland.Data;

namespace PrisonIsland.Core
{
    [System.Serializable]
    public class GameSaveData
    {
        public GameResources resources;
        public int currentDay;
        public int totalEscapes;
        public int totalDeaths;
        public List<BuildingSaveData> buildings;
        public List<PrisonerSaveData> prisoners;
    }

    [System.Serializable]
    public class BuildingSaveData
    {
        public int buildingType;
        public Vector3 position;
        public int level;
    }

    [System.Serializable]
    public class PrisonerSaveData
    {
        public string name;
        public int dangerLevel;
        public int state;
        public PrisonerStats stats;
        public Vector3 position;
        public int escapeAttempts;
    }

    public class SaveLoadSystem : MonoBehaviour
    {
        public static SaveLoadSystem Instance { get; private set; }

        private string saveFilePath;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            saveFilePath = Path.Combine(Application.persistentDataPath, "prison_save.json");
        }

        public void SaveGame()
        {
            GameSaveData saveData = new GameSaveData
            {
                resources = GameManager.Instance.resources,
                currentDay = GameManager.Instance.currentDay,
                totalEscapes = GameManager.Instance.totalEscapes,
                totalDeaths = GameManager.Instance.totalDeaths,
                buildings = new List<BuildingSaveData>(),
                prisoners = new List<PrisonerSaveData>()
            };

            // Save buildings
            foreach (var building in GameManager.Instance.buildings)
            {
                saveData.buildings.Add(new BuildingSaveData
                {
                    buildingType = (int)building.type,
                    position = building.transform.position,
                    level = building.level
                });
            }

            // Save prisoners
            foreach (var prisoner in GameManager.Instance.prisoners)
            {
                saveData.prisoners.Add(new PrisonerSaveData
                {
                    name = prisoner.prisonerName,
                    dangerLevel = (int)prisoner.dangerLevel,
                    state = (int)prisoner.currentState,
                    stats = prisoner.stats,
                    position = prisoner.transform.position,
                    escapeAttempts = prisoner.escapeAttempts
                });
            }

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);

            Debug.Log($"Game saved to {saveFilePath}");
        }

        public void LoadGame()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.Log("No save file found.");
                return;
            }

            string json = File.ReadAllText(saveFilePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            if (saveData == null)
            {
                Debug.LogError("Failed to load save data!");
                return;
            }

            // Clear existing game
            ClearGame();

            // Load resources
            GameManager.Instance.resources = saveData.resources;
            GameManager.Instance.currentDay = saveData.currentDay;
            GameManager.Instance.totalEscapes = saveData.totalEscapes;
            GameManager.Instance.totalDeaths = saveData.totalDeaths;

            // Load buildings
            foreach (var buildingData in saveData.buildings)
            {
                BuildingType type = (BuildingType)buildingData.buildingType;
                // This would need BuildingManager to spawn buildings
                // You'll implement this based on your building spawning system
            }

            // Load prisoners
            foreach (var prisonerData in saveData.prisoners)
            {
                // This would need PrisonerManager to spawn prisoners
                // You'll implement this based on your prisoner spawning system
            }

            Debug.Log("Game loaded!");
        }

        private void ClearGame()
        {
            // Destroy all buildings
            foreach (var building in GameManager.Instance.buildings.ToArray())
            {
                Destroy(building.gameObject);
            }
            GameManager.Instance.buildings.Clear();

            // Destroy all prisoners
            foreach (var prisoner in GameManager.Instance.prisoners.ToArray())
            {
                Destroy(prisoner.gameObject);
            }
            GameManager.Instance.prisoners.Clear();
        }

        public bool SaveExists()
        {
            return File.Exists(saveFilePath);
        }

        public void DeleteSave()
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("Save file deleted.");
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveGame();
            }
        }
    }
}
