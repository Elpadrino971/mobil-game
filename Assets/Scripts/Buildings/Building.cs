using UnityEngine;
using PrisonIsland.Data;
using PrisonIsland.Core;
using System.Collections.Generic;

namespace PrisonIsland.Buildings
{
    public class Building : MonoBehaviour
    {
        public BuildingData data;
        public BuildingType type;
        public int level = 1;
        public List<Prisoners.Prisoner> assignedPrisoners = new List<Prisoners.Prisoner>();

        private void Start()
        {
            GameManager.Instance.RegisterBuilding(this);
        }

        public void Initialize(BuildingType buildingType)
        {
            type = buildingType;
            // In a real Unity project, you would load BuildingData from Resources or ScriptableObject
            // For now, we'll create data on the fly
            CreateDefaultData();
        }

        private void CreateDefaultData()
        {
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<BuildingData>();
                data.type = type;
                data.buildingName = BuildingData.GetBuildingName(type);
                BuildingCost cost = BuildingData.GetCost(type);
                data.cost = cost;

                // Set capacities and bonuses
                switch (type)
                {
                    case BuildingType.Cell:
                        data.capacity = 2;
                        break;
                    case BuildingType.Cafeteria:
                        data.capacity = 20;
                        break;
                    case BuildingType.Yard:
                        data.capacity = 30;
                        break;
                    case BuildingType.GuardTower:
                        data.securityBonus = 10f;
                        break;
                    case BuildingType.Workshop:
                        data.moneyProduction = 2f;
                        break;
                    case BuildingType.Kitchen:
                        data.foodProduction = 3f;
                        break;
                }
            }
        }

        public int GetCurrentOccupancy()
        {
            // Remove null references
            assignedPrisoners.RemoveAll(p => p == null);
            return assignedPrisoners.Count;
        }

        public bool HasCapacity()
        {
            return GetCurrentOccupancy() < data.capacity;
        }

        public void AssignPrisoner(Prisoners.Prisoner prisoner)
        {
            if (HasCapacity() && !assignedPrisoners.Contains(prisoner))
            {
                assignedPrisoners.Add(prisoner);
                prisoner.assignedBuilding = this;
            }
        }

        public void UnassignPrisoner(Prisoners.Prisoner prisoner)
        {
            assignedPrisoners.Remove(prisoner);
            if (prisoner.assignedBuilding == this)
            {
                prisoner.assignedBuilding = null;
            }
        }

        public void Demolish()
        {
            // Unassign all prisoners
            foreach (var prisoner in assignedPrisoners)
            {
                if (prisoner != null)
                {
                    prisoner.assignedBuilding = null;
                }
            }
            assignedPrisoners.Clear();

            GameManager.Instance.UnregisterBuilding(this);
            Destroy(gameObject);
        }

        private void OnMouseDown()
        {
            // Show building info UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowBuildingInfo(this);
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance?.UnregisterBuilding(this);
        }
    }
}
