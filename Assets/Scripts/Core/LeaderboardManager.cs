using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages global and friend leaderboards
    /// Supports multiple leaderboard types
    /// Ready for backend integration (Firebase, PlayFab, etc.)
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance { get; private set; }

        [Header("Leaderboard Settings")]
        [SerializeField] private int maxEntriesPerBoard = 100;
        [SerializeField] private bool enableLocalLeaderboards = true; // For testing without backend

        [Header("Player Info")]
        public string playerName = "Player";
        public string playerId;

        // Local leaderboards (for testing/offline mode)
        private Dictionary<LeaderboardType, List<LeaderboardEntry>> localLeaderboards;

        // Events
        public event Action<LeaderboardType, int> OnPlayerRankChanged;
        public event Action<LeaderboardEntry> OnNewHighScore;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeLeaderboards();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadPlayerInfo();
            if (enableLocalLeaderboards)
            {
                LoadLocalLeaderboards();
            }
        }

        private void InitializeLeaderboards()
        {
            localLeaderboards = new Dictionary<LeaderboardType, List<LeaderboardEntry>>();

            foreach (LeaderboardType type in Enum.GetValues(typeof(LeaderboardType)))
            {
                localLeaderboards[type] = new List<LeaderboardEntry>();
            }
        }

        #region Submit Scores

        /// <summary>
        /// Submit a score to a specific leaderboard
        /// </summary>
        public void SubmitScore(LeaderboardType type, long score)
        {
            if (string.IsNullOrEmpty(playerId))
            {
                playerId = SystemInfo.deviceUniqueIdentifier;
            }

            LeaderboardEntry entry = new LeaderboardEntry
            {
                playerId = playerId,
                playerName = playerName,
                score = score,
                timestamp = DateTime.Now
            };

            if (enableLocalLeaderboards)
            {
                SubmitToLocalLeaderboard(type, entry);
            }

            // TODO: Submit to backend (Firebase, PlayFab, etc.)
            // SubmitToBackend(type, entry);

            // Track analytics
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.TrackEvent("leaderboard_score_submitted", new Dictionary<string, object>
                {
                    { "leaderboard_type", type.ToString() },
                    { "score", score },
                    { "player_name", playerName }
                });
            }
        }

        private void SubmitToLocalLeaderboard(LeaderboardType type, LeaderboardEntry entry)
        {
            List<LeaderboardEntry> board = localLeaderboards[type];

            // Check if player already has an entry
            LeaderboardEntry existingEntry = board.FirstOrDefault(e => e.playerId == entry.playerId);

            if (existingEntry != null)
            {
                // Update if new score is better
                if (entry.score > existingEntry.score)
                {
                    existingEntry.score = entry.score;
                    existingEntry.timestamp = entry.timestamp;

                    OnNewHighScore?.Invoke(entry);
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnInfoMessage?.Invoke($"🏆 Nouveau record : {entry.score:N0}!");
                    }
                }
            }
            else
            {
                // Add new entry
                board.Add(entry);
                OnNewHighScore?.Invoke(entry);
            }

            // Sort by score (descending)
            board.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim to max entries
            if (board.Count > maxEntriesPerBoard)
            {
                board.RemoveRange(maxEntriesPerBoard, board.Count - maxEntriesPerBoard);
            }

            // Update player rank
            int rank = GetPlayerRank(type);
            OnPlayerRankChanged?.Invoke(type, rank);

            SaveLocalLeaderboards();
        }

        #endregion

        #region Get Leaderboards

        /// <summary>
        /// Get top entries from a leaderboard
        /// </summary>
        public List<LeaderboardEntry> GetLeaderboard(LeaderboardType type, int maxEntries = 10)
        {
            if (enableLocalLeaderboards && localLeaderboards.ContainsKey(type))
            {
                return localLeaderboards[type].Take(maxEntries).ToList();
            }

            // TODO: Fetch from backend
            return new List<LeaderboardEntry>();
        }

        /// <summary>
        /// Get entries around the player (player + neighbors)
        /// </summary>
        public List<LeaderboardEntry> GetLeaderboardAroundPlayer(LeaderboardType type, int range = 5)
        {
            if (!enableLocalLeaderboards || !localLeaderboards.ContainsKey(type))
                return new List<LeaderboardEntry>();

            List<LeaderboardEntry> board = localLeaderboards[type];
            int playerIndex = board.FindIndex(e => e.playerId == playerId);

            if (playerIndex == -1)
                return board.Take(range * 2).ToList();

            int start = Mathf.Max(0, playerIndex - range);
            int count = Mathf.Min(range * 2 + 1, board.Count - start);

            return board.GetRange(start, count);
        }

        /// <summary>
        /// Get player's current rank
        /// </summary>
        public int GetPlayerRank(LeaderboardType type)
        {
            if (!enableLocalLeaderboards || !localLeaderboards.ContainsKey(type))
                return -1;

            List<LeaderboardEntry> board = localLeaderboards[type];
            int index = board.FindIndex(e => e.playerId == playerId);

            return index >= 0 ? index + 1 : -1; // Rank is 1-indexed
        }

        /// <summary>
        /// Get player's best score for a leaderboard
        /// </summary>
        public long GetPlayerBestScore(LeaderboardType type)
        {
            if (!enableLocalLeaderboards || !localLeaderboards.ContainsKey(type))
                return 0;

            LeaderboardEntry entry = localLeaderboards[type].FirstOrDefault(e => e.playerId == playerId);
            return entry?.score ?? 0;
        }

        #endregion

        #region Auto-Submit from Game Stats

        /// <summary>
        /// Auto-submit scores based on game state
        /// Call this at the end of each game day or when milestones are reached
        /// </summary>
        public void UpdateLeaderboardsFromGameState()
        {
            if (GameManager.Instance == null) return;

            // Total money earned
            SubmitScore(LeaderboardType.TotalMoney, (long)GameManager.Instance.resources.money);

            // Prison population
            SubmitScore(LeaderboardType.PrisonPopulation, GameManager.Instance.prisoners.Count);

            // Total buildings
            SubmitScore(LeaderboardType.TotalBuildings, GameManager.Instance.buildings.Count);

            // Days survived
            SubmitScore(LeaderboardType.DaysSurvived, GameManager.Instance.currentDay);

            // Security rating
            SubmitScore(LeaderboardType.SecurityRating, (long)GameManager.Instance.resources.security);

            // Total prisoners received
            int totalReceived = GameManager.Instance.prisoners.Count +
                                PlayerPrefs.GetInt("TotalPrisonersEscaped", 0) +
                                PlayerPrefs.GetInt("TotalPrisonersReleased", 0);
            SubmitScore(LeaderboardType.TotalPrisonersManaged, totalReceived);
        }

        #endregion

        #region Save/Load

        private void LoadPlayerInfo()
        {
            playerName = PlayerPrefs.GetString("PlayerName", "Player");
            playerId = PlayerPrefs.GetString("PlayerId", SystemInfo.deviceUniqueIdentifier);

            // Save player ID if first time
            if (!PlayerPrefs.HasKey("PlayerId"))
            {
                PlayerPrefs.SetString("PlayerId", playerId);
                PlayerPrefs.Save();
            }
        }

        public void SetPlayerName(string name)
        {
            playerName = name;
            PlayerPrefs.SetString("PlayerName", name);
            PlayerPrefs.Save();
        }

        private void SaveLocalLeaderboards()
        {
            foreach (var kvp in localLeaderboards)
            {
                string json = JsonUtility.ToJson(new LeaderboardWrapper { entries = kvp.Value });
                PlayerPrefs.SetString($"Leaderboard_{kvp.Key}", json);
            }
            PlayerPrefs.Save();
        }

        private void LoadLocalLeaderboards()
        {
            foreach (LeaderboardType type in Enum.GetValues(typeof(LeaderboardType)))
            {
                string key = $"Leaderboard_{type}";
                if (PlayerPrefs.HasKey(key))
                {
                    string json = PlayerPrefs.GetString(key);
                    LeaderboardWrapper wrapper = JsonUtility.FromJson<LeaderboardWrapper>(json);
                    if (wrapper?.entries != null)
                    {
                        localLeaderboards[type] = wrapper.entries;
                    }
                }
            }
        }

        /// <summary>
        /// Reset all leaderboards (for testing)
        /// </summary>
        [ContextMenu("Reset All Leaderboards")]
        public void ResetAllLeaderboards()
        {
            foreach (LeaderboardType type in Enum.GetValues(typeof(LeaderboardType)))
            {
                PlayerPrefs.DeleteKey($"Leaderboard_{type}");
            }
            InitializeLeaderboards();
            Debug.Log("All leaderboards reset!");
        }

        #endregion

        #region Backend Integration (Placeholder)

        // TODO: Implement these methods for real backend integration

        /*
        private void SubmitToBackend(LeaderboardType type, LeaderboardEntry entry)
        {
            // Firebase example:
            // FirebaseDatabase.DefaultInstance
            //     .GetReference($"leaderboards/{type}/{playerId}")
            //     .SetRawJsonValueAsync(JsonUtility.ToJson(entry));

            // PlayFab example:
            // var request = new UpdatePlayerStatisticsRequest
            // {
            //     Statistics = new List<StatisticUpdate>
            //     {
            //         new StatisticUpdate { StatisticName = type.ToString(), Value = (int)entry.score }
            //     }
            // };
            // PlayFabClientAPI.UpdatePlayerStatistics(request, OnPlayFabSuccess, OnPlayFabError);
        }

        private void FetchFromBackend(LeaderboardType type, int maxEntries)
        {
            // Firebase example:
            // FirebaseDatabase.DefaultInstance
            //     .GetReference($"leaderboards/{type}")
            //     .OrderByChild("score")
            //     .LimitToLast(maxEntries)
            //     .GetValueAsync()
            //     .ContinueWith(task => { ... });

            // PlayFab example:
            // var request = new GetLeaderboardRequest
            // {
            //     StatisticName = type.ToString(),
            //     MaxResultsCount = maxEntries
            // };
            // PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardSuccess, OnPlayFabError);
        }
        */

        #endregion
    }

    #region Data Structures

    [Serializable]
    public class LeaderboardEntry
    {
        public string playerId;
        public string playerName;
        public long score;
        public DateTime timestamp;
        public int rank; // Set when fetching from backend
    }

    [Serializable]
    public class LeaderboardWrapper
    {
        public List<LeaderboardEntry> entries;
    }

    public enum LeaderboardType
    {
        TotalMoney,              // Most money accumulated
        PrisonPopulation,        // Largest prison population
        TotalBuildings,          // Most buildings constructed
        DaysSurvived,           // Longest game duration
        SecurityRating,         // Highest security rating
        TotalPrisonersManaged,  // Total prisoners received (including escaped/released)
        EscapesPreventedStreak, // Longest streak without escapes
        AchievementPoints       // Most achievement points
    }

    #endregion
}
