using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages game localization and multi-language support
    /// Supports easy addition of new languages
    /// Uses key-value pairs for all text
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        [Header("Language Settings")]
        [SerializeField] private SystemLanguage currentLanguage = SystemLanguage.English;
        [SerializeField] private SystemLanguage fallbackLanguage = SystemLanguage.English;

        [Header("Available Languages")]
        [SerializeField] private List<SystemLanguage> supportedLanguages = new List<SystemLanguage>
        {
            SystemLanguage.English,
            SystemLanguage.French,
            SystemLanguage.Spanish,
            SystemLanguage.German,
            SystemLanguage.Portuguese,
            SystemLanguage.Japanese,
            SystemLanguage.ChineseSimplified
        };

        // Localization dictionaries
        private Dictionary<SystemLanguage, Dictionary<string, string>> translations;

        // Events
        public event Action<SystemLanguage> OnLanguageChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeTranslations();
                LoadLanguagePreference();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Auto-detect system language if first time
            if (!PlayerPrefs.HasKey("UserLanguage"))
            {
                AutoDetectLanguage();
            }
        }

        #region Initialization

        private void InitializeTranslations()
        {
            translations = new Dictionary<SystemLanguage, Dictionary<string, string>>();

            // Initialize each supported language
            foreach (var language in supportedLanguages)
            {
                translations[language] = new Dictionary<string, string>();
            }

            // Load all translations
            LoadEnglishTranslations();
            LoadFrenchTranslations();
            LoadSpanishTranslations();
            LoadGermanTranslations();
            LoadPortugueseTranslations();
            LoadJapaneseTranslations();
            LoadChineseTranslations();
        }

        #endregion

        #region Translation Loading

        private void LoadEnglishTranslations()
        {
            var dict = translations[SystemLanguage.English];

            // UI
            dict["ui_play"] = "Play";
            dict["ui_settings"] = "Settings";
            dict["ui_shop"] = "Shop";
            dict["ui_quit"] = "Quit";
            dict["ui_back"] = "Back";
            dict["ui_confirm"] = "Confirm";
            dict["ui_cancel"] = "Cancel";
            dict["ui_close"] = "Close";
            dict["ui_buy"] = "Buy";
            dict["ui_claim"] = "Claim";

            // Resources
            dict["resource_money"] = "Money";
            dict["resource_food"] = "Food";
            dict["resource_materials"] = "Materials";
            dict["resource_security"] = "Security";
            dict["resource_reputation"] = "Reputation";
            dict["resource_crystals"] = "Crystals";

            // Buildings
            dict["building_cell"] = "Cell Block";
            dict["building_canteen"] = "Canteen";
            dict["building_workshop"] = "Workshop";
            dict["building_gym"] = "Gym";
            dict["building_infirmary"] = "Infirmary";
            dict["building_guard_tower"] = "Guard Tower";
            dict["building_kitchen"] = "Kitchen";
            dict["building_office"] = "Office";
            dict["building_shower"] = "Shower";
            dict["building_yard"] = "Yard";
            dict["building_wall"] = "Wall";

            // Game Messages
            dict["msg_insufficient_money"] = "Not enough money!";
            dict["msg_insufficient_materials"] = "Not enough materials!";
            dict["msg_insufficient_space"] = "Not enough space!";
            dict["msg_building_complete"] = "Building completed!";
            dict["msg_prisoner_arrived"] = "New prisoner arrived";
            dict["msg_prisoner_escaped"] = "Prisoner escaped!";
            dict["msg_daily_reward_claimed"] = "Daily reward claimed!";
            dict["msg_quest_completed"] = "Quest completed!";
            dict["msg_achievement_unlocked"] = "Achievement unlocked!";

            // Quests
            dict["quest_daily"] = "Daily Quest";
            dict["quest_weekly"] = "Weekly Mission";
            dict["quest_story"] = "Story Quest";

            // Shop
            dict["shop_crystals"] = "Crystals";
            dict["shop_resources"] = "Resources";
            dict["shop_vip"] = "VIP";
            dict["shop_starter_pack"] = "Starter Pack";
            dict["shop_builder_pack"] = "Builder Pack";
            dict["shop_premium_pack"] = "Premium Pack";
            dict["shop_vip_monthly"] = "VIP Monthly";

            // Daily Rewards
            dict["daily_reward_title"] = "Daily Reward!";
            dict["daily_reward_streak"] = "Streak: {0} days 🔥";
            dict["daily_reward_day"] = "Day {0}";

            // Settings
            dict["settings_graphics"] = "Graphics";
            dict["settings_audio"] = "Audio";
            dict["settings_gameplay"] = "Gameplay";
            dict["settings_language"] = "Language";

            // Leaderboards
            dict["leaderboard_title"] = "Leaderboards";
            dict["leaderboard_rank"] = "Rank";
            dict["leaderboard_player"] = "Player";
            dict["leaderboard_score"] = "Score";
            dict["leaderboard_total_money"] = "Total Money";
            dict["leaderboard_population"] = "Prison Population";
            dict["leaderboard_buildings"] = "Total Buildings";
            dict["leaderboard_days_survived"] = "Days Survived";

            // Tutorial
            dict["tutorial_welcome"] = "Welcome to Prison Island Manager!";
            dict["tutorial_build_cell"] = "Build your first cell block";
            dict["tutorial_receive_prisoner"] = "Wait for prisoners to arrive";
            dict["tutorial_manage_resources"] = "Manage your resources carefully";
        }

        private void LoadFrenchTranslations()
        {
            var dict = translations[SystemLanguage.French];

            // UI
            dict["ui_play"] = "Jouer";
            dict["ui_settings"] = "Paramètres";
            dict["ui_shop"] = "Boutique";
            dict["ui_quit"] = "Quitter";
            dict["ui_back"] = "Retour";
            dict["ui_confirm"] = "Confirmer";
            dict["ui_cancel"] = "Annuler";
            dict["ui_close"] = "Fermer";
            dict["ui_buy"] = "Acheter";
            dict["ui_claim"] = "Réclamer";

            // Resources
            dict["resource_money"] = "Argent";
            dict["resource_food"] = "Nourriture";
            dict["resource_materials"] = "Matériaux";
            dict["resource_security"] = "Sécurité";
            dict["resource_reputation"] = "Réputation";
            dict["resource_crystals"] = "Cristaux";

            // Buildings
            dict["building_cell"] = "Bloc de cellules";
            dict["building_canteen"] = "Cantine";
            dict["building_workshop"] = "Atelier";
            dict["building_gym"] = "Gymnase";
            dict["building_infirmary"] = "Infirmerie";
            dict["building_guard_tower"] = "Tour de garde";
            dict["building_kitchen"] = "Cuisine";
            dict["building_office"] = "Bureau";
            dict["building_shower"] = "Douches";
            dict["building_yard"] = "Cour";
            dict["building_wall"] = "Mur";

            // Game Messages
            dict["msg_insufficient_money"] = "Pas assez d'argent!";
            dict["msg_insufficient_materials"] = "Pas assez de matériaux!";
            dict["msg_insufficient_space"] = "Pas assez d'espace!";
            dict["msg_building_complete"] = "Construction terminée!";
            dict["msg_prisoner_arrived"] = "Nouveau prisonnier arrivé";
            dict["msg_prisoner_escaped"] = "Prisonnier évadé!";
            dict["msg_daily_reward_claimed"] = "Récompense quotidienne réclamée!";
            dict["msg_quest_completed"] = "Quête terminée!";
            dict["msg_achievement_unlocked"] = "Succès débloqué!";

            // Quests
            dict["quest_daily"] = "Quête quotidienne";
            dict["quest_weekly"] = "Mission hebdomadaire";
            dict["quest_story"] = "Quête d'histoire";

            // Shop
            dict["shop_crystals"] = "Cristaux";
            dict["shop_resources"] = "Ressources";
            dict["shop_vip"] = "VIP";
            dict["shop_starter_pack"] = "Pack débutant";
            dict["shop_builder_pack"] = "Pack constructeur";
            dict["shop_premium_pack"] = "Pack premium";
            dict["shop_vip_monthly"] = "VIP mensuel";

            // Daily Rewards
            dict["daily_reward_title"] = "Récompense quotidienne!";
            dict["daily_reward_streak"] = "Série: {0} jours 🔥";
            dict["daily_reward_day"] = "Jour {0}";

            // Settings
            dict["settings_graphics"] = "Graphismes";
            dict["settings_audio"] = "Audio";
            dict["settings_gameplay"] = "Gameplay";
            dict["settings_language"] = "Langue";

            // Leaderboards
            dict["leaderboard_title"] = "Classements";
            dict["leaderboard_rank"] = "Rang";
            dict["leaderboard_player"] = "Joueur";
            dict["leaderboard_score"] = "Score";
            dict["leaderboard_total_money"] = "Argent total";
            dict["leaderboard_population"] = "Population prison";
            dict["leaderboard_buildings"] = "Bâtiments totaux";
            dict["leaderboard_days_survived"] = "Jours survécus";

            // Tutorial
            dict["tutorial_welcome"] = "Bienvenue dans Prison Island Manager!";
            dict["tutorial_build_cell"] = "Construisez votre premier bloc de cellules";
            dict["tutorial_receive_prisoner"] = "Attendez l'arrivée des prisonniers";
            dict["tutorial_manage_resources"] = "Gérez vos ressources avec soin";
        }

        private void LoadSpanishTranslations()
        {
            var dict = translations[SystemLanguage.Spanish];

            dict["ui_play"] = "Jugar";
            dict["ui_settings"] = "Ajustes";
            dict["ui_shop"] = "Tienda";
            dict["ui_quit"] = "Salir";
            dict["resource_money"] = "Dinero";
            dict["resource_food"] = "Comida";
            dict["resource_materials"] = "Materiales";
            dict["building_cell"] = "Bloque de celdas";
            dict["msg_insufficient_money"] = "¡No hay suficiente dinero!";
            dict["tutorial_welcome"] = "¡Bienvenido a Prison Island Manager!";
            // Add more Spanish translations as needed
        }

        private void LoadGermanTranslations()
        {
            var dict = translations[SystemLanguage.German];

            dict["ui_play"] = "Spielen";
            dict["ui_settings"] = "Einstellungen";
            dict["ui_shop"] = "Laden";
            dict["ui_quit"] = "Beenden";
            dict["resource_money"] = "Geld";
            dict["resource_food"] = "Essen";
            dict["resource_materials"] = "Materialien";
            dict["building_cell"] = "Zellenblock";
            dict["msg_insufficient_money"] = "Nicht genug Geld!";
            dict["tutorial_welcome"] = "Willkommen bei Prison Island Manager!";
            // Add more German translations as needed
        }

        private void LoadPortugueseTranslations()
        {
            var dict = translations[SystemLanguage.Portuguese];

            dict["ui_play"] = "Jogar";
            dict["ui_settings"] = "Configurações";
            dict["ui_shop"] = "Loja";
            dict["ui_quit"] = "Sair";
            dict["resource_money"] = "Dinheiro";
            dict["resource_food"] = "Comida";
            dict["resource_materials"] = "Materiais";
            dict["building_cell"] = "Bloco de celas";
            dict["msg_insufficient_money"] = "Dinheiro insuficiente!";
            dict["tutorial_welcome"] = "Bem-vindo ao Prison Island Manager!";
            // Add more Portuguese translations as needed
        }

        private void LoadJapaneseTranslations()
        {
            var dict = translations[SystemLanguage.Japanese];

            dict["ui_play"] = "プレイ";
            dict["ui_settings"] = "設定";
            dict["ui_shop"] = "ショップ";
            dict["ui_quit"] = "終了";
            dict["resource_money"] = "お金";
            dict["resource_food"] = "食料";
            dict["resource_materials"] = "材料";
            dict["building_cell"] = "独房ブロック";
            dict["msg_insufficient_money"] = "お金が足りません！";
            dict["tutorial_welcome"] = "Prison Island Managerへようこそ！";
            // Add more Japanese translations as needed
        }

        private void LoadChineseTranslations()
        {
            var dict = translations[SystemLanguage.ChineseSimplified];

            dict["ui_play"] = "开始";
            dict["ui_settings"] = "设置";
            dict["ui_shop"] = "商店";
            dict["ui_quit"] = "退出";
            dict["resource_money"] = "金钱";
            dict["resource_food"] = "食物";
            dict["resource_materials"] = "材料";
            dict["building_cell"] = "牢房区";
            dict["msg_insufficient_money"] = "金钱不足！";
            dict["tutorial_welcome"] = "欢迎来到监狱岛管理！";
            // Add more Chinese translations as needed
        }

        #endregion

        #region Public API

        /// <summary>
        /// Get localized text by key
        /// </summary>
        public string GetText(string key)
        {
            // Try current language
            if (translations.ContainsKey(currentLanguage) &&
                translations[currentLanguage].ContainsKey(key))
            {
                return translations[currentLanguage][key];
            }

            // Try fallback language
            if (translations.ContainsKey(fallbackLanguage) &&
                translations[fallbackLanguage].ContainsKey(key))
            {
                Debug.LogWarning($"Translation key '{key}' not found for {currentLanguage}, using fallback");
                return translations[fallbackLanguage][key];
            }

            // Return key if nothing found
            Debug.LogError($"Translation key '{key}' not found!");
            return $"[{key}]";
        }

        /// <summary>
        /// Get localized text with string formatting
        /// </summary>
        public string GetText(string key, params object[] args)
        {
            string text = GetText(key);
            try
            {
                return string.Format(text, args);
            }
            catch (Exception e)
            {
                Debug.LogError($"String format error for key '{key}': {e.Message}");
                return text;
            }
        }

        /// <summary>
        /// Change current language
        /// </summary>
        public void SetLanguage(SystemLanguage language)
        {
            if (!supportedLanguages.Contains(language))
            {
                Debug.LogWarning($"Language {language} not supported, using fallback");
                language = fallbackLanguage;
            }

            currentLanguage = language;
            SaveLanguagePreference();
            OnLanguageChanged?.Invoke(currentLanguage);

            // Track analytics
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.TrackEvent("language_changed", new Dictionary<string, object>
                {
                    { "language", language.ToString() }
                });
            }

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke($"Language changed to {language}");
            }
        }

        /// <summary>
        /// Auto-detect system language
        /// </summary>
        public void AutoDetectLanguage()
        {
            SystemLanguage systemLang = Application.systemLanguage;

            if (supportedLanguages.Contains(systemLang))
            {
                SetLanguage(systemLang);
                Debug.Log($"Auto-detected language: {systemLang}");
            }
            else
            {
                SetLanguage(fallbackLanguage);
                Debug.Log($"System language {systemLang} not supported, using {fallbackLanguage}");
            }
        }

        /// <summary>
        /// Get current language
        /// </summary>
        public SystemLanguage GetCurrentLanguage()
        {
            return currentLanguage;
        }

        /// <summary>
        /// Get list of supported languages
        /// </summary>
        public List<SystemLanguage> GetSupportedLanguages()
        {
            return new List<SystemLanguage>(supportedLanguages);
        }

        /// <summary>
        /// Check if a key exists in current language
        /// </summary>
        public bool HasKey(string key)
        {
            return translations.ContainsKey(currentLanguage) &&
                   translations[currentLanguage].ContainsKey(key);
        }

        #endregion

        #region Save/Load

        private void SaveLanguagePreference()
        {
            PlayerPrefs.SetString("UserLanguage", currentLanguage.ToString());
            PlayerPrefs.Save();
        }

        private void LoadLanguagePreference()
        {
            if (PlayerPrefs.HasKey("UserLanguage"))
            {
                string langStr = PlayerPrefs.GetString("UserLanguage");
                if (Enum.TryParse(langStr, out SystemLanguage lang))
                {
                    currentLanguage = lang;
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get language display name
        /// </summary>
        public string GetLanguageDisplayName(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.English: return "English";
                case SystemLanguage.French: return "Français";
                case SystemLanguage.Spanish: return "Español";
                case SystemLanguage.German: return "Deutsch";
                case SystemLanguage.Portuguese: return "Português";
                case SystemLanguage.Japanese: return "日本語";
                case SystemLanguage.ChineseSimplified: return "简体中文";
                default: return language.ToString();
            }
        }

        /// <summary>
        /// Get language flag emoji
        /// </summary>
        public string GetLanguageFlag(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.English: return "🇬🇧";
                case SystemLanguage.French: return "🇫🇷";
                case SystemLanguage.Spanish: return "🇪🇸";
                case SystemLanguage.German: return "🇩🇪";
                case SystemLanguage.Portuguese: return "🇵🇹";
                case SystemLanguage.Japanese: return "🇯🇵";
                case SystemLanguage.ChineseSimplified: return "🇨🇳";
                default: return "🌐";
            }
        }

        #endregion

        #region Development Tools

        /// <summary>
        /// Export all translation keys for a language (for translators)
        /// </summary>
        [ContextMenu("Export Translation Keys")]
        public void ExportTranslationKeys()
        {
            if (translations.ContainsKey(SystemLanguage.English))
            {
                var keys = translations[SystemLanguage.English].Keys.OrderBy(k => k).ToList();
                Debug.Log($"Translation Keys ({keys.Count}):\n" + string.Join("\n", keys));
            }
        }

        /// <summary>
        /// Find missing translations for a language
        /// </summary>
        public List<string> FindMissingTranslations(SystemLanguage language)
        {
            if (!translations.ContainsKey(SystemLanguage.English) ||
                !translations.ContainsKey(language))
            {
                return new List<string>();
            }

            var englishKeys = translations[SystemLanguage.English].Keys;
            var languageKeys = translations[language].Keys;

            return englishKeys.Except(languageKeys).ToList();
        }

        #endregion
    }
}
