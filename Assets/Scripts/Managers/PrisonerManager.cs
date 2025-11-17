using UnityEngine;
using System.Collections.Generic;
using PrisonIsland.Data;
using PrisonIsland.Core;

namespace PrisonIsland.Managers
{
    public class PrisonerManager : MonoBehaviour
    {
        public static PrisonerManager Instance { get; private set; }

        [Header("Prefab")]
        public GameObject prisonerPrefab;

        [Header("Names")]
        private string[] firstNames = { "Marcus", "Victor", "Antoine", "Pierre", "Jean", "Claude", "Robert", "Michel", "André", "Philippe" };
        private string[] lastNames = { "Dupont", "Martin", "Bernard", "Dubois", "Thomas", "Robert", "Richard", "Petit", "Durand", "Leroy" };

        [Header("Spawn Settings")]
        public Transform spawnPoint;
        public float spawnInterval = 30f;
        private float spawnTimer = 0f;

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
            SpawnInitialPrisoners();
        }

        private void Update()
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval && GameManager.Instance.prisoners.Count < 50)
            {
                if (Random.value < 0.3f)
                {
                    SpawnPrisoner();
                }
                spawnTimer = 0f;
            }
        }

        private void SpawnInitialPrisoners()
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnPrisoner();
            }
        }

        public void SpawnPrisoner()
        {
            if (prisonerPrefab == null)
            {
                Debug.LogWarning("Prisoner prefab not assigned!");
                return;
            }

            Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            GameObject prisonerObj = Instantiate(prisonerPrefab, position, Quaternion.identity);
            prisonerObj.transform.SetParent(GameManager.Instance.prisonerParent);

            Prisoners.Prisoner prisoner = prisonerObj.GetComponent<Prisoners.Prisoner>();
            if (prisoner != null)
            {
                prisoner.Initialize(GenerateRandomName(), GenerateRandomDangerLevel());
            }
        }

        private string GenerateRandomName()
        {
            string first = firstNames[Random.Range(0, firstNames.Length)];
            string last = lastNames[Random.Range(0, lastNames.Length)];
            return $"{first} {last}";
        }

        private DangerLevel GenerateRandomDangerLevel()
        {
            int random = Random.Range(0, 100);
            if (random < 40) return DangerLevel.Low;
            if (random < 70) return DangerLevel.Medium;
            if (random < 90) return DangerLevel.High;
            return DangerLevel.Maximum;
        }

        public Buildings.Building FindAvailableCell()
        {
            foreach (var building in GameManager.Instance.buildings)
            {
                if (building.data.type == BuildingType.Cell)
                {
                    if (building.GetCurrentOccupancy() < building.data.capacity)
                    {
                        return building;
                    }
                }
            }
            return null;
        }
    }
}
