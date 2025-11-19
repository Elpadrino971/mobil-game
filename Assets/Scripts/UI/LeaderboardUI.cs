using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace PrisonIsland.UI
{
    /// <summary>
    /// Manages the Leaderboard UI panel
    /// </summary>
    public class LeaderboardUI : MonoBehaviour
    {
        public static LeaderboardUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private Transform leaderboardContent;
        [SerializeField] private GameObject leaderboardEntryPrefab;

        [Header("Leaderboard Type Selection")]
        [SerializeField] private TMP_Dropdown leaderboardTypeDropdown;

        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI playerRankText;
        [SerializeField] private TextMeshProUGUI playerScoreText;

        [Header("Buttons")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button closeButton;

        private Core.LeaderboardType currentLeaderboardType = Core.LeaderboardType.TotalMoney;

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
            if (refreshButton != null)
                refreshButton.onClick.AddListener(RefreshLeaderboard);

            if (closeButton != null)
                closeButton.onClick.AddListener(CloseLeaderboard);

            if (leaderboardTypeDropdown != null)
            {
                SetupLeaderboardTypeDropdown();
                leaderboardTypeDropdown.onValueChanged.AddListener(OnLeaderboardTypeChanged);
            }

            CloseLeaderboard();
        }

        public void OpenLeaderboard()
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(true);
                RefreshLeaderboard();

                // Track analytics
                if (Core.AnalyticsManager.Instance != null)
                {
                    Core.AnalyticsManager.Instance.TrackScreen("leaderboard");
                }
            }
        }

        public void CloseLeaderboard()
        {
            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(false);
        }

        private void SetupLeaderboardTypeDropdown()
        {
            leaderboardTypeDropdown.ClearOptions();

            List<string> options = new List<string>();

            if (Core.LocalizationManager.Instance != null)
            {
                options.Add(Core.LocalizationManager.Instance.GetText("leaderboard_total_money"));
                options.Add(Core.LocalizationManager.Instance.GetText("leaderboard_population"));
                options.Add(Core.LocalizationManager.Instance.GetText("leaderboard_buildings"));
                options.Add(Core.LocalizationManager.Instance.GetText("leaderboard_days_survived"));
            }
            else
            {
                options.Add("Total Money");
                options.Add("Prison Population");
                options.Add("Total Buildings");
                options.Add("Days Survived");
            }

            options.Add("Security Rating");
            options.Add("Total Prisoners Managed");
            options.Add("Escapes Prevented");
            options.Add("Achievement Points");

            leaderboardTypeDropdown.AddOptions(options);
        }

        private void OnLeaderboardTypeChanged(int index)
        {
            currentLeaderboardType = (Core.LeaderboardType)index;
            RefreshLeaderboard();
        }

        public void RefreshLeaderboard()
        {
            if (Core.LeaderboardManager.Instance == null) return;

            // Clear existing entries
            foreach (Transform child in leaderboardContent)
            {
                Destroy(child.gameObject);
            }

            // Get leaderboard entries
            List<Core.LeaderboardEntry> entries = Core.LeaderboardManager.Instance.GetLeaderboard(
                currentLeaderboardType, 10);

            // Create UI entries
            for (int i = 0; i < entries.Count; i++)
            {
                CreateLeaderboardEntry(entries[i], i + 1);
            }

            // Update player info
            UpdatePlayerInfo();
        }

        private void CreateLeaderboardEntry(Core.LeaderboardEntry entry, int rank)
        {
            GameObject entryObj;

            if (leaderboardEntryPrefab != null)
            {
                entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            }
            else
            {
                // Create simple entry
                entryObj = new GameObject($"Entry_{rank}");
                entryObj.transform.SetParent(leaderboardContent);

                TextMeshProUGUI text = entryObj.AddComponent<TextMeshProUGUI>();
                text.text = $"{rank}. {entry.playerName} - {entry.score:N0}";
                text.fontSize = 16;
                text.color = rank <= 3 ? Color.yellow : Color.white;
            }

            // Highlight player's entry
            if (entry.playerId == Core.LeaderboardManager.Instance.playerId)
            {
                TextMeshProUGUI text = entryObj.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    text.color = Color.green;
                    text.fontStyle = FontStyles.Bold;
                }
            }
        }

        private void UpdatePlayerInfo()
        {
            if (Core.LeaderboardManager.Instance == null) return;

            int rank = Core.LeaderboardManager.Instance.GetPlayerRank(currentLeaderboardType);
            long score = Core.LeaderboardManager.Instance.GetPlayerBestScore(currentLeaderboardType);

            if (playerRankText != null)
            {
                string rankText = rank > 0 ? $"#{rank}" : "Unranked";
                string rankLabel = "Rank";
                if (Core.LocalizationManager.Instance != null)
                {
                    rankLabel = Core.LocalizationManager.Instance.GetText("leaderboard_rank");
                }
                playerRankText.text = $"{rankLabel}: {rankText}";
            }

            if (playerScoreText != null)
            {
                string scoreLabel = "Score";
                if (Core.LocalizationManager.Instance != null)
                {
                    scoreLabel = Core.LocalizationManager.Instance.GetText("leaderboard_score");
                }
                playerScoreText.text = $"{scoreLabel}: {score:N0}";
            }
        }
    }
}
