using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrisonIsland.Core;
using PrisonIsland.Buildings;
using PrisonIsland.Prisoners;
using PrisonIsland.Data;

namespace PrisonIsland
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Resource Display")]
        public TextMeshProUGUI moneyText;
        public TextMeshProUGUI foodText;
        public TextMeshProUGUI materialsText;
        public TextMeshProUGUI securityText;
        public TextMeshProUGUI reputationText;
        public TextMeshProUGUI dayText;

        [Header("Stats Display")]
        public TextMeshProUGUI prisonerCountText;
        public TextMeshProUGUI buildingCountText;
        public TextMeshProUGUI escapesText;

        [Header("Panels")]
        public GameObject buildMenuPanel;
        public GameObject prisonerInfoPanel;
        public GameObject buildingInfoPanel;
        public GameObject statsPanel;

        [Header("Prisoner Info")]
        public TextMeshProUGUI prisonerNameText;
        public TextMeshProUGUI prisonerDangerText;
        public Slider healthSlider;
        public Slider hungerSlider;
        public Slider moraleSlider;
        public Slider hygieneSlider;
        public TextMeshProUGUI escapeRiskText;
        public Button feedPrisonerButton;
        public Button releasePrisonerButton;

        [Header("Building Info")]
        public TextMeshProUGUI buildingNameText;
        public TextMeshProUGUI buildingDescriptionText;
        public TextMeshProUGUI occupancyText;
        public Button demolishButton;

        private Prisoner selectedPrisoner;
        private Building selectedBuilding;

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
            ClosePrisonerInfo();
            CloseBuildingInfo();
            CloseBuildMenu();
            CloseStatsPanel();

            // Setup button listeners
            if (feedPrisonerButton != null)
                feedPrisonerButton.onClick.AddListener(OnFeedPrisoner);
            if (releasePrisonerButton != null)
                releasePrisonerButton.onClick.AddListener(OnReleasePrisoner);
            if (demolishButton != null)
                demolishButton.onClick.AddListener(OnDemolishBuilding);
        }

        private void Update()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            UpdateResourceDisplay();
            UpdateStatsDisplay();

            if (selectedPrisoner != null)
            {
                UpdatePrisonerInfoDisplay();
            }
        }

        private void UpdateResourceDisplay()
        {
            if (GameManager.Instance == null) return;

            var resources = GameManager.Instance.resources;

            if (moneyText != null)
                moneyText.text = $"💰 {resources.money:F0}";
            if (foodText != null)
                foodText.text = $"🍞 {resources.food:F0}";
            if (materialsText != null)
                materialsText.text = $"🧱 {resources.materials:F0}";
            if (securityText != null)
                securityText.text = $"🛡️ {resources.security:F0}%";
            if (reputationText != null)
                reputationText.text = $"⭐ {resources.reputation:F0}%";
            if (dayText != null)
                dayText.text = $"📅 Jour {GameManager.Instance.currentDay}";
        }

        private void UpdateStatsDisplay()
        {
            if (GameManager.Instance == null) return;

            if (prisonerCountText != null)
                prisonerCountText.text = $"Détenus: {GameManager.Instance.prisoners.Count}";
            if (buildingCountText != null)
                buildingCountText.text = $"Bâtiments: {GameManager.Instance.buildings.Count}";
            if (escapesText != null)
                escapesText.text = $"Évasions: {GameManager.Instance.totalEscapes}";
        }

        // Build Menu
        public void ShowBuildMenu()
        {
            if (buildMenuPanel != null)
                buildMenuPanel.SetActive(true);
        }

        public void CloseBuildMenu()
        {
            if (buildMenuPanel != null)
                buildMenuPanel.SetActive(false);
        }

        public void OnBuildingTypeSelected(int typeIndex)
        {
            BuildingType type = (BuildingType)typeIndex;
            Managers.BuildingManager.Instance?.EnterBuildMode(type);
            CloseBuildMenu();
        }

        // Prisoner Info
        public void ShowPrisonerInfo(Prisoner prisoner)
        {
            selectedPrisoner = prisoner;
            if (prisonerInfoPanel != null)
                prisonerInfoPanel.SetActive(true);
            UpdatePrisonerInfoDisplay();
        }

        private void UpdatePrisonerInfoDisplay()
        {
            if (selectedPrisoner == null) return;

            if (prisonerNameText != null)
                prisonerNameText.text = selectedPrisoner.prisonerName;

            if (prisonerDangerText != null)
            {
                string dangerEmoji = "";
                switch (selectedPrisoner.dangerLevel)
                {
                    case DangerLevel.Low: dangerEmoji = "🟢"; break;
                    case DangerLevel.Medium: dangerEmoji = "🟡"; break;
                    case DangerLevel.High: dangerEmoji = "🟠"; break;
                    case DangerLevel.Maximum: dangerEmoji = "🔴"; break;
                }
                prisonerDangerText.text = $"{dangerEmoji} {selectedPrisoner.dangerLevel}";
            }

            if (healthSlider != null)
                healthSlider.value = selectedPrisoner.stats.health / 100f;
            if (hungerSlider != null)
                hungerSlider.value = (100f - selectedPrisoner.stats.hunger) / 100f;
            if (moraleSlider != null)
                moraleSlider.value = selectedPrisoner.stats.morale / 100f;
            if (hygieneSlider != null)
                hygieneSlider.value = selectedPrisoner.stats.hygiene / 100f;

            if (escapeRiskText != null)
            {
                float risk = selectedPrisoner.stats.GetEscapeRisk(selectedPrisoner.dangerLevel, selectedPrisoner.escapeAttempts);
                escapeRiskText.text = $"Risque d'évasion: {risk:F0}%";
            }
        }

        public void ClosePrisonerInfo()
        {
            selectedPrisoner = null;
            if (prisonerInfoPanel != null)
                prisonerInfoPanel.SetActive(false);
        }

        private void OnFeedPrisoner()
        {
            if (selectedPrisoner != null)
            {
                selectedPrisoner.Feed();
            }
        }

        private void OnReleasePrisoner()
        {
            if (selectedPrisoner != null)
            {
                selectedPrisoner.Release();
                ClosePrisonerInfo();
            }
        }

        // Building Info
        public void ShowBuildingInfo(Building building)
        {
            selectedBuilding = building;
            if (buildingInfoPanel != null)
                buildingInfoPanel.SetActive(true);

            if (buildingNameText != null)
                buildingNameText.text = building.data.buildingName;
            if (buildingDescriptionText != null)
                buildingDescriptionText.text = building.data.description;
            if (occupancyText != null)
                occupancyText.text = $"Occupation: {building.GetCurrentOccupancy()}/{building.data.capacity}";
        }

        public void CloseBuildingInfo()
        {
            selectedBuilding = null;
            if (buildingInfoPanel != null)
                buildingInfoPanel.SetActive(false);
        }

        private void OnDemolishBuilding()
        {
            if (selectedBuilding != null)
            {
                selectedBuilding.Demolish();
                CloseBuildingInfo();
            }
        }

        // Stats Panel
        public void ToggleStatsPanel()
        {
            if (statsPanel != null)
                statsPanel.SetActive(!statsPanel.activeSelf);
        }

        public void CloseStatsPanel()
        {
            if (statsPanel != null)
                statsPanel.SetActive(false);
        }
    }
}
