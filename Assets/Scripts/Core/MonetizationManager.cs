using UnityEngine;
using System;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages all monetization: Premium Currency (IAP disabled for testing)
    /// SIMPLIFIED VERSION - Unity IAP removed for quick testing
    /// </summary>
    public class MonetizationManager : MonoBehaviour
    {
        public static MonetizationManager Instance { get; private set; }

        [Header("Premium Currency")]
        public int crystals = 100; // Start with some crystals for testing

        [Header("VIP Status")]
        public bool isVIP = false;
        public DateTime vipExpirationDate;

        [Header("Ad Settings")]
        public int maxDailyAds = 5;
        private int adsWatchedToday = 0;
        private DateTime lastAdResetDate;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadMonetizationData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Debug.Log("💰 MonetizationManager (Simplified) - IAP disabled for testing");
        }

        #region Premium Currency (Simplified)

        /// <summary>
        /// Add free crystals (for quest rewards, etc.)
        /// </summary>
        public void AddFreeCrystals(int amount)
        {
            crystals += amount;
            SaveMonetizationData();

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke($"💎 +{amount} Crystals! Total: {crystals}");
            }

            Debug.Log($"💎 Added {amount} crystals. Total: {crystals}");
        }

        /// <summary>
        /// Spend crystals
        /// </summary>
        public bool SpendCrystals(int amount)
        {
            if (crystals >= amount)
            {
                crystals -= amount;
                SaveMonetizationData();

                if (GameEvents.Instance != null)
                {
                    GameEvents.Instance.OnInfoMessage?.Invoke($"💎 -{amount} Crystals. Reste: {crystals}");
                }

                Debug.Log($"💎 Spent {amount} crystals. Remaining: {crystals}");
                return true;
            }

            Debug.LogWarning($"❌ Not enough crystals! Need {amount}, have {crystals}");

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnWarning?.Invoke("❌ Pas assez de cristaux!");
            }

            return false;
        }

        /// <summary>
        /// Get current crystal balance
        /// </summary>
        public int GetCrystals()
        {
            return crystals;
        }

        #endregion

        #region VIP System

        public void ActivateVIP(int days)
        {
            isVIP = true;
            vipExpirationDate = DateTime.Now.AddDays(days);
            SaveMonetizationData();

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnInfoMessage?.Invoke($"👑 VIP activé pour {days} jours!");
            }

            Debug.Log($"👑 VIP activated for {days} days");
        }

        public bool IsVIPActive()
        {
            if (!isVIP) return false;

            if (DateTime.Now > vipExpirationDate)
            {
                isVIP = false;
                SaveMonetizationData();
                return false;
            }

            return true;
        }

        #endregion

        #region Ads (Simplified - No real ads)

        public void ShowRewardedAd(string rewardType)
        {
            if (adsWatchedToday >= maxDailyAds)
            {
                Debug.LogWarning("⚠️ Daily ad limit reached!");

                if (GameEvents.Instance != null)
                {
                    GameEvents.Instance.OnWarning?.Invoke("Limite de publicités atteinte aujourd'hui!");
                }

                return;
            }

            // Simulate ad reward (no real ad for testing)
            adsWatchedToday++;
            SaveMonetizationData();

            // Give reward based on type
            switch (rewardType.ToLower())
            {
                case "crystals":
                    AddFreeCrystals(10);
                    break;
                case "money":
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.resources.money += 1000;
                    }
                    break;
                case "speed":
                    // Speed boost would go here
                    break;
            }

            Debug.Log($"📺 Simulated ad watched. Reward: {rewardType}");
        }

        public bool CanWatchAd()
        {
            CheckAdReset();
            return adsWatchedToday < maxDailyAds;
        }

        private void CheckAdReset()
        {
            if ((DateTime.Now - lastAdResetDate).TotalDays >= 1)
            {
                adsWatchedToday = 0;
                lastAdResetDate = DateTime.Now;
                SaveMonetizationData();
            }
        }

        #endregion

        #region Save/Load

        private void LoadMonetizationData()
        {
            crystals = PlayerPrefs.GetInt("Crystals", 100); // Start with 100 for testing
            isVIP = PlayerPrefs.GetInt("IsVIP", 0) == 1;

            string vipDateStr = PlayerPrefs.GetString("VIPExpirationDate", "");
            if (!string.IsNullOrEmpty(vipDateStr))
            {
                DateTime.TryParse(vipDateStr, out vipExpirationDate);
            }

            adsWatchedToday = PlayerPrefs.GetInt("AdsWatchedToday", 0);

            string adDateStr = PlayerPrefs.GetString("LastAdResetDate", DateTime.Now.ToString());
            DateTime.TryParse(adDateStr, out lastAdResetDate);

            Debug.Log($"💰 Loaded monetization data: {crystals} crystals, VIP: {isVIP}");
        }

        private void SaveMonetizationData()
        {
            PlayerPrefs.SetInt("Crystals", crystals);
            PlayerPrefs.SetInt("IsVIP", isVIP ? 1 : 0);
            PlayerPrefs.SetString("VIPExpirationDate", vipExpirationDate.ToString());
            PlayerPrefs.SetInt("AdsWatchedToday", adsWatchedToday);
            PlayerPrefs.SetString("LastAdResetDate", lastAdResetDate.ToString());
            PlayerPrefs.Save();
        }

        #endregion

        #region Testing Methods

        [ContextMenu("Add 100 Test Crystals")]
        public void AddTestCrystals()
        {
            AddFreeCrystals(100);
        }

        [ContextMenu("Activate VIP (30 days)")]
        public void ActivateTestVIP()
        {
            ActivateVIP(30);
        }

        #endregion
    }
}
