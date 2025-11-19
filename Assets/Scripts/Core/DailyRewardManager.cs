using UnityEngine;
using System;
using System.Collections.Generic;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages daily login rewards to increase player retention
    /// </summary>
    public class DailyRewardManager : MonoBehaviour
    {
        public static DailyRewardManager Instance { get; private set; }

        [Header("Daily Reward Settings")]
        [SerializeField] private bool enableDailyRewards = true;
        [SerializeField] private int maxStreakDays = 7;

        private int currentStreak = 0;
        private DateTime lastClaimDate;
        private bool hasClaimedToday = false;

        // Daily rewards configuration
        private List<DailyReward> dailyRewards = new List<DailyReward>();

        public event Action<DailyReward> OnRewardClaimed;
        public event Action<int> OnStreakBroken;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDailyRewards();
                LoadProgress();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            CheckDailyReward();
        }

        private void InitializeDailyRewards()
        {
            dailyRewards.Clear();

            // Day 1 - Welcome bonus
            dailyRewards.Add(new DailyReward
            {
                day = 1,
                rewardName = "Bonus Bienvenue",
                description = "Commencez votre aventure !",
                icon = "🎁",
                money = 500,
                food = 20,
                materials = 0,
                crystals = 0
            });

            // Day 2 - Resources
            dailyRewards.Add(new DailyReward
            {
                day = 2,
                rewardName = "Pack Ressources",
                description = "Développez votre prison",
                icon = "📦",
                money = 1000,
                food = 50,
                materials = 50,
                crystals = 0
            });

            // Day 3 - Crystals!
            dailyRewards.Add(new DailyReward
            {
                day = 3,
                rewardName = "Cristaux Gratuits",
                description = "Monnaie premium !",
                icon = "💎",
                money = 0,
                food = 0,
                materials = 0,
                crystals = 25
            });

            // Day 4 - Big money
            dailyRewards.Add(new DailyReward
            {
                day = 4,
                rewardName = "Jackpot",
                description = "Grosse somme d'argent",
                icon = "💰",
                money = 2000,
                food = 100,
                materials = 100,
                crystals = 0
            });

            // Day 5 - More crystals
            dailyRewards.Add(new DailyReward
            {
                day = 5,
                rewardName = "Cristaux Premium",
                description = "Encore plus de cristaux !",
                icon = "💎",
                money = 1500,
                food = 0,
                materials = 0,
                crystals = 50
            });

            // Day 6 - Mega pack
            dailyRewards.Add(new DailyReward
            {
                day = 6,
                rewardName = "Méga Pack",
                description = "Presque au bout !",
                icon = "🎁",
                money = 3000,
                food = 200,
                materials = 150,
                crystals = 25
            });

            // Day 7 - Ultimate reward
            dailyRewards.Add(new DailyReward
            {
                day = 7,
                rewardName = "Récompense Ultime",
                description = "7 jours consécutifs !",
                icon = "🏆",
                money = 5000,
                food = 300,
                materials = 250,
                crystals = 100,
                specialBonus = "Skin exclusif débloqué !"
            });
        }

        private void CheckDailyReward()
        {
            if (!enableDailyRewards) return;

            DateTime today = DateTime.Now.Date;

            // First time player
            if (lastClaimDate == DateTime.MinValue)
            {
                currentStreak = 0;
                hasClaimedToday = false;
                SaveProgress();
                ShowDailyRewardPopup();
                return;
            }

            // Check if already claimed today
            if (lastClaimDate.Date == today)
            {
                hasClaimedToday = true;
                return;
            }

            // Check if streak is broken (missed a day)
            TimeSpan timeSinceLastClaim = today - lastClaimDate.Date;
            if (timeSinceLastClaim.Days > 1)
            {
                // Streak broken
                int oldStreak = currentStreak;
                currentStreak = 0;
                OnStreakBroken?.Invoke(oldStreak);
                Debug.Log($"Daily reward streak broken. Was at {oldStreak} days.");
            }

            hasClaimedToday = false;
            ShowDailyRewardPopup();
        }

        public void ClaimDailyReward()
        {
            if (hasClaimedToday)
            {
                if (GameEvents.Instance != null)
                {
                    GameEvents.Instance.OnWarning?.Invoke("❌ Récompense déjà réclamée aujourd'hui");
                }
                return;
            }

            currentStreak++;
            hasClaimedToday = true;
            lastClaimDate = DateTime.Now.Date;

            // Get reward for current streak day (loop back after day 7)
            int rewardIndex = ((currentStreak - 1) % maxStreakDays);
            DailyReward reward = dailyRewards[rewardIndex];

            // Give rewards
            GiveReward(reward);

            // Save progress
            SaveProgress();

            // Track analytics
            AnalyticsManager.Instance?.TrackEvent("daily_reward_claimed", new Dictionary<string, object>
            {
                { "streak_day", currentStreak },
                { "reward_day", reward.day },
                { "crystals_earned", reward.crystals }
            });

            // Notify
            OnRewardClaimed?.Invoke(reward);

            Debug.Log($"Daily reward claimed! Day {currentStreak}, Reward: {reward.rewardName}");
        }

        private void GiveReward(DailyReward reward)
        {
            // Give resources
            if (reward.money > 0 || reward.food > 0 || reward.materials > 0)
            {
                GameManager.Instance.resources.Add(
                    moneyAmount: reward.money,
                    foodAmount: reward.food,
                    materialsAmount: reward.materials
                );
            }

            // Give crystals
            if (reward.crystals > 0 && MonetizationManager.Instance != null)
            {
                MonetizationManager.Instance.AddFreeCrystals(reward.crystals);
            }

            // Special bonus
            if (!string.IsNullOrEmpty(reward.specialBonus))
            {
                // Handle special rewards (skins, etc.)
                Debug.Log($"Special bonus: {reward.specialBonus}");
            }

            // Show notification
            string message = $"{reward.icon} {reward.rewardName} réclamé !\n";
            if (reward.money > 0) message += $"💰 +{reward.money}$ ";
            if (reward.food > 0) message += $"🍞 +{reward.food} ";
            if (reward.materials > 0) message += $"🧱 +{reward.materials} ";
            if (reward.crystals > 0) message += $"💎 +{reward.crystals} cristaux ";

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke(message);
            }
        }

        private void ShowDailyRewardPopup()
        {
            // This should show a UI popup
            // For now, just log
            int nextRewardDay = ((currentStreak) % maxStreakDays) + 1;
            Debug.Log($"Daily reward available! Current streak: {currentStreak}, Next reward: Day {nextRewardDay}");

            // Notify UI to show popup
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke("🎁 Récompense quotidienne disponible !");
            }
        }

        public bool CanClaimToday()
        {
            return enableDailyRewards && !hasClaimedToday;
        }

        public int GetCurrentStreak()
        {
            return currentStreak;
        }

        public DailyReward GetNextReward()
        {
            int nextIndex = (currentStreak % maxStreakDays);
            return dailyRewards[nextIndex];
        }

        public DailyReward GetRewardForDay(int day)
        {
            int index = Mathf.Clamp(day - 1, 0, dailyRewards.Count - 1);
            return dailyRewards[index];
        }

        public List<DailyReward> GetAllRewards()
        {
            return new List<DailyReward>(dailyRewards);
        }

        #region Save/Load

        private void SaveProgress()
        {
            PlayerPrefs.SetInt("DailyRewardStreak", currentStreak);
            PlayerPrefs.SetString("LastClaimDate", lastClaimDate.ToString());
            PlayerPrefs.SetInt("HasClaimedToday", hasClaimedToday ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadProgress()
        {
            currentStreak = PlayerPrefs.GetInt("DailyRewardStreak", 0);
            hasClaimedToday = PlayerPrefs.GetInt("HasClaimedToday", 0) == 1;

            string lastClaimStr = PlayerPrefs.GetString("LastClaimDate", DateTime.MinValue.ToString());
            if (!DateTime.TryParse(lastClaimStr, out lastClaimDate))
            {
                lastClaimDate = DateTime.MinValue;
            }

            Debug.Log($"Daily rewards loaded. Streak: {currentStreak}, Last claim: {lastClaimDate.ToShortDateString()}");
        }

        #endregion

        #region Admin/Debug

        [ContextMenu("Reset Daily Rewards")]
        public void ResetDailyRewards()
        {
            currentStreak = 0;
            lastClaimDate = DateTime.MinValue;
            hasClaimedToday = false;
            SaveProgress();
            Debug.Log("Daily rewards reset!");
        }

        [ContextMenu("Simulate Next Day")]
        public void SimulateNextDay()
        {
            lastClaimDate = DateTime.Now.Date.AddDays(-1);
            hasClaimedToday = false;
            SaveProgress();
            CheckDailyReward();
            Debug.Log("Simulated next day!");
        }

        #endregion
    }

    [System.Serializable]
    public class DailyReward
    {
        public int day;
        public string rewardName;
        public string description;
        public string icon;
        public float money;
        public float food;
        public float materials;
        public int crystals;
        public string specialBonus;

        public override string ToString()
        {
            return $"Day {day}: {rewardName} - 💰{money} 🍞{food} 🧱{materials} 💎{crystals}";
        }
    }
}
