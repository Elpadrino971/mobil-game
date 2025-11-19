using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages daily quests, weekly missions, and challenges
    /// Provides progression and rewards to keep players engaged
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quest Settings")]
        [SerializeField] private int maxDailyQuests = 3;
        [SerializeField] private int maxWeeklyQuests = 5;

        [Header("Quest Lists")]
        public List<Quest> dailyQuests = new List<Quest>();
        public List<Quest> weeklyQuests = new List<Quest>();
        public List<Quest> storyQuests = new List<Quest>();

        private DateTime lastDailyReset;
        private DateTime lastWeeklyReset;

        // Events
        public event Action<Quest> OnQuestCompleted;
        public event Action<Quest> OnQuestProgressed;
        public event Action OnDailyQuestsRefreshed;
        public event Action OnWeeklyQuestsRefreshed;

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
        }

        private void Start()
        {
            LoadQuestData();
            InitializeQuests();
            CheckQuestResets();
            SubscribeToGameEvents();
        }

        #region Initialization

        private void InitializeQuests()
        {
            InitializeDailyQuests();
            InitializeWeeklyQuests();
            InitializeStoryQuests();
        }

        private void InitializeDailyQuests()
        {
            if (dailyQuests.Count == 0)
            {
                GenerateDailyQuests();
            }
        }

        private void InitializeWeeklyQuests()
        {
            if (weeklyQuests.Count == 0)
            {
                GenerateWeeklyQuests();
            }
        }

        private void InitializeStoryQuests()
        {
            // Story quests are permanent and guide player progression
            if (storyQuests.Count == 0)
            {
                storyQuests.Add(new Quest
                {
                    id = "story_001",
                    questName = "Welcome to Prison Island",
                    description = "Build your first cell block",
                    questType = QuestType.Story,
                    targetType = QuestTargetType.BuildSpecific,
                    targetValue = 1,
                    currentProgress = 0,
                    rewards = new QuestReward { money = 1000, crystals = 10 },
                    isActive = true
                });

                storyQuests.Add(new Quest
                {
                    id = "story_002",
                    questName = "First Inmates",
                    description = "Receive 5 prisoners",
                    questType = QuestType.Story,
                    targetType = QuestTargetType.ReceivePrisoners,
                    targetValue = 5,
                    rewards = new QuestReward { money = 2000, food = 100 }
                });

                storyQuests.Add(new Quest
                {
                    id = "story_003",
                    questName = "Security First",
                    description = "Build a Guard Tower",
                    questType = QuestType.Story,
                    targetType = QuestTargetType.BuildSpecific,
                    targetValue = 1,
                    rewards = new QuestReward { money = 5000, crystals = 25 }
                });

                storyQuests.Add(new Quest
                {
                    id = "story_004",
                    questName = "Prevent Escapes",
                    description = "Reach 75% security rating",
                    questType = QuestType.Story,
                    targetType = QuestTargetType.ReachSecurity,
                    targetValue = 75,
                    rewards = new QuestReward { crystals = 50 }
                });

                storyQuests.Add(new Quest
                {
                    id = "story_005",
                    questName = "Prison Tycoon",
                    description = "Earn $50,000 total",
                    questType = QuestType.Story,
                    targetType = QuestTargetType.EarnMoney,
                    targetValue = 50000,
                    rewards = new QuestReward { crystals = 100, specialItem = "Golden Trophy" }
                });
            }
        }

        #endregion

        #region Daily Quests

        private void GenerateDailyQuests()
        {
            dailyQuests.Clear();

            // Pool of possible daily quests
            List<Quest> questPool = new List<Quest>
            {
                new Quest
                {
                    id = "daily_build",
                    questName = "Build Something",
                    description = "Construct 2 buildings",
                    questType = QuestType.Daily,
                    targetType = QuestTargetType.BuildAny,
                    targetValue = 2,
                    rewards = new QuestReward { money = 500, food = 20 }
                },
                new Quest
                {
                    id = "daily_earn",
                    questName = "Make Money",
                    description = "Earn $5,000",
                    questType = QuestType.Daily,
                    targetType = QuestTargetType.EarnMoney,
                    targetValue = 5000,
                    rewards = new QuestReward { crystals = 5 }
                },
                new Quest
                {
                    id = "daily_prisoners",
                    questName = "Accept Inmates",
                    description = "Receive 3 new prisoners",
                    questType = QuestType.Daily,
                    targetType = QuestTargetType.ReceivePrisoners,
                    targetValue = 3,
                    rewards = new QuestReward { money = 1000 }
                },
                new Quest
                {
                    id = "daily_security",
                    questName = "Stay Safe",
                    description = "Keep security above 60% for 1 day",
                    questType = QuestType.Daily,
                    targetType = QuestTargetType.MaintainSecurity,
                    targetValue = 60,
                    rewards = new QuestReward { money = 800, materials = 30 }
                },
                new Quest
                {
                    id = "daily_no_escape",
                    questName = "Lockdown",
                    description = "Prevent any escapes today",
                    questType = QuestType.Daily,
                    targetType = QuestTargetType.PreventEscapes,
                    targetValue = 0,
                    rewards = new QuestReward { crystals = 10 }
                }
            };

            // Randomly select quests
            questPool = questPool.OrderBy(x => UnityEngine.Random.value).ToList();
            for (int i = 0; i < Mathf.Min(maxDailyQuests, questPool.Count); i++)
            {
                Quest quest = questPool[i];
                quest.isActive = true;
                quest.currentProgress = 0;
                dailyQuests.Add(quest);
            }

            lastDailyReset = DateTime.Now;
            SaveQuestData();
            OnDailyQuestsRefreshed?.Invoke();

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke("📋 Nouvelles quêtes quotidiennes disponibles!");
            }
        }

        #endregion

        #region Weekly Quests

        private void GenerateWeeklyQuests()
        {
            weeklyQuests.Clear();

            List<Quest> questPool = new List<Quest>
            {
                new Quest
                {
                    id = "weekly_expand",
                    questName = "Expansion",
                    description = "Build 10 buildings",
                    questType = QuestType.Weekly,
                    targetType = QuestTargetType.BuildAny,
                    targetValue = 10,
                    rewards = new QuestReward { money = 5000, crystals = 25 }
                },
                new Quest
                {
                    id = "weekly_population",
                    questName = "Full House",
                    description = "Reach 50 prisoners",
                    questType = QuestType.Weekly,
                    targetType = QuestTargetType.ReachPopulation,
                    targetValue = 50,
                    rewards = new QuestReward { crystals = 50 }
                },
                new Quest
                {
                    id = "weekly_profit",
                    questName = "Profitable Week",
                    description = "Earn $50,000 this week",
                    questType = QuestType.Weekly,
                    targetType = QuestTargetType.EarnMoney,
                    targetValue = 50000,
                    rewards = new QuestReward { crystals = 75 }
                },
                new Quest
                {
                    id = "weekly_survive",
                    questName = "Week Survivor",
                    description = "Survive 7 days without bankruptcy",
                    questType = QuestType.Weekly,
                    targetType = QuestTargetType.SurviveDays,
                    targetValue = 7,
                    rewards = new QuestReward { money = 10000, crystals = 30 }
                },
                new Quest
                {
                    id = "weekly_fortress",
                    questName = "Fortress",
                    description = "Build 5 Guard Towers",
                    questType = QuestType.Weekly,
                    targetType = QuestTargetType.BuildSpecific,
                    targetValue = 5,
                    rewards = new QuestReward { crystals = 40, materials = 200 }
                }
            };

            weeklyQuests = questPool.OrderBy(x => UnityEngine.Random.value).Take(maxWeeklyQuests).ToList();
            foreach (var quest in weeklyQuests)
            {
                quest.isActive = true;
                quest.currentProgress = 0;
            }

            lastWeeklyReset = DateTime.Now;
            SaveQuestData();
            OnWeeklyQuestsRefreshed?.Invoke();

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke("📋 Nouvelles missions hebdomadaires!");
            }
        }

        #endregion

        #region Quest Progress Tracking

        public void UpdateQuestProgress(QuestTargetType targetType, float progress)
        {
            // Update all active quests of this type
            UpdateQuestList(dailyQuests, targetType, progress);
            UpdateQuestList(weeklyQuests, targetType, progress);
            UpdateQuestList(storyQuests, targetType, progress);
        }

        private void UpdateQuestList(List<Quest> quests, QuestTargetType targetType, float progress)
        {
            foreach (Quest quest in quests)
            {
                if (quest.isActive && !quest.isCompleted && quest.targetType == targetType)
                {
                    quest.currentProgress += progress;

                    OnQuestProgressed?.Invoke(quest);

                    // Check if completed
                    if (quest.currentProgress >= quest.targetValue)
                    {
                        CompleteQuest(quest);
                    }

                    SaveQuestData();
                }
            }
        }

        private void CompleteQuest(Quest quest)
        {
            quest.isCompleted = true;
            quest.completedDate = DateTime.Now;

            // Give rewards
            GiveQuestRewards(quest.rewards);

            OnQuestCompleted?.Invoke(quest);

            // Track analytics
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.TrackEvent("quest_completed", new Dictionary<string, object>
                {
                    { "quest_id", quest.id },
                    { "quest_type", quest.questType.ToString() },
                    { "quest_name", quest.questName }
                });
            }

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke($"✅ Quête terminée: {quest.questName}!");
            }

            SaveQuestData();
        }

        private void GiveQuestRewards(QuestReward rewards)
        {
            if (GameManager.Instance != null)
            {
                if (rewards.money > 0)
                {
                    GameManager.Instance.resources.money += rewards.money;
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnInfoMessage?.Invoke($"💰 +{rewards.money:F0}$");
                    }
                }

                if (rewards.food > 0)
                {
                    GameManager.Instance.resources.food += rewards.food;
                }

                if (rewards.materials > 0)
                {
                    GameManager.Instance.resources.materials += rewards.materials;
                }
            }

            if (rewards.crystals > 0 && MonetizationManager.Instance != null)
            {
                MonetizationManager.Instance.AddFreeCrystals(rewards.crystals);
            }

            if (!string.IsNullOrEmpty(rewards.specialItem) && GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke($"🎁 Objet spécial reçu: {rewards.specialItem}");
            }
        }

        #endregion

        #region Game Event Subscriptions

        private void SubscribeToGameEvents()
        {
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnBuildingConstructed += OnBuildingConstructed;
                GameEvents.Instance.OnPrisonerArrived += OnPrisonerArrived;
                GameEvents.Instance.OnPrisonerEscaped += OnPrisonerEscaped;
                GameEvents.Instance.OnNewDay += OnNewDay;
            }
        }

        private void OnBuildingConstructed(Buildings.Building building)
        {
            UpdateQuestProgress(QuestTargetType.BuildAny, 1);

            // Check specific building types
            if (building.data != null && building.data.buildingName.Contains("Guard Tower"))
            {
                UpdateQuestProgress(QuestTargetType.BuildSpecific, 1);
            }
        }

        private void OnPrisonerArrived(Prisoners.Prisoner prisoner)
        {
            UpdateQuestProgress(QuestTargetType.ReceivePrisoners, 1);

            // Check population milestones
            if (GameManager.Instance != null)
            {
                int population = GameManager.Instance.prisoners.Count;
                if (population >= 50)
                {
                    UpdateQuestProgress(QuestTargetType.ReachPopulation, population);
                }
            }
        }

        private void OnPrisonerEscaped(Prisoners.Prisoner prisoner)
        {
            // This might fail "No Escape" quests
            // We track escapes separately
        }

        private void OnNewDay(int day)
        {
            // Track daily survival
            UpdateQuestProgress(QuestTargetType.SurviveDays, 1);

            // Check security maintenance
            if (GameManager.Instance != null && GameManager.Instance.resources.security >= 60)
            {
                UpdateQuestProgress(QuestTargetType.MaintainSecurity, 1);
            }

            // Check money earned
            if (GameManager.Instance != null)
            {
                float dailyIncome = GameManager.Instance.resources.GetDailyIncome();
                UpdateQuestProgress(QuestTargetType.EarnMoney, dailyIncome);
            }
        }

        #endregion

        #region Quest Resets

        private void CheckQuestResets()
        {
            // Check daily reset
            if ((DateTime.Now - lastDailyReset).TotalDays >= 1)
            {
                GenerateDailyQuests();
            }

            // Check weekly reset
            if ((DateTime.Now - lastWeeklyReset).TotalDays >= 7)
            {
                GenerateWeeklyQuests();
            }
        }

        #endregion

        #region Save/Load

        private void SaveQuestData()
        {
            // Save daily quests
            string dailyJson = JsonUtility.ToJson(new QuestListWrapper { quests = dailyQuests });
            PlayerPrefs.SetString("DailyQuests", dailyJson);
            PlayerPrefs.SetString("LastDailyReset", lastDailyReset.ToString());

            // Save weekly quests
            string weeklyJson = JsonUtility.ToJson(new QuestListWrapper { quests = weeklyQuests });
            PlayerPrefs.SetString("WeeklyQuests", weeklyJson);
            PlayerPrefs.SetString("LastWeeklyReset", lastWeeklyReset.ToString());

            // Save story quests
            string storyJson = JsonUtility.ToJson(new QuestListWrapper { quests = storyQuests });
            PlayerPrefs.SetString("StoryQuests", storyJson);

            PlayerPrefs.Save();
        }

        private void LoadQuestData()
        {
            // Load daily quests
            if (PlayerPrefs.HasKey("DailyQuests"))
            {
                string json = PlayerPrefs.GetString("DailyQuests");
                QuestListWrapper wrapper = JsonUtility.FromJson<QuestListWrapper>(json);
                if (wrapper?.quests != null)
                {
                    dailyQuests = wrapper.quests;
                }
            }

            // Load weekly quests
            if (PlayerPrefs.HasKey("WeeklyQuests"))
            {
                string json = PlayerPrefs.GetString("WeeklyQuests");
                QuestListWrapper wrapper = JsonUtility.FromJson<QuestListWrapper>(json);
                if (wrapper?.quests != null)
                {
                    weeklyQuests = wrapper.quests;
                }
            }

            // Load story quests
            if (PlayerPrefs.HasKey("StoryQuests"))
            {
                string json = PlayerPrefs.GetString("StoryQuests");
                QuestListWrapper wrapper = JsonUtility.FromJson<QuestListWrapper>(json);
                if (wrapper?.quests != null)
                {
                    storyQuests = wrapper.quests;
                }
            }

            // Load reset dates
            string dailyResetStr = PlayerPrefs.GetString("LastDailyReset", DateTime.Now.ToString());
            DateTime.TryParse(dailyResetStr, out lastDailyReset);

            string weeklyResetStr = PlayerPrefs.GetString("LastWeeklyReset", DateTime.Now.ToString());
            DateTime.TryParse(weeklyResetStr, out lastWeeklyReset);
        }

        [ContextMenu("Reset All Quests")]
        public void ResetAllQuests()
        {
            PlayerPrefs.DeleteKey("DailyQuests");
            PlayerPrefs.DeleteKey("WeeklyQuests");
            PlayerPrefs.DeleteKey("StoryQuests");
            dailyQuests.Clear();
            weeklyQuests.Clear();
            storyQuests.Clear();
            InitializeQuests();
            Debug.Log("All quests reset!");
        }

        #endregion

        #region Public Helper Methods

        public List<Quest> GetActiveQuests()
        {
            List<Quest> active = new List<Quest>();
            active.AddRange(dailyQuests.Where(q => q.isActive && !q.isCompleted));
            active.AddRange(weeklyQuests.Where(q => q.isActive && !q.isCompleted));
            active.AddRange(storyQuests.Where(q => q.isActive && !q.isCompleted));
            return active;
        }

        public int GetCompletedQuestsCount()
        {
            int count = 0;
            count += dailyQuests.Count(q => q.isCompleted);
            count += weeklyQuests.Count(q => q.isCompleted);
            count += storyQuests.Count(q => q.isCompleted);
            return count;
        }

        #endregion

        private void OnDestroy()
        {
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnBuildingConstructed -= OnBuildingConstructed;
                GameEvents.Instance.OnPrisonerArrived -= OnPrisonerArrived;
                GameEvents.Instance.OnPrisonerEscaped -= OnPrisonerEscaped;
                GameEvents.Instance.OnNewDay -= OnNewDay;
            }
        }
    }

    #region Data Structures

    [Serializable]
    public class Quest
    {
        public string id;
        public string questName;
        public string description;
        public QuestType questType;
        public QuestTargetType targetType;
        public float targetValue;
        public float currentProgress;
        public QuestReward rewards;
        public bool isActive;
        public bool isCompleted;
        public DateTime completedDate;

        public float GetProgressPercentage()
        {
            return Mathf.Clamp01(currentProgress / targetValue) * 100f;
        }
    }

    [Serializable]
    public class QuestReward
    {
        public float money;
        public float food;
        public float materials;
        public int crystals;
        public string specialItem;
    }

    [Serializable]
    public class QuestListWrapper
    {
        public List<Quest> quests;
    }

    public enum QuestType
    {
        Daily,      // Resets every day
        Weekly,     // Resets every week
        Story       // Permanent progression quests
    }

    public enum QuestTargetType
    {
        BuildAny,           // Build any X buildings
        BuildSpecific,      // Build specific building type
        ReceivePrisoners,   // Receive X prisoners
        PreventEscapes,     // Prevent escapes for X days
        EarnMoney,          // Earn X money
        ReachSecurity,      // Reach X security rating
        ReachPopulation,    // Reach X prisoner population
        MaintainSecurity,   // Maintain security above X for Y days
        SurviveDays         // Survive X days
    }

    #endregion
}
