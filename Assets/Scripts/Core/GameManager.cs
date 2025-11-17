using System.Collections.Generic;
using UnityEngine;
using PrisonIsland.Data;
using PrisonIsland.Buildings;
using PrisonIsland.Prisoners;

namespace PrisonIsland.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public GameResources resources = new GameResources();
        public int currentDay = 1;
        public int totalEscapes = 0;
        public int totalDeaths = 0;

        [Header("Game Settings")]
        public float dayDuration = 120f; // 2 minutes real time = 1 day
        private float dayTimer = 0f;

        [Header("Collections")]
        public List<Building> buildings = new List<Building>();
        public List<Prisoner> prisoners = new List<Prisoner>();

        [Header("References")]
        public Transform buildingParent;
        public Transform prisonerParent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            UpdateDayNightCycle();
            UpdatePrisoners();
        }

        private void InitializeGame()
        {
            // Initial buildings will be spawned via BuildingManager
            // Initial prisoners will be spawned via PrisonerManager
        }

        private void UpdateDayNightCycle()
        {
            dayTimer += Time.deltaTime;

            if (dayTimer >= dayDuration)
            {
                dayTimer = 0f;
                ProcessNewDay();
            }
        }

        private void ProcessNewDay()
        {
            currentDay++;

            // Consume resources
            resources.ConsumeDaily(prisoners.Count);

            // Add income
            resources.AddDailyIncome(prisoners.Count);

            // Add production from buildings
            foreach (var building in buildings)
            {
                if (building.data.moneyProduction > 0)
                    resources.money += building.data.moneyProduction;
                if (building.data.foodProduction > 0)
                    resources.food += building.data.foodProduction;
            }

            // Feed prisoners if enough food
            if (resources.food >= prisoners.Count * 3)
            {
                foreach (var prisoner in prisoners)
                {
                    prisoner.stats.Feed();
                }
            }

            resources.ClampValues();

            // Random new prisoner
            if (prisoners.Count < 50 && Random.value < 0.3f)
            {
                FindObjectOfType<PrisonerManager>()?.SpawnPrisoner();
            }

            UIManager.Instance?.UpdateUI();
        }

        private void UpdatePrisoners()
        {
            foreach (var prisoner in prisoners)
            {
                prisoner.stats.UpdateNeeds(Time.deltaTime);
            }
        }

        public bool CanAffordBuilding(BuildingType type)
        {
            BuildingCost cost = BuildingData.GetCost(type);
            return resources.CanAfford(cost.money, 0, cost.materials);
        }

        public void PurchaseBuilding(BuildingType type)
        {
            BuildingCost cost = BuildingData.GetCost(type);
            resources.Deduct(cost.money, 0, cost.materials);
        }

        public void RegisterBuilding(Building building)
        {
            if (!buildings.Contains(building))
            {
                buildings.Add(building);
            }
        }

        public void UnregisterBuilding(Building building)
        {
            buildings.Remove(building);
        }

        public void RegisterPrisoner(Prisoner prisoner)
        {
            if (!prisoners.Contains(prisoner))
            {
                prisoners.Add(prisoner);
            }
        }

        public void UnregisterPrisoner(Prisoner prisoner)
        {
            prisoners.Remove(prisoner);
        }

        public void OnEscapeSuccess()
        {
            totalEscapes++;
            resources.reputation = Mathf.Max(0, resources.reputation - 20);
        }

        public void OnPrisonerDeath()
        {
            totalDeaths++;
            resources.reputation = Mathf.Max(0, resources.reputation - 10);
        }
    }
}
