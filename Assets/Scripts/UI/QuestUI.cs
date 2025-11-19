using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace PrisonIsland.UI
{
    /// <summary>
    /// Manages the Quest/Mission UI panel
    /// </summary>
    public class QuestUI : MonoBehaviour
    {
        public static QuestUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject questPanel;
        [SerializeField] private Transform dailyQuestsContainer;
        [SerializeField] private Transform weeklyQuestsContainer;
        [SerializeField] private Transform storyQuestsContainer;
        [SerializeField] private GameObject questItemPrefab;

        [Header("Tabs")]
        [SerializeField] private Button dailyTab;
        [SerializeField] private Button weeklyTab;
        [SerializeField] private Button storyTab;

        [Header("Tab Panels")]
        [SerializeField] private GameObject dailyPanel;
        [SerializeField] private GameObject weeklyPanel;
        [SerializeField] private GameObject storyPanel;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        private QuestTabType currentTab = QuestTabType.Daily;

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
            SetupTabs();

            if (closeButton != null)
                closeButton.onClick.AddListener(CloseQuestPanel);

            CloseQuestPanel();

            // Subscribe to quest events
            if (Core.QuestManager.Instance != null)
            {
                Core.QuestManager.Instance.OnQuestCompleted += OnQuestCompleted;
                Core.QuestManager.Instance.OnQuestProgressed += OnQuestProgressed;
                Core.QuestManager.Instance.OnDailyQuestsRefreshed += RefreshQuests;
                Core.QuestManager.Instance.OnWeeklyQuestsRefreshed += RefreshQuests;
            }
        }

        private void SetupTabs()
        {
            if (dailyTab != null)
                dailyTab.onClick.AddListener(() => ShowTab(QuestTabType.Daily));

            if (weeklyTab != null)
                weeklyTab.onClick.AddListener(() => ShowTab(QuestTabType.Weekly));

            if (storyTab != null)
                storyTab.onClick.AddListener(() => ShowTab(QuestTabType.Story));
        }

        public void OpenQuestPanel()
        {
            if (questPanel != null)
            {
                questPanel.SetActive(true);
                ShowTab(currentTab);
                RefreshQuests();

                // Track analytics
                if (Core.AnalyticsManager.Instance != null)
                {
                    Core.AnalyticsManager.Instance.TrackScreen("quests");
                }
            }
        }

        public void CloseQuestPanel()
        {
            if (questPanel != null)
                questPanel.SetActive(false);
        }

        private void ShowTab(QuestTabType tab)
        {
            currentTab = tab;

            // Hide all panels
            if (dailyPanel != null) dailyPanel.SetActive(false);
            if (weeklyPanel != null) weeklyPanel.SetActive(false);
            if (storyPanel != null) storyPanel.SetActive(false);

            // Show selected panel
            switch (tab)
            {
                case QuestTabType.Daily:
                    if (dailyPanel != null) dailyPanel.SetActive(true);
                    RefreshDailyQuests();
                    break;
                case QuestTabType.Weekly:
                    if (weeklyPanel != null) weeklyPanel.SetActive(true);
                    RefreshWeeklyQuests();
                    break;
                case QuestTabType.Story:
                    if (storyPanel != null) storyPanel.SetActive(true);
                    RefreshStoryQuests();
                    break;
            }
        }

        private void RefreshQuests()
        {
            RefreshDailyQuests();
            RefreshWeeklyQuests();
            RefreshStoryQuests();
        }

        private void RefreshDailyQuests()
        {
            if (Core.QuestManager.Instance == null || dailyQuestsContainer == null) return;

            ClearContainer(dailyQuestsContainer);

            List<Core.Quest> quests = Core.QuestManager.Instance.dailyQuests;
            foreach (var quest in quests)
            {
                if (quest.isActive)
                {
                    CreateQuestItem(quest, dailyQuestsContainer);
                }
            }
        }

        private void RefreshWeeklyQuests()
        {
            if (Core.QuestManager.Instance == null || weeklyQuestsContainer == null) return;

            ClearContainer(weeklyQuestsContainer);

            List<Core.Quest> quests = Core.QuestManager.Instance.weeklyQuests;
            foreach (var quest in quests)
            {
                if (quest.isActive)
                {
                    CreateQuestItem(quest, weeklyQuestsContainer);
                }
            }
        }

        private void RefreshStoryQuests()
        {
            if (Core.QuestManager.Instance == null || storyQuestsContainer == null) return;

            ClearContainer(storyQuestsContainer);

            List<Core.Quest> quests = Core.QuestManager.Instance.storyQuests;
            foreach (var quest in quests)
            {
                if (quest.isActive)
                {
                    CreateQuestItem(quest, storyQuestsContainer);
                }
            }
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }

        private void CreateQuestItem(Core.Quest quest, Transform container)
        {
            GameObject item;

            if (questItemPrefab != null)
            {
                item = Instantiate(questItemPrefab, container);

                // Update quest item UI
                TextMeshProUGUI titleText = item.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
                if (titleText != null)
                {
                    titleText.text = quest.questName;
                }

                TextMeshProUGUI descText = item.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
                if (descText != null)
                {
                    descText.text = quest.description;
                }

                Slider progressBar = item.transform.Find("ProgressBar")?.GetComponent<Slider>();
                if (progressBar != null)
                {
                    progressBar.value = quest.GetProgressPercentage() / 100f;
                }

                TextMeshProUGUI progressText = item.transform.Find("ProgressText")?.GetComponent<TextMeshProUGUI>();
                if (progressText != null)
                {
                    progressText.text = $"{quest.currentProgress:F0} / {quest.targetValue:F0}";
                }

                TextMeshProUGUI rewardText = item.transform.Find("Reward")?.GetComponent<TextMeshProUGUI>();
                if (rewardText != null)
                {
                    rewardText.text = GetRewardText(quest.rewards);
                }

                // Mark completed quests
                if (quest.isCompleted)
                {
                    Image background = item.GetComponent<Image>();
                    if (background != null)
                    {
                        background.color = new Color(0, 1, 0, 0.3f); // Green tint
                    }
                }
            }
            else
            {
                // Create simple quest item
                item = new GameObject($"Quest_{quest.id}");
                item.transform.SetParent(container);

                TextMeshProUGUI text = item.AddComponent<TextMeshProUGUI>();

                string status = quest.isCompleted ? "✅" : "🔲";
                string progress = $"{quest.currentProgress:F0}/{quest.targetValue:F0}";
                string rewards = GetRewardText(quest.rewards);

                text.text = $"{status} {quest.questName}\n{quest.description}\n{progress} | {rewards}";
                text.fontSize = 14;
                text.color = quest.isCompleted ? Color.green : Color.white;
            }
        }

        private string GetRewardText(Core.QuestReward rewards)
        {
            List<string> parts = new List<string>();

            if (rewards.money > 0)
                parts.Add($"💰 {rewards.money:F0}$");

            if (rewards.food > 0)
                parts.Add($"🍞 {rewards.food:F0}");

            if (rewards.materials > 0)
                parts.Add($"🧱 {rewards.materials:F0}");

            if (rewards.crystals > 0)
                parts.Add($"💎 {rewards.crystals}");

            if (!string.IsNullOrEmpty(rewards.specialItem))
                parts.Add($"🎁 {rewards.specialItem}");

            return string.Join(" | ", parts);
        }

        private void OnQuestCompleted(Core.Quest quest)
        {
            // Refresh the UI when a quest is completed
            RefreshQuests();

            // Show celebration effect
            // Can be implemented later with VFX system

            // Play sound
            if (Core.AudioManager.Instance != null)
            {
                Core.AudioManager.Instance.PlaySFX("success");
            }
        }

        private void OnQuestProgressed(Core.Quest quest)
        {
            // Update progress bar in real-time
            RefreshQuests();
        }

        private void OnDestroy()
        {
            if (Core.QuestManager.Instance != null)
            {
                Core.QuestManager.Instance.OnQuestCompleted -= OnQuestCompleted;
                Core.QuestManager.Instance.OnQuestProgressed -= OnQuestProgressed;
                Core.QuestManager.Instance.OnDailyQuestsRefreshed -= RefreshQuests;
                Core.QuestManager.Instance.OnWeeklyQuestsRefreshed -= RefreshQuests;
            }
        }

        private enum QuestTabType
        {
            Daily,
            Weekly,
            Story
        }
    }
}
