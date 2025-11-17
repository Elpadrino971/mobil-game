using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages achievements and player progression
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        [Header("Achievements")]
        private List<Achievement> achievements = new List<Achievement>();
        private HashSet<string> unlockedAchievements = new HashSet<string>();

        public event System.Action<Achievement> OnAchievementUnlocked;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAchievements();
                LoadProgress();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SubscribeToEvents();
        }

        private void InitializeAchievements()
        {
            // Prison Management Achievements
            achievements.Add(new Achievement
            {
                id = "first_prison",
                title = "Première Prison",
                description = "Construisez votre premier bâtiment",
                icon = "🏗️",
                points = 10
            });

            achievements.Add(new Achievement
            {
                id = "big_prison",
                title = "Grande Prison",
                description = "Possédez 20 bâtiments",
                icon = "🏛️",
                points = 50
            });

            achievements.Add(new Achievement
            {
                id = "mega_prison",
                title = "Méga Prison",
                description = "Possédez 50 bâtiments",
                icon = "🏙️",
                points = 100
            });

            // Prisoner Achievements
            achievements.Add(new Achievement
            {
                id = "first_prisoner",
                title = "Premier Détenu",
                description = "Accueillez votre premier prisonnier",
                icon = "👤",
                points = 10
            });

            achievements.Add(new Achievement
            {
                id = "full_house",
                title = "Prison Pleine",
                description = "Gérez 50 prisonniers simultanément",
                icon = "👥",
                points = 50
            });

            achievements.Add(new Achievement
            {
                id = "hundred_prisoners",
                title = "Centenaire",
                description = "Gérez 100 prisonniers au total",
                icon = "💯",
                points = 75
            });

            // Security Achievements
            achievements.Add(new Achievement
            {
                id = "no_escapes",
                title = "Forteresse Imprenable",
                description = "30 jours sans évasion",
                icon = "🛡️",
                points = 100
            });

            achievements.Add(new Achievement
            {
                id = "max_security",
                title = "Sécurité Maximale",
                description = "Atteignez 100% de sécurité",
                icon = "🔒",
                points = 50
            });

            achievements.Add(new Achievement
            {
                id = "escape_prevented",
                title = "Évasion Déjouée",
                description = "Empêchez 10 tentatives d'évasion",
                icon = "⚠️",
                points = 30
            });

            // Economic Achievements
            achievements.Add(new Achievement
            {
                id = "millionaire",
                title = "Millionnaire",
                description = "Accumulez 1 000 000$",
                icon = "💰",
                points = 100
            });

            achievements.Add(new Achievement
            {
                id = "profitable",
                title = "Rentable",
                description = "Gagnez 10 000$ en un jour",
                icon = "💵",
                points = 50
            });

            // Reputation Achievements
            achievements.Add(new Achievement
            {
                id = "perfect_reputation",
                title = "Réputation Parfaite",
                description = "Atteignez 100% de réputation",
                icon = "⭐",
                points = 75
            });

            achievements.Add(new Achievement
            {
                id = "survivor",
                title = "Survivant",
                description = "Survivez 100 jours",
                icon = "📅",
                points = 100
            });

            // Special Achievements
            achievements.Add(new Achievement
            {
                id = "speed_builder",
                title = "Constructeur Rapide",
                description = "Construisez 10 bâtiments en moins de 5 minutes",
                icon = "⚡",
                points = 50
            });

            achievements.Add(new Achievement
            {
                id = "humane_warden",
                title = "Directeur Humain",
                description = "Maintenez le moral moyen au-dessus de 80% pendant 30 jours",
                icon = "❤️",
                points = 75
            });
        }

        private void SubscribeToEvents()
        {
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnBuildingConstructed += CheckBuildingAchievements;
                GameEvents.Instance.OnPrisonerArrived += CheckPrisonerAchievements;
                GameEvents.Instance.OnEscapeAttemptFailed += (p) => CheckSecurityAchievements();
                GameEvents.Instance.OnNewDay += CheckDailyAchievements;
            }
        }

        private void CheckBuildingAchievements(Buildings.Building building)
        {
            int buildingCount = GameManager.Instance.buildings.Count;

            if (buildingCount >= 1)
                UnlockAchievement("first_prison");
            if (buildingCount >= 20)
                UnlockAchievement("big_prison");
            if (buildingCount >= 50)
                UnlockAchievement("mega_prison");
        }

        private void CheckPrisonerAchievements(Prisoners.Prisoner prisoner)
        {
            int prisonerCount = GameManager.Instance.prisoners.Count;

            if (prisonerCount >= 1)
                UnlockAchievement("first_prisoner");
            if (prisonerCount >= 50)
                UnlockAchievement("full_house");
        }

        private void CheckSecurityAchievements()
        {
            if (GameManager.Instance.resources.security >= 100)
                UnlockAchievement("max_security");
        }

        private void CheckDailyAchievements(int day)
        {
            if (day >= 100)
                UnlockAchievement("survivor");

            if (GameManager.Instance.resources.reputation >= 100)
                UnlockAchievement("perfect_reputation");

            if (GameManager.Instance.resources.money >= 1000000)
                UnlockAchievement("millionaire");
        }

        public void UnlockAchievement(string achievementId)
        {
            if (unlockedAchievements.Contains(achievementId))
                return;

            Achievement achievement = achievements.FirstOrDefault(a => a.id == achievementId);
            if (achievement != null)
            {
                unlockedAchievements.Add(achievementId);
                OnAchievementUnlocked?.Invoke(achievement);
                SaveProgress();

                Debug.Log($"🏆 Achievement Unlocked: {achievement.title}");
                GameEvents.Instance?.OnInfoMessage?.Invoke($"🏆 {achievement.title} débloqué !");
            }
        }

        public bool IsAchievementUnlocked(string achievementId)
        {
            return unlockedAchievements.Contains(achievementId);
        }

        public int GetTotalPoints()
        {
            return achievements
                .Where(a => unlockedAchievements.Contains(a.id))
                .Sum(a => a.points);
        }

        public float GetCompletionPercentage()
        {
            return (float)unlockedAchievements.Count / achievements.Count * 100f;
        }

        public List<Achievement> GetAllAchievements()
        {
            return achievements;
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            return achievements.Where(a => unlockedAchievements.Contains(a.id)).ToList();
        }

        private void SaveProgress()
        {
            string data = string.Join(",", unlockedAchievements);
            PlayerPrefs.SetString("UnlockedAchievements", data);
            PlayerPrefs.Save();
        }

        private void LoadProgress()
        {
            string data = PlayerPrefs.GetString("UnlockedAchievements", "");
            if (!string.IsNullOrEmpty(data))
            {
                unlockedAchievements = new HashSet<string>(data.Split(','));
            }
        }
    }

    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public string icon;
        public int points;
    }
}
