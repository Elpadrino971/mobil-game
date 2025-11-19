using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace PrisonIsland.UI
{
    /// <summary>
    /// Manages the Daily Reward popup UI
    /// </summary>
    public class DailyRewardUI : MonoBehaviour
    {
        public static DailyRewardUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject dailyRewardPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI streakText;
        [SerializeField] private Button claimButton;
        [SerializeField] private Button closeButton;

        [Header("Reward Display")]
        [SerializeField] private Transform rewardItemsContainer;
        [SerializeField] private GameObject rewardItemPrefab;

        [Header("Day Indicators")]
        [SerializeField] private Transform dayIndicatorsContainer;
        [SerializeField] private GameObject dayIndicatorPrefab;

        private Core.DailyReward currentReward;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (claimButton != null)
                claimButton.onClick.AddListener(OnClaimButtonClicked);

            if (closeButton != null)
                closeButton.onClick.AddListener(CloseDailyRewardPanel);

            CloseDailyRewardPanel();

            // Subscribe to daily reward events
            if (Core.DailyRewardManager.Instance != null)
            {
                Core.DailyRewardManager.Instance.OnRewardClaimed += OnRewardClaimed;
            }
        }

        public void ShowDailyRewardPanel()
        {
            if (Core.DailyRewardManager.Instance == null) return;

            if (!Core.DailyRewardManager.Instance.CanClaimToday())
            {
                if (Core.GameEvents.Instance != null)
                {
                    Core.GameEvents.Instance.OnWarning?.Invoke("❌ Déjà réclamé aujourd'hui");
                }
                return;
            }

            currentReward = Core.DailyRewardManager.Instance.GetNextReward();
            int currentStreak = Core.DailyRewardManager.Instance.GetCurrentStreak();

            UpdateUI(currentReward, currentStreak);

            if (dailyRewardPanel != null)
                dailyRewardPanel.SetActive(true);

            // Track analytics
            Core.AnalyticsManager.Instance?.TrackScreen("daily_reward");
        }

        public void CloseDailyRewardPanel()
        {
            if (dailyRewardPanel != null)
                dailyRewardPanel.SetActive(false);
        }

        private void UpdateUI(Core.DailyReward reward, int streak)
        {
            // Update title
            if (titleText != null)
            {
                titleText.text = $"{reward.icon} {reward.rewardName}";
            }

            // Update description
            if (descriptionText != null)
            {
                descriptionText.text = reward.description;
            }

            // Update streak
            if (streakText != null)
            {
                streakText.text = $"Série: {streak + 1} jours 🔥";
            }

            // Update reward items
            UpdateRewardItems(reward);

            // Update day indicators
            UpdateDayIndicators(streak);
        }

        private void UpdateRewardItems(Core.DailyReward reward)
        {
            if (rewardItemsContainer == null) return;

            // Clear existing items
            foreach (Transform child in rewardItemsContainer)
            {
                Destroy(child.gameObject);
            }

            // Add reward items
            if (reward.money > 0)
            {
                CreateRewardItem("💰", $"{reward.money:F0}$");
            }

            if (reward.food > 0)
            {
                CreateRewardItem("🍞", $"{reward.food:F0}");
            }

            if (reward.materials > 0)
            {
                CreateRewardItem("🧱", $"{reward.materials:F0}");
            }

            if (reward.crystals > 0)
            {
                CreateRewardItem("💎", $"{reward.crystals} Cristaux");
            }

            if (!string.IsNullOrEmpty(reward.specialBonus))
            {
                CreateRewardItem("🎁", reward.specialBonus);
            }
        }

        private void CreateRewardItem(string icon, string text)
        {
            GameObject item;

            if (rewardItemPrefab != null)
            {
                item = Instantiate(rewardItemPrefab, rewardItemsContainer);
            }
            else
            {
                // Create simple reward item
                item = new GameObject("RewardItem");
                item.transform.SetParent(rewardItemsContainer);

                TextMeshProUGUI tmp = item.AddComponent<TextMeshProUGUI>();
                tmp.text = $"{icon} {text}";
                tmp.fontSize = 18;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;
            }
        }

        private void UpdateDayIndicators(int currentStreak)
        {
            if (dayIndicatorsContainer == null) return;

            // Clear existing indicators
            foreach (Transform child in dayIndicatorsContainer)
            {
                Destroy(child.gameObject);
            }

            // Get all rewards
            List<Core.DailyReward> allRewards = Core.DailyRewardManager.Instance.GetAllRewards();

            // Create day indicators
            for (int i = 0; i < allRewards.Count; i++)
            {
                CreateDayIndicator(i + 1, i < currentStreak, i == currentStreak);
            }
        }

        private void CreateDayIndicator(int day, bool claimed, bool current)
        {
            GameObject indicator;

            if (dayIndicatorPrefab != null)
            {
                indicator = Instantiate(dayIndicatorPrefab, dayIndicatorsContainer);

                // Update indicator visuals based on state
                Image img = indicator.GetComponent<Image>();
                if (img != null)
                {
                    if (claimed)
                        img.color = Color.green;
                    else if (current)
                        img.color = Color.yellow;
                    else
                        img.color = Color.gray;
                }

                TextMeshProUGUI text = indicator.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = day.ToString();
                }
            }
            else
            {
                // Create simple indicator
                indicator = new GameObject($"Day{day}");
                indicator.transform.SetParent(dayIndicatorsContainer);

                TextMeshProUGUI tmp = indicator.AddComponent<TextMeshProUGUI>();
                string status = claimed ? "✅" : (current ? "🔥" : "⭕");
                tmp.text = $"{status} {day}";
                tmp.fontSize = 16;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = claimed ? Color.green : (current ? Color.yellow : Color.gray);
            }
        }

        private void OnClaimButtonClicked()
        {
            if (Core.DailyRewardManager.Instance != null)
            {
                Core.DailyRewardManager.Instance.ClaimDailyReward();
                CloseDailyRewardPanel();
            }
        }

        private void OnRewardClaimed(Core.DailyReward reward)
        {
            // Show celebration effect
            if (Core.VFX.VisualEffectsManager.Instance != null)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                // Show particle effect at center of screen
            }

            // Play sound
            if (Core.AudioManager.Instance != null)
            {
                Core.AudioManager.Instance.PlaySFX("success");
            }
        }

        // Public method to open from other scripts
        public void OpenDailyReward()
        {
            ShowDailyRewardPanel();
        }

        private void OnDestroy()
        {
            if (Core.DailyRewardManager.Instance != null)
            {
                Core.DailyRewardManager.Instance.OnRewardClaimed -= OnRewardClaimed;
            }
        }
    }
}
