using UnityEngine;
using System.Collections.Generic;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages game analytics tracking
    /// Integrate with Firebase Analytics, Unity Analytics, or custom backend
    /// </summary>
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance { get; private set; }

        [Header("Analytics Settings")]
        [SerializeField] private bool enableAnalytics = true;
        [SerializeField] private bool debugMode = true;

        [Header("Session Tracking")]
        private float sessionStartTime;
        private int sessionCount = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAnalytics();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartSession();
            SubscribeToGameEvents();
        }

        private void OnApplicationQuit()
        {
            EndSession();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                EndSession();
            }
            else
            {
                StartSession();
            }
        }

        private void InitializeAnalytics()
        {
            if (!enableAnalytics) return;

            // TODO: Initialize Firebase Analytics
            /*
            Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            Firebase.Analytics.FirebaseAnalytics.SetSessionTimeoutDuration(new System.TimeSpan(0, 30, 0));
            */

            // TODO: Initialize Unity Analytics
            /*
            UnityEngine.Analytics.Analytics.enabled = true;
            */

            sessionCount = PlayerPrefs.GetInt("TotalSessions", 0);

            if (debugMode)
            {
                Debug.Log("[Analytics] Analytics Manager Initialized");
            }
        }

        #region Session Management

        private void StartSession()
        {
            sessionStartTime = Time.time;
            sessionCount++;
            PlayerPrefs.SetInt("TotalSessions", sessionCount);

            TrackEvent("session_start", new Dictionary<string, object>
            {
                { "session_count", sessionCount },
                { "timestamp", System.DateTime.Now.ToString() }
            });

            if (debugMode)
            {
                Debug.Log($"[Analytics] Session {sessionCount} started");
            }
        }

        private void EndSession()
        {
            float sessionDuration = Time.time - sessionStartTime;

            TrackEvent("session_end", new Dictionary<string, object>
            {
                { "session_duration", sessionDuration },
                { "session_count", sessionCount }
            });

            if (debugMode)
            {
                Debug.Log($"[Analytics] Session ended. Duration: {sessionDuration:F0}s");
            }
        }

        #endregion

        #region Event Tracking

        /// <summary>
        /// Track a custom event with parameters
        /// </summary>
        public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            if (!enableAnalytics) return;

            // Firebase Analytics
            /*
            if (parameters != null)
            {
                Firebase.Analytics.Parameter[] firebaseParams = new Firebase.Analytics.Parameter[parameters.Count];
                int i = 0;
                foreach (var param in parameters)
                {
                    firebaseParams[i] = new Firebase.Analytics.Parameter(param.Key, param.Value.ToString());
                    i++;
                }
                Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, firebaseParams);
            }
            else
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
            }
            */

            // Unity Analytics
            /*
            if (parameters != null)
            {
                UnityEngine.Analytics.Analytics.CustomEvent(eventName, parameters);
            }
            else
            {
                UnityEngine.Analytics.Analytics.CustomEvent(eventName);
            }
            */

            // Debug logging
            if (debugMode)
            {
                string paramsStr = "";
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        paramsStr += $"{param.Key}={param.Value}, ";
                    }
                }
                Debug.Log($"[Analytics] Event: {eventName} | {paramsStr}");
            }
        }

        /// <summary>
        /// Track screen/view changes
        /// </summary>
        public void TrackScreen(string screenName)
        {
            if (!enableAnalytics) return;

            // Firebase
            // Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventScreenView,
            //     new Firebase.Analytics.Parameter("screen_name", screenName));

            TrackEvent("screen_view", new Dictionary<string, object>
            {
                { "screen_name", screenName }
            });
        }

        /// <summary>
        /// Track user property
        /// </summary>
        public void SetUserProperty(string propertyName, string value)
        {
            if (!enableAnalytics) return;

            // Firebase.Analytics.FirebaseAnalytics.SetUserProperty(propertyName, value);

            if (debugMode)
            {
                Debug.Log($"[Analytics] User Property: {propertyName} = {value}");
            }
        }

        #endregion

        #region Game-Specific Events

        private void SubscribeToGameEvents()
        {
            if (GameEvents.Instance == null) return;

            // Building events
            GameEvents.Instance.OnBuildingConstructed += (building) =>
            {
                TrackEvent("building_constructed", new Dictionary<string, object>
                {
                    { "building_type", building.type.ToString() },
                    { "building_level", building.level },
                    { "total_buildings", GameManager.Instance.buildings.Count }
                });
            };

            GameEvents.Instance.OnBuildingDemolished += (building) =>
            {
                TrackEvent("building_demolished", new Dictionary<string, object>
                {
                    { "building_type", building.type.ToString() }
                });
            };

            // Prisoner events
            GameEvents.Instance.OnPrisonerArrived += (prisoner) =>
            {
                TrackEvent("prisoner_arrived", new Dictionary<string, object>
                {
                    { "danger_level", prisoner.dangerLevel.ToString() },
                    { "total_prisoners", GameManager.Instance.prisoners.Count }
                });
            };

            GameEvents.Instance.OnPrisonerEscaped += (prisoner) =>
            {
                TrackEvent("prisoner_escaped", new Dictionary<string, object>
                {
                    { "danger_level", prisoner.dangerLevel.ToString() },
                    { "total_escapes", GameManager.Instance.totalEscapes }
                });
            };

            // Resource events
            GameEvents.Instance.OnResourcesInsufficient += () =>
            {
                TrackEvent("resources_insufficient", new Dictionary<string, object>
                {
                    { "money", GameManager.Instance.resources.money },
                    { "food", GameManager.Instance.resources.food },
                    { "materials", GameManager.Instance.resources.materials }
                });
            };

            // Game state events
            GameEvents.Instance.OnNewDay += (day) =>
            {
                // Track every 7 days to avoid too many events
                if (day % 7 == 0)
                {
                    TrackEvent("game_milestone", new Dictionary<string, object>
                    {
                        { "day", day },
                        { "prisoners", GameManager.Instance.prisoners.Count },
                        { "buildings", GameManager.Instance.buildings.Count },
                        { "money", GameManager.Instance.resources.money },
                        { "reputation", GameManager.Instance.resources.reputation }
                    });
                }
            };
        }

        #endregion

        #region Monetization Tracking

        public void TrackPurchaseAttempt(string productId, float price)
        {
            TrackEvent("purchase_attempt", new Dictionary<string, object>
            {
                { "product_id", productId },
                { "price", price }
            });
        }

        public void TrackPurchaseSuccess(string productId, float price, string currency = "EUR")
        {
            // Firebase E-commerce event
            /*
            Firebase.Analytics.FirebaseAnalytics.LogEvent(
                Firebase.Analytics.FirebaseAnalytics.EventPurchase,
                new Firebase.Analytics.Parameter[]
                {
                    new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterItemId, productId),
                    new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterValue, price),
                    new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCurrency, currency)
                }
            );
            */

            TrackEvent("purchase_success", new Dictionary<string, object>
            {
                { "product_id", productId },
                { "price", price },
                { "currency", currency }
            });
        }

        public void TrackPurchaseFailed(string productId, string reason)
        {
            TrackEvent("purchase_failed", new Dictionary<string, object>
            {
                { "product_id", productId },
                { "reason", reason }
            });
        }

        public void TrackAdImpression(string adType, string placement)
        {
            TrackEvent("ad_impression", new Dictionary<string, object>
            {
                { "ad_type", adType },
                { "placement", placement }
            });
        }

        public void TrackAdReward(string rewardType, float rewardAmount)
        {
            TrackEvent("ad_reward_claimed", new Dictionary<string, object>
            {
                { "reward_type", rewardType },
                { "reward_amount", rewardAmount }
            });
        }

        #endregion

        #region Progression Tracking

        public void TrackLevelUp(int newLevel)
        {
            TrackEvent("level_up", new Dictionary<string, object>
            {
                { "new_level", newLevel }
            });
        }

        public void TrackAchievementUnlocked(string achievementId, int points)
        {
            TrackEvent("achievement_unlocked", new Dictionary<string, object>
            {
                { "achievement_id", achievementId },
                { "points", points }
            });
        }

        public void TrackTutorialComplete()
        {
            TrackEvent("tutorial_complete");
        }

        public void TrackTutorialSkipped()
        {
            TrackEvent("tutorial_skipped");
        }

        #endregion

        #region Retention Metrics

        public void TrackDailyLogin()
        {
            string lastLogin = PlayerPrefs.GetString("LastLoginDate", "");
            string today = System.DateTime.Now.Date.ToString();

            if (lastLogin != today)
            {
                int consecutiveDays = PlayerPrefs.GetInt("ConsecutiveLoginDays", 0) + 1;
                PlayerPrefs.SetInt("ConsecutiveLoginDays", consecutiveDays);
                PlayerPrefs.SetString("LastLoginDate", today);

                TrackEvent("daily_login", new Dictionary<string, object>
                {
                    { "consecutive_days", consecutiveDays }
                });
            }
        }

        #endregion

        #region User Segmentation

        public void IdentifyUser(string userId)
        {
            if (!enableAnalytics) return;

            // Firebase.Analytics.FirebaseAnalytics.SetUserId(userId);

            SetUserProperty("user_id", userId);
        }

        public void SetPlayerType(string playerType)
        {
            // e.g., "free", "paying", "whale"
            SetUserProperty("player_type", playerType);
        }

        public void SetPlayerLevel(int level)
        {
            SetUserProperty("player_level", level.ToString());
        }

        #endregion

        #region Debug Tools

        [ContextMenu("Test Event")]
        public void TestEvent()
        {
            TrackEvent("test_event", new Dictionary<string, object>
            {
                { "test_param", "test_value" },
                { "timestamp", System.DateTime.Now.ToString() }
            });
        }

        [ContextMenu("Print Session Info")]
        public void PrintSessionInfo()
        {
            Debug.Log($"Total Sessions: {sessionCount}");
            Debug.Log($"Current Session Duration: {Time.time - sessionStartTime:F0}s");
        }

        #endregion
    }
}
