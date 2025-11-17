using UnityEngine;
using UnityEngine.Purchasing;
using System;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages all monetization: IAP, Ads, Premium Currency
    /// </summary>
    public class MonetizationManager : MonoBehaviour, IStoreListener
    {
        public static MonetizationManager Instance { get; private set; }

        [Header("Premium Currency")]
        public int crystals = 0;

        [Header("VIP Status")]
        public bool isVIP = false;
        public DateTime vipExpirationDate;

        [Header("IAP Products")]
        private const string CRYSTALS_100 = "crystals_100";
        private const string CRYSTALS_500 = "crystals_500";
        private const string CRYSTALS_1200 = "crystals_1200";
        private const string CRYSTALS_3000 = "crystals_3000";
        private const string STARTER_PACK = "starter_pack";
        private const string BUILDER_PACK = "builder_pack";
        private const string PREMIUM_PACK = "premium_pack";
        private const string VIP_MONTHLY = "vip_monthly";

        [Header("Ad Settings")]
        public int maxDailyAds = 5;
        private int adsWatchedToday = 0;
        private DateTime lastAdResetDate;

        private IStoreController storeController;
        private IExtensionProvider storeExtensionProvider;

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
            InitializePurchasing();
            CheckDailyAdReset();
        }

        #region Unity IAP Setup

        private void InitializePurchasing()
        {
            if (IsInitialized()) return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Crystal Packs
            builder.AddProduct(CRYSTALS_100, ProductType.Consumable, new IDs()
            {
                { CRYSTALS_100, AppleAppStore.Name },
                { CRYSTALS_100, GooglePlay.Name }
            });

            builder.AddProduct(CRYSTALS_500, ProductType.Consumable);
            builder.AddProduct(CRYSTALS_1200, ProductType.Consumable);
            builder.AddProduct(CRYSTALS_3000, ProductType.Consumable);

            // Resource Packs
            builder.AddProduct(STARTER_PACK, ProductType.Consumable);
            builder.AddProduct(BUILDER_PACK, ProductType.Consumable);
            builder.AddProduct(PREMIUM_PACK, ProductType.Consumable);

            // VIP Subscription
            builder.AddProduct(VIP_MONTHLY, ProductType.Subscription);

            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            return storeController != null && storeExtensionProvider != null;
        }

        #endregion

        #region IStoreListener Implementation

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            storeExtensionProvider = extensions;
            Debug.Log("Unity IAP Initialized successfully");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Unity IAP Initialization Failed: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Unity IAP Initialization Failed: {error} - {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string productId = args.purchasedProduct.definition.id;

            // Process Crystal Packs
            if (productId == CRYSTALS_100)
            {
                AddCrystals(100);
            }
            else if (productId == CRYSTALS_500)
            {
                AddCrystals(500);
            }
            else if (productId == CRYSTALS_1200)
            {
                AddCrystals(1200);
            }
            else if (productId == CRYSTALS_3000)
            {
                AddCrystals(3000);
            }
            // Process Resource Packs
            else if (productId == STARTER_PACK)
            {
                GameManager.Instance.resources.Add(5000, 200, 100);
                GameEvents.Instance?.OnInfoMessage?.Invoke("📦 Starter Pack reçu !");
            }
            else if (productId == BUILDER_PACK)
            {
                GameManager.Instance.resources.Add(15000, 0, 500);
                GameEvents.Instance?.OnInfoMessage?.Invoke("🏗️ Builder Pack reçu !");
            }
            else if (productId == PREMIUM_PACK)
            {
                GameManager.Instance.resources.Add(50000, 500, 1000);
                AddCrystals(200);
                GameEvents.Instance?.OnInfoMessage?.Invoke("👑 Premium Pack reçu !");
            }
            // Process VIP
            else if (productId == VIP_MONTHLY)
            {
                ActivateVIP(30); // 30 days
            }

            SaveMonetizationData();
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogWarning($"Purchase failed: {product.definition.id} - {failureReason}");
            GameEvents.Instance?.OnWarning?.Invoke("❌ Achat échoué");
        }

        #endregion

        #region Purchase Methods

        public void BuyCrystals100()
        {
            BuyProductID(CRYSTALS_100);
        }

        public void BuyCrystals500()
        {
            BuyProductID(CRYSTALS_500);
        }

        public void BuyCrystals1200()
        {
            BuyProductID(CRYSTALS_1200);
        }

        public void BuyCrystals3000()
        {
            BuyProductID(CRYSTALS_3000);
        }

        public void BuyStarterPack()
        {
            BuyProductID(STARTER_PACK);
        }

        public void BuyBuilderPack()
        {
            BuyProductID(BUILDER_PACK);
        }

        public void BuyPremiumPack()
        {
            BuyProductID(PREMIUM_PACK);
        }

        public void BuyVIPMonthly()
        {
            BuyProductID(VIP_MONTHLY);
        }

        private void BuyProductID(string productId)
        {
            if (IsInitialized())
            {
                Product product = storeController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log($"Purchasing product: {product.definition.id}");
                    storeController.InitiatePurchase(product);
                }
                else
                {
                    Debug.LogError($"Product not available: {productId}");
                }
            }
            else
            {
                Debug.LogError("Unity IAP not initialized!");
            }
        }

        #endregion

        #region Crystal Management

        public void AddCrystals(int amount)
        {
            crystals += amount;
            SaveMonetizationData();
            GameEvents.Instance?.OnInfoMessage?.Invoke($"💎 +{amount} Cristaux !");
            Debug.Log($"Added {amount} crystals. Total: {crystals}");
        }

        public bool SpendCrystals(int amount)
        {
            if (crystals >= amount)
            {
                crystals -= amount;
                SaveMonetizationData();
                return true;
            }
            GameEvents.Instance?.OnWarning?.Invoke("❌ Pas assez de cristaux !");
            return false;
        }

        public void AddFreeCrystals(int amount)
        {
            // Free crystals from gameplay (achievements, daily rewards)
            AddCrystals(amount);
        }

        #endregion

        #region VIP Management

        private void ActivateVIP(int days)
        {
            isVIP = true;
            vipExpirationDate = DateTime.Now.AddDays(days);
            SaveMonetizationData();
            GameEvents.Instance?.OnInfoMessage?.Invoke("👑 Statut VIP activé !");
            Debug.Log($"VIP activated until {vipExpirationDate}");
        }

        public bool IsVIPActive()
        {
            if (isVIP && DateTime.Now < vipExpirationDate)
            {
                return true;
            }
            else if (isVIP && DateTime.Now >= vipExpirationDate)
            {
                isVIP = false;
                SaveMonetizationData();
            }
            return false;
        }

        public float GetVIPIncomeMultiplier()
        {
            return IsVIPActive() ? 1.5f : 1.0f; // +50% revenus pour VIP
        }

        #endregion

        #region Rewarded Ads

        private void CheckDailyAdReset()
        {
            DateTime today = DateTime.Now.Date;
            if (lastAdResetDate.Date != today)
            {
                adsWatchedToday = 0;
                lastAdResetDate = today;
                SaveMonetizationData();
            }
        }

        public bool CanWatchAd()
        {
            CheckDailyAdReset();
            return adsWatchedToday < maxDailyAds;
        }

        public void ShowRewardedAd(AdRewardType rewardType)
        {
            if (!CanWatchAd())
            {
                GameEvents.Instance?.OnWarning?.Invoke("❌ Limite quotidienne de pubs atteinte");
                return;
            }

            // TODO: Integrate Unity Ads or AdMob
            // For now, simulate ad watched
            OnAdWatched(rewardType);
        }

        private void OnAdWatched(AdRewardType rewardType)
        {
            adsWatchedToday++;
            SaveMonetizationData();

            switch (rewardType)
            {
                case AdRewardType.Money:
                    GameManager.Instance.resources.money += 1000;
                    GameEvents.Instance?.OnInfoMessage?.Invoke("💰 +1000$ reçu !");
                    break;

                case AdRewardType.Food:
                    GameManager.Instance.resources.food += 50;
                    GameEvents.Instance?.OnInfoMessage?.Invoke("🍞 +50 nourriture !");
                    break;

                case AdRewardType.Materials:
                    GameManager.Instance.resources.materials += 25;
                    GameEvents.Instance?.OnInfoMessage?.Invoke("🧱 +25 matériaux !");
                    break;

                case AdRewardType.DoubleIncome:
                    StartCoroutine(DoubleIncomeBoost(3600)); // 1 hour
                    GameEvents.Instance?.OnInfoMessage?.Invoke("💰 Revenus x2 pendant 1h !");
                    break;

                case AdRewardType.FreeCrystals:
                    AddFreeCrystals(10);
                    break;
            }
        }

        private System.Collections.IEnumerator DoubleIncomeBoost(float duration)
        {
            // Implement double income logic
            yield return new WaitForSeconds(duration);
            GameEvents.Instance?.OnInfoMessage?.Invoke("⏰ Boost de revenus terminé");
        }

        #endregion

        #region Crystal Spending Options

        public void SpeedUpConstruction(int crystalCost = 50)
        {
            if (SpendCrystals(crystalCost))
            {
                // Complete current building instantly
                GameEvents.Instance?.OnInfoMessage?.Invoke("⚡ Construction accélérée !");
            }
        }

        public void BuyProductionBoost(int crystalCost = 100)
        {
            if (SpendCrystals(crystalCost))
            {
                StartCoroutine(ProductionBoost(86400)); // 24 hours
                GameEvents.Instance?.OnInfoMessage?.Invoke("🚀 Production x2 pendant 24h !");
            }
        }

        public void BuySecurityBoost(int crystalCost = 75)
        {
            if (SpendCrystals(crystalCost))
            {
                GameManager.Instance.resources.security += 50;
                GameEvents.Instance?.OnInfoMessage?.Invoke("🛡️ Sécurité +50% !");
            }
        }

        public void BuyResourcePack(int crystalCost = 200)
        {
            if (SpendCrystals(crystalCost))
            {
                GameManager.Instance.resources.Add(10000, 200, 150);
                GameEvents.Instance?.OnInfoMessage?.Invoke("📦 Pack de ressources acheté !");
            }
        }

        private System.Collections.IEnumerator ProductionBoost(float duration)
        {
            // Implement production boost logic
            yield return new WaitForSeconds(duration);
            GameEvents.Instance?.OnInfoMessage?.Invoke("⏰ Boost de production terminé");
        }

        #endregion

        #region Save/Load

        private void SaveMonetizationData()
        {
            PlayerPrefs.SetInt("Crystals", crystals);
            PlayerPrefs.SetInt("IsVIP", isVIP ? 1 : 0);
            PlayerPrefs.SetString("VIPExpiration", vipExpirationDate.ToString());
            PlayerPrefs.SetInt("AdsWatchedToday", adsWatchedToday);
            PlayerPrefs.SetString("LastAdResetDate", lastAdResetDate.ToString());
            PlayerPrefs.Save();
        }

        private void LoadMonetizationData()
        {
            crystals = PlayerPrefs.GetInt("Crystals", 0);
            isVIP = PlayerPrefs.GetInt("IsVIP", 0) == 1;

            string vipExpStr = PlayerPrefs.GetString("VIPExpiration", DateTime.Now.ToString());
            DateTime.TryParse(vipExpStr, out vipExpirationDate);

            adsWatchedToday = PlayerPrefs.GetInt("AdsWatchedToday", 0);

            string lastResetStr = PlayerPrefs.GetString("LastAdResetDate", DateTime.Now.ToString());
            DateTime.TryParse(lastResetStr, out lastAdResetDate);
        }

        #endregion

        public enum AdRewardType
        {
            Money,
            Food,
            Materials,
            DoubleIncome,
            FreeCrystals
        }
    }
}
