using UnityEngine;
using System.Collections.Generic;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages all audio in the game
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambienceSource;

        [Header("Music")]
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip gameplayMusic;
        [SerializeField] private AudioClip tensionMusic;

        [Header("SFX - Construction")]
        [SerializeField] private AudioClip buildSound;
        [SerializeField] private AudioClip demolishSound;

        [Header("SFX - Prisoners")]
        [SerializeField] private AudioClip prisonerArriveSound;
        [SerializeField] private AudioClip escapeAlarmSound;
        [SerializeField] private AudioClip fightSound;

        [Header("SFX - UI")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioClip warningSound;

        [Header("Ambience")]
        [SerializeField] private AudioClip prisonAmbienceSound;
        [SerializeField] private AudioClip outdoorAmbienceSound;

        [Header("Settings")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.7f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float ambienceVolume = 0.5f;

        private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                SubscribeToEvents();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (ambienceSource == null)
            {
                GameObject ambienceObj = new GameObject("AmbienceSource");
                ambienceObj.transform.SetParent(transform);
                ambienceSource = ambienceObj.AddComponent<AudioSource>();
                ambienceSource.loop = true;
                ambienceSource.playOnAwake = false;
            }
        }

        private void SubscribeToEvents()
        {
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnBuildingConstructed += (b) => PlaySFX("build");
                GameEvents.Instance.OnBuildingDemolished += (b) => PlaySFX("demolish");
                GameEvents.Instance.OnPrisonerArrived += (p) => PlaySFX("prisoner_arrive");
                GameEvents.Instance.OnPrisonerEscaped += (p) => PlaySFX("escape_alarm");
                GameEvents.Instance.OnResourcesInsufficient += () => PlaySFX("error");
            }
        }

        private void Update()
        {
            // Update volumes
            if (musicSource != null)
                musicSource.volume = masterVolume * musicVolume;
            if (sfxSource != null)
                sfxSource.volume = masterVolume * sfxVolume;
            if (ambienceSource != null)
                ambienceSource.volume = masterVolume * ambienceVolume;
        }

        #region Music Control
        public void PlayMusic(string musicName)
        {
            AudioClip clip = GetMusicClip(musicName);
            if (clip != null && musicSource != null)
            {
                if (musicSource.clip != clip)
                {
                    musicSource.clip = clip;
                    musicSource.Play();
                }
            }
        }

        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        public void PauseMusic()
        {
            if (musicSource != null)
                musicSource.Pause();
        }

        public void ResumeMusic()
        {
            if (musicSource != null)
                musicSource.UnPause();
        }

        private AudioClip GetMusicClip(string name)
        {
            switch (name.ToLower())
            {
                case "menu": return mainMenuMusic;
                case "gameplay": return gameplayMusic;
                case "tension": return tensionMusic;
                default: return null;
            }
        }
        #endregion

        #region SFX Control
        public void PlaySFX(string sfxName)
        {
            AudioClip clip = GetSFXClip(sfxName);
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        private AudioClip GetSFXClip(string name)
        {
            switch (name.ToLower())
            {
                case "build": return buildSound;
                case "demolish": return demolishSound;
                case "prisoner_arrive": return prisonerArriveSound;
                case "escape_alarm": return escapeAlarmSound;
                case "fight": return fightSound;
                case "button_click": return buttonClickSound;
                case "success": return successSound;
                case "error": return errorSound;
                case "warning": return warningSound;
                default: return null;
            }
        }
        #endregion

        #region Ambience Control
        public void PlayAmbience(string ambienceName)
        {
            AudioClip clip = GetAmbienceClip(ambienceName);
            if (clip != null && ambienceSource != null)
            {
                if (ambienceSource.clip != clip)
                {
                    ambienceSource.clip = clip;
                    ambienceSource.Play();
                }
            }
        }

        public void StopAmbience()
        {
            if (ambienceSource != null)
                ambienceSource.Stop();
        }

        private AudioClip GetAmbienceClip(string name)
        {
            switch (name.ToLower())
            {
                case "prison": return prisonAmbienceSound;
                case "outdoor": return outdoorAmbienceSound;
                default: return null;
            }
        }
        #endregion

        #region Volume Control
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }

        public void SetAmbienceVolume(float volume)
        {
            ambienceVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("AmbienceVolume", ambienceVolume);
        }

        public void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 0.5f);
        }
        #endregion
    }
}
