using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PrisonIsland.UI
{
    /// <summary>
    /// Manages the in-game shop UI
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        [Header("Shop Panels")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private GameObject crystalShopPanel;
        [SerializeField] private GameObject resourceShopPanel;
        [SerializeField] private GameObject vipPanel;

        [Header("Crystal Display")]
        [SerializeField] private TextMeshProUGUI crystalCountText;

        [Header("Tabs")]
        [SerializeField] private Button crystalsTab;
        [SerializeField] private Button resourcesTab;
        [SerializeField] private Button vipTab;

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
            UpdateCrystalDisplay();
            CloseShop();
        }

        private void SetupTabs()
        {
            if (crystalsTab != null)
                crystalsTab.onClick.AddListener(() => ShowTab(ShopTab.Crystals));
            if (resourcesTab != null)
                resourcesTab.onClick.AddListener(() => ShowTab(ShopTab.Resources));
            if (vipTab != null)
                vipTab.onClick.AddListener(() => ShowTab(ShopTab.VIP));
        }

        public void OpenShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                ShowTab(ShopTab.Crystals);
                UpdateCrystalDisplay();
            }
        }

        public void CloseShop()
        {
            if (shopPanel != null)
                shopPanel.SetActive(false);
        }

        private void ShowTab(ShopTab tab)
        {
            // Hide all panels
            if (crystalShopPanel != null) crystalShopPanel.SetActive(false);
            if (resourceShopPanel != null) resourceShopPanel.SetActive(false);
            if (vipPanel != null) vipPanel.SetActive(false);

            // Show selected panel
            switch (tab)
            {
                case ShopTab.Crystals:
                    if (crystalShopPanel != null) crystalShopPanel.SetActive(true);
                    break;
                case ShopTab.Resources:
                    if (resourceShopPanel != null) resourceShopPanel.SetActive(true);
                    break;
                case ShopTab.VIP:
                    if (vipPanel != null) vipPanel.SetActive(true);
                    break;
            }
        }

        private void UpdateCrystalDisplay()
        {
            if (crystalCountText != null && Core.MonetizationManager.Instance != null)
            {
                crystalCountText.text = $"💎 {Core.MonetizationManager.Instance.crystals}";
            }
        }

        #region Crystal Purchase Buttons

        public void OnBuyCrystals100()
        {
            Core.MonetizationManager.Instance?.BuyCrystals100();
            UpdateCrystalDisplay();
        }

        public void OnBuyCrystals500()
        {
            Core.MonetizationManager.Instance?.BuyCrystals500();
            UpdateCrystalDisplay();
        }

        public void OnBuyCrystals1200()
        {
            Core.MonetizationManager.Instance?.BuyCrystals1200();
            UpdateCrystalDisplay();
        }

        public void OnBuyCrystals3000()
        {
            Core.MonetizationManager.Instance?.BuyCrystals3000();
            UpdateCrystalDisplay();
        }

        #endregion

        #region Resource Pack Buttons

        public void OnBuyStarterPack()
        {
            Core.MonetizationManager.Instance?.BuyStarterPack();
        }

        public void OnBuyBuilderPack()
        {
            Core.MonetizationManager.Instance?.BuyBuilderPack();
        }

        public void OnBuyPremiumPack()
        {
            Core.MonetizationManager.Instance?.BuyPremiumPack();
        }

        #endregion

        #region VIP

        public void OnBuyVIP()
        {
            Core.MonetizationManager.Instance?.BuyVIPMonthly();
        }

        #endregion

        #region Crystal Spending

        public void OnSpeedUpConstruction()
        {
            Core.MonetizationManager.Instance?.SpeedUpConstruction(50);
            UpdateCrystalDisplay();
        }

        public void OnBuyProductionBoost()
        {
            Core.MonetizationManager.Instance?.BuyProductionBoost(100);
            UpdateCrystalDisplay();
        }

        public void OnBuySecurityBoost()
        {
            Core.MonetizationManager.Instance?.BuySecurityBoost(75);
            UpdateCrystalDisplay();
        }

        public void OnBuyResourcePackCrystals()
        {
            Core.MonetizationManager.Instance?.BuyResourcePack(200);
            UpdateCrystalDisplay();
        }

        #endregion

        #region Rewarded Ads

        public void ShowAdForMoney()
        {
            if (Core.MonetizationManager.Instance != null &&
                Core.MonetizationManager.Instance.CanWatchAd())
            {
                Core.MonetizationManager.Instance.ShowRewardedAd(
                    Core.MonetizationManager.AdRewardType.Money);
            }
        }

        public void ShowAdForFood()
        {
            if (Core.MonetizationManager.Instance != null &&
                Core.MonetizationManager.Instance.CanWatchAd())
            {
                Core.MonetizationManager.Instance.ShowRewardedAd(
                    Core.MonetizationManager.AdRewardType.Food);
            }
        }

        public void ShowAdForMaterials()
        {
            if (Core.MonetizationManager.Instance != null &&
                Core.MonetizationManager.Instance.CanWatchAd())
            {
                Core.MonetizationManager.Instance.ShowRewardedAd(
                    Core.MonetizationManager.AdRewardType.Materials);
            }
        }

        public void ShowAdForDoubleIncome()
        {
            if (Core.MonetizationManager.Instance != null &&
                Core.MonetizationManager.Instance.CanWatchAd())
            {
                Core.MonetizationManager.Instance.ShowRewardedAd(
                    Core.MonetizationManager.AdRewardType.DoubleIncome);
            }
        }

        public void ShowAdForCrystals()
        {
            if (Core.MonetizationManager.Instance != null &&
                Core.MonetizationManager.Instance.CanWatchAd())
            {
                Core.MonetizationManager.Instance.ShowRewardedAd(
                    Core.MonetizationManager.AdRewardType.FreeCrystals);
                UpdateCrystalDisplay();
            }
        }

        #endregion

        private enum ShopTab
        {
            Crystals,
            Resources,
            VIP
        }
    }
}
