using UnityEngine;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages game settings and preferences
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [Header("Graphics Settings")]
        public int qualityLevel = 2; // 0=Low, 1=Medium, 2=High
        public int targetFrameRate = 60;
        public bool vSyncEnabled = true;

        [Header("Audio Settings")]
        public float masterVolume = 1f;
        public float musicVolume = 0.7f;
        public float sfxVolume = 1f;
        public bool audioEnabled = true;

        [Header("Gameplay Settings")]
        public float gameSpeed = 1f;
        public bool autosaveEnabled = true;
        public float autosaveInterval = 300f; // 5 minutes
        public bool tutorialEnabled = true;
        public bool notificationsEnabled = true;

        [Header("Controls Settings")]
        public float cameraSensitivity = 1f;
        public bool edgeScrolling = true;
        public bool hapticFeedback = true;

        [Header("Language")]
        public string language = "fr"; // fr, en, es, de, etc.

        private float autosaveTimer = 0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
                ApplySettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (autosaveEnabled)
            {
                autosaveTimer += Time.deltaTime;
                if (autosaveTimer >= autosaveInterval)
                {
                    autosaveTimer = 0f;
                    SaveLoadSystem.Instance?.SaveGame();
                }
            }
        }

        public void LoadSettings()
        {
            // Graphics
            qualityLevel = PlayerPrefs.GetInt("QualityLevel", 2);
            targetFrameRate = PlayerPrefs.GetInt("TargetFrameRate", 60);
            vSyncEnabled = PlayerPrefs.GetInt("VSync", 1) == 1;

            // Audio
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            audioEnabled = PlayerPrefs.GetInt("AudioEnabled", 1) == 1;

            // Gameplay
            gameSpeed = PlayerPrefs.GetFloat("GameSpeed", 1f);
            autosaveEnabled = PlayerPrefs.GetInt("AutosaveEnabled", 1) == 1;
            autosaveInterval = PlayerPrefs.GetFloat("AutosaveInterval", 300f);
            tutorialEnabled = PlayerPrefs.GetInt("TutorialEnabled", 1) == 1;
            notificationsEnabled = PlayerPrefs.GetInt("NotificationsEnabled", 1) == 1;

            // Controls
            cameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
            edgeScrolling = PlayerPrefs.GetInt("EdgeScrolling", 1) == 1;
            hapticFeedback = PlayerPrefs.GetInt("HapticFeedback", 1) == 1;

            // Language
            language = PlayerPrefs.GetString("Language", "fr");
        }

        public void SaveSettings()
        {
            // Graphics
            PlayerPrefs.SetInt("QualityLevel", qualityLevel);
            PlayerPrefs.SetInt("TargetFrameRate", targetFrameRate);
            PlayerPrefs.SetInt("VSync", vSyncEnabled ? 1 : 0);

            // Audio
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetInt("AudioEnabled", audioEnabled ? 1 : 0);

            // Gameplay
            PlayerPrefs.SetFloat("GameSpeed", gameSpeed);
            PlayerPrefs.SetInt("AutosaveEnabled", autosaveEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("AutosaveInterval", autosaveInterval);
            PlayerPrefs.SetInt("TutorialEnabled", tutorialEnabled ? 1 : 0);
            PlayerPrefs.SetInt("NotificationsEnabled", notificationsEnabled ? 1 : 0);

            // Controls
            PlayerPrefs.SetFloat("CameraSensitivity", cameraSensitivity);
            PlayerPrefs.SetInt("EdgeScrolling", edgeScrolling ? 1 : 0);
            PlayerPrefs.SetInt("HapticFeedback", hapticFeedback ? 1 : 0);

            // Language
            PlayerPrefs.SetString("Language", language);

            PlayerPrefs.Save();

            ApplySettings();
        }

        public void ApplySettings()
        {
            // Apply graphics settings
            QualitySettings.SetQualityLevel(qualityLevel);
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;

            // Apply audio settings
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(masterVolume);
                AudioManager.Instance.SetMusicVolume(musicVolume);
                AudioManager.Instance.SetSFXVolume(sfxVolume);
            }

            // Apply game speed
            Time.timeScale = gameSpeed;

            Debug.Log("Settings applied successfully!");
        }

        public void ResetToDefaults()
        {
            qualityLevel = 2;
            targetFrameRate = 60;
            vSyncEnabled = true;
            masterVolume = 1f;
            musicVolume = 0.7f;
            sfxVolume = 1f;
            audioEnabled = true;
            gameSpeed = 1f;
            autosaveEnabled = true;
            autosaveInterval = 300f;
            tutorialEnabled = true;
            notificationsEnabled = true;
            cameraSensitivity = 1f;
            edgeScrolling = true;
            hapticFeedback = true;
            language = "fr";

            SaveSettings();
        }

        // Convenience methods
        public void SetQuality(int level)
        {
            qualityLevel = Mathf.Clamp(level, 0, 2);
            SaveSettings();
        }

        public void SetGameSpeed(float speed)
        {
            gameSpeed = Mathf.Clamp(speed, 0.5f, 3f);
            Time.timeScale = gameSpeed;
            SaveSettings();
        }

        public void ToggleAudio()
        {
            audioEnabled = !audioEnabled;
            AudioListener.volume = audioEnabled ? masterVolume : 0f;
            SaveSettings();
        }
    }
}
