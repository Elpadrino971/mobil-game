using UnityEngine;
using PrisonIsland.Data;
using PrisonIsland.Core;

namespace PrisonIsland.Managers
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        [Header("Building Prefabs")]
        public GameObject cellPrefab;
        public GameObject cafeteriaPrefab;
        public GameObject yardPrefab;
        public GameObject infirmaryPrefab;
        public GameObject workshopPrefab;
        public GameObject guardTowerPrefab;
        public GameObject showerPrefab;
        public GameObject kitchenPrefab;
        public GameObject libraryPrefab;
        public GameObject visitingRoomPrefab;
        public GameObject solitaryPrefab;

        [Header("Build Settings")]
        public LayerMask groundLayer;
        public Material validPlacementMaterial;
        public Material invalidPlacementMaterial;

        private GameObject currentBuildingPreview;
        private BuildingType currentBuildingType;
        private bool isBuildMode = false;

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
            SpawnInitialBuildings();
        }

        private void SpawnInitialBuildings()
        {
            // Spawn 2 cells
            PlaceBuildingAt(BuildingType.Cell, new Vector3(5, 0, 5), true);
            PlaceBuildingAt(BuildingType.Cell, new Vector3(5, 0, 10), true);

            // Spawn 1 cafeteria
            PlaceBuildingAt(BuildingType.Cafeteria, new Vector3(10, 0, 5), true);
        }

        public void EnterBuildMode(BuildingType type)
        {
            if (!GameManager.Instance.CanAffordBuilding(type))
            {
                Debug.Log("Not enough resources!");
                return;
            }

            isBuildMode = true;
            currentBuildingType = type;
            CreateBuildingPreview(type);
        }

        public void ExitBuildMode()
        {
            isBuildMode = false;
            if (currentBuildingPreview != null)
            {
                Destroy(currentBuildingPreview);
            }
        }

        private void Update()
        {
            if (isBuildMode && currentBuildingPreview != null)
            {
                UpdateBuildingPreview();

                if (Input.GetMouseButtonDown(0))
                {
                    TryPlaceBuilding();
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ExitBuildMode();
                }
            }
        }

        private void CreateBuildingPreview(BuildingType type)
        {
            GameObject prefab = GetBuildingPrefab(type);
            if (prefab != null)
            {
                currentBuildingPreview = Instantiate(prefab);
                // Make semi-transparent
                SetPreviewTransparency(currentBuildingPreview, 0.5f);
            }
        }

        private void UpdateBuildingPreview()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
            {
                // Snap to grid
                Vector3 position = hit.point;
                position.x = Mathf.Round(position.x / 5f) * 5f;
                position.z = Mathf.Round(position.z / 5f) * 5f;
                position.y = 0;

                currentBuildingPreview.transform.position = position;

                // Check if valid placement
                bool isValid = IsValidPlacement(position);
                UpdatePreviewMaterial(isValid);
            }
        }

        private bool IsValidPlacement(Vector3 position)
        {
            // Check if there's already a building here
            Collider[] colliders = Physics.OverlapSphere(position, 2f);
            foreach (var col in colliders)
            {
                if (col.GetComponent<Buildings.Building>() != null)
                {
                    return false;
                }
            }
            return true;
        }

        private void TryPlaceBuilding()
        {
            Vector3 position = currentBuildingPreview.transform.position;

            if (IsValidPlacement(position))
            {
                PlaceBuildingAt(currentBuildingType, position, false);
                GameManager.Instance.PurchaseBuilding(currentBuildingType);
                ExitBuildMode();
            }
        }

        private void PlaceBuildingAt(BuildingType type, Vector3 position, bool isFree)
        {
            GameObject prefab = GetBuildingPrefab(type);
            if (prefab != null)
            {
                GameObject buildingObj = Instantiate(prefab, position, Quaternion.identity);
                buildingObj.transform.SetParent(GameManager.Instance.buildingParent);

                Buildings.Building building = buildingObj.GetComponent<Buildings.Building>();
                if (building != null)
                {
                    building.Initialize(type);
                }
            }
        }

        private GameObject GetBuildingPrefab(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Cell: return cellPrefab;
                case BuildingType.Cafeteria: return cafeteriaPrefab;
                case BuildingType.Yard: return yardPrefab;
                case BuildingType.Infirmary: return infirmaryPrefab;
                case BuildingType.Workshop: return workshopPrefab;
                case BuildingType.GuardTower: return guardTowerPrefab;
                case BuildingType.Shower: return showerPrefab;
                case BuildingType.Kitchen: return kitchenPrefab;
                case BuildingType.Library: return libraryPrefab;
                case BuildingType.VisitingRoom: return visitingRoomPrefab;
                case BuildingType.Solitary: return solitaryPrefab;
                default: return cellPrefab;
            }
        }

        private void SetPreviewTransparency(GameObject obj, float alpha)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;
                }
            }
        }

        private void UpdatePreviewMaterial(bool isValid)
        {
            Material mat = isValid ? validPlacementMaterial : invalidPlacementMaterial;
            Renderer[] renderers = currentBuildingPreview.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material = mat;
            }
        }
    }
}
