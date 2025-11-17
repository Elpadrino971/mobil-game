using UnityEngine;

namespace PrisonIsland.Data
{
    public enum BuildingType
    {
        Cell,
        Cafeteria,
        Yard,
        Infirmary,
        Workshop,
        GuardTower,
        Shower,
        Kitchen,
        Library,
        VisitingRoom,
        Solitary
    }

    [System.Serializable]
    public class BuildingCost
    {
        public float money;
        public float materials;

        public BuildingCost(float money, float materials)
        {
            this.money = money;
            this.materials = materials;
        }
    }

    [CreateAssetMenu(fileName = "BuildingData", menuName = "Prison Island/Building Data")]
    public class BuildingData : ScriptableObject
    {
        public BuildingType type;
        public string buildingName;
        [TextArea] public string description;
        public GameObject prefab;
        public BuildingCost cost;
        public int capacity;
        public float securityBonus;
        public float moneyProduction;
        public float foodProduction;

        public static BuildingCost GetCost(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Cell:
                    return new BuildingCost(100f, 50f);
                case BuildingType.Cafeteria:
                    return new BuildingCost(200f, 100f);
                case BuildingType.Yard:
                    return new BuildingCost(150f, 80f);
                case BuildingType.Infirmary:
                    return new BuildingCost(250f, 120f);
                case BuildingType.Workshop:
                    return new BuildingCost(180f, 90f);
                case BuildingType.GuardTower:
                    return new BuildingCost(300f, 150f);
                case BuildingType.Shower:
                    return new BuildingCost(120f, 60f);
                case BuildingType.Kitchen:
                    return new BuildingCost(220f, 110f);
                case BuildingType.Library:
                    return new BuildingCost(160f, 80f);
                case BuildingType.VisitingRoom:
                    return new BuildingCost(140f, 70f);
                case BuildingType.Solitary:
                    return new BuildingCost(180f, 100f);
                default:
                    return new BuildingCost(100f, 50f);
            }
        }

        public static string GetBuildingName(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Cell: return "Cellule";
                case BuildingType.Cafeteria: return "Cantine";
                case BuildingType.Yard: return "Cour";
                case BuildingType.Infirmary: return "Infirmerie";
                case BuildingType.Workshop: return "Atelier";
                case BuildingType.GuardTower: return "Tour de Garde";
                case BuildingType.Shower: return "Douches";
                case BuildingType.Kitchen: return "Cuisine";
                case BuildingType.Library: return "Bibliothèque";
                case BuildingType.VisitingRoom: return "Parloir";
                case BuildingType.Solitary: return "Cachot";
                default: return "Bâtiment";
            }
        }
    }
}
