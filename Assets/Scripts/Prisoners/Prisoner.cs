using UnityEngine;
using UnityEngine.AI;
using PrisonIsland.Data;
using PrisonIsland.Core;
using PrisonIsland.Buildings;

namespace PrisonIsland.Prisoners
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Prisoner : MonoBehaviour
    {
        [Header("Info")]
        public string prisonerName;
        public DangerLevel dangerLevel;
        public PrisonerState currentState;
        public int escapeAttempts = 0;

        [Header("Stats")]
        public PrisonerStats stats = new PrisonerStats();

        [Header("References")]
        public Building assignedBuilding;
        private NavMeshAgent agent;

        [Header("AI Settings")]
        public float wanderRadius = 10f;
        public float stateChangeInterval = 5f;
        private float stateTimer = 0f;

        [Header("Visuals")]
        public Renderer bodyRenderer;
        public Material lowDangerMaterial;
        public Material mediumDangerMaterial;
        public Material highDangerMaterial;
        public Material maximumDangerMaterial;

        private Transform targetDestination;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            GameManager.Instance.RegisterPrisoner(this);
            ApplyDangerLevelColor();
        }

        public void Initialize(string name, DangerLevel danger)
        {
            prisonerName = name;
            dangerLevel = danger;
            currentState = PrisonerState.Idle;

            // Try to assign to a cell
            Building cell = Managers.PrisonerManager.Instance.FindAvailableCell();
            if (cell != null)
            {
                cell.AssignPrisoner(this);
            }
        }

        private void Update()
        {
            UpdateAI();
            CheckEscapeRisk();
        }

        private void UpdateAI()
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= stateChangeInterval)
            {
                stateTimer = 0f;
                ChangeState();
            }

            ExecuteCurrentState();
        }

        private void ChangeState()
        {
            // Random state change
            int random = Random.Range(0, 100);

            if (random < 20)
                currentState = PrisonerState.Idle;
            else if (random < 40)
                currentState = PrisonerState.Exercising;
            else if (random < 60)
                currentState = PrisonerState.Sleeping;
            else if (random < 80)
                currentState = PrisonerState.Working;
            else
                currentState = PrisonerState.Plotting;
        }

        private void ExecuteCurrentState()
        {
            switch (currentState)
            {
                case PrisonerState.Idle:
                    // Stand still or wander slowly
                    if (!agent.hasPath)
                    {
                        WanderAround();
                    }
                    break;

                case PrisonerState.Exercising:
                    // Move to yard if exists, otherwise wander
                    Building yard = FindNearestBuilding(BuildingType.Yard);
                    if (yard != null)
                    {
                        MoveToBuilding(yard);
                    }
                    else
                    {
                        WanderAround();
                    }
                    break;

                case PrisonerState.Sleeping:
                    // Move to assigned cell
                    if (assignedBuilding != null)
                    {
                        MoveToBuilding(assignedBuilding);
                    }
                    break;

                case PrisonerState.Working:
                    // Move to workshop
                    Building workshop = FindNearestBuilding(BuildingType.Workshop);
                    if (workshop != null)
                    {
                        MoveToBuilding(workshop);
                    }
                    break;

                case PrisonerState.Plotting:
                    // Move suspiciously, look for escape routes
                    WanderAround();
                    break;
            }
        }

        private void WanderAround()
        {
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += transform.position;
                randomDirection.y = 0;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }

        private Building FindNearestBuilding(BuildingType type)
        {
            Building nearest = null;
            float minDistance = float.MaxValue;

            foreach (var building in GameManager.Instance.buildings)
            {
                if (building.data.type == type)
                {
                    float distance = Vector3.Distance(transform.position, building.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = building;
                    }
                }
            }

            return nearest;
        }

        private void MoveToBuilding(Building building)
        {
            if (building != null)
            {
                agent.SetDestination(building.transform.position);
            }
        }

        private void CheckEscapeRisk()
        {
            float risk = stats.GetEscapeRisk(dangerLevel, escapeAttempts);

            if (risk > 80 && Random.value < 0.001f) // Very rare per frame
            {
                AttemptEscape();
            }
        }

        private void AttemptEscape()
        {
            float securityLevel = GameManager.Instance.resources.security;

            // Add security from guard towers
            foreach (var building in GameManager.Instance.buildings)
            {
                if (building.data.type == BuildingType.GuardTower)
                {
                    securityLevel += building.data.securityBonus;
                }
            }

            if (Random.value * 100 > securityLevel)
            {
                // Escape successful
                EscapeSuccessful();
            }
            else
            {
                // Escape failed
                EscapeFailed();
            }
        }

        private void EscapeSuccessful()
        {
            Debug.Log($"{prisonerName} escaped!");
            GameManager.Instance.OnEscapeSuccess();
            GameManager.Instance.UnregisterPrisoner(this);
            Destroy(gameObject);
        }

        private void EscapeFailed()
        {
            escapeAttempts++;
            stats.morale = Mathf.Max(0, stats.morale - 20);
            currentState = PrisonerState.Idle;
            Debug.Log($"{prisonerName} failed to escape!");
        }

        public void Feed()
        {
            if (GameManager.Instance.resources.food >= 3)
            {
                GameManager.Instance.resources.food -= 3;
                stats.Feed();
            }
        }

        public void Release()
        {
            GameManager.Instance.resources.reputation = Mathf.Min(100, GameManager.Instance.resources.reputation + 5);
            GameManager.Instance.UnregisterPrisoner(this);
            if (assignedBuilding != null)
            {
                assignedBuilding.UnassignPrisoner(this);
            }
            Destroy(gameObject);
        }

        private void ApplyDangerLevelColor()
        {
            if (bodyRenderer == null) return;

            Material mat = null;
            switch (dangerLevel)
            {
                case DangerLevel.Low:
                    mat = lowDangerMaterial;
                    bodyRenderer.material.color = Color.green;
                    break;
                case DangerLevel.Medium:
                    mat = mediumDangerMaterial;
                    bodyRenderer.material.color = Color.yellow;
                    break;
                case DangerLevel.High:
                    mat = highDangerMaterial;
                    bodyRenderer.material.color = new Color(1f, 0.5f, 0f); // Orange
                    break;
                case DangerLevel.Maximum:
                    mat = maximumDangerMaterial;
                    bodyRenderer.material.color = Color.red;
                    break;
            }

            if (mat != null)
            {
                bodyRenderer.material = mat;
            }
        }

        private void OnMouseDown()
        {
            // Show prisoner info UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPrisonerInfo(this);
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance?.UnregisterPrisoner(this);
        }
    }
}
