using UnityEngine;
using System.Collections.Generic;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages random events that add challenge and variety to gameplay
    /// </summary>
    public class RandomEventManager : MonoBehaviour
    {
        public static RandomEventManager Instance { get; private set; }

        [Header("Event Settings")]
        [SerializeField] private float eventCheckInterval = 60f; // Check every minute
        [SerializeField] private float eventChance = 0.2f; // 20% chance per check

        private float eventTimer = 0f;
        private List<RandomEvent> possibleEvents = new List<RandomEvent>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeEvents();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            eventTimer += Time.deltaTime;

            if (eventTimer >= eventCheckInterval)
            {
                eventTimer = 0f;
                TryTriggerEvent();
            }
        }

        private void InitializeEvents()
        {
            // Positive Events
            possibleEvents.Add(new RandomEvent
            {
                name = "Donation Généreuse",
                description = "Un donateur anonyme vous envoie 5000$ !",
                type = EventType.Positive,
                effect = () => {
                    GameManager.Instance.resources.money += 5000;
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnInfoMessage?.Invoke("💰 Donation de 5000$ reçue !");
                    }
                }
            });

            possibleEvents.Add(new RandomEvent
            {
                name = "Livraison de Nourriture",
                description = "Une livraison surprise de nourriture arrive !",
                type = EventType.Positive,
                effect = () => {
                    GameManager.Instance.resources.food += 200;
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnInfoMessage?.Invoke("🍞 +200 nourriture reçue !");
                    }
                }
            });

            possibleEvents.Add(new RandomEvent
            {
                name = "Inspection Réussie",
                description = "L'inspection gouvernementale améliore votre réputation !",
                type = EventType.Positive,
                effect = () => {
                    GameManager.Instance.resources.reputation += 15;
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnInfoMessage?.Invoke("⭐ Inspection réussie ! +15 réputation");
                    }
                }
            });

            // Negative Events
            possibleEvents.Add(new RandomEvent
            {
                name = "Émeute",
                description = "Une émeute éclate ! Tous les prisonniers perdent du moral.",
                type = EventType.Negative,
                effect = () => {
                    foreach (var prisoner in GameManager.Instance.prisoners)
                    {
                        prisoner.stats.morale = Mathf.Max(0, prisoner.stats.morale - 30);
                    }
                    GameManager.Instance.resources.reputation -= 10;
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnCriticalAlert?.Invoke("🚨 ÉMEUTE ! Moral des détenus -30");
                    }
                }
            });

            possibleEvents.Add(new RandomEvent
            {
                name = "Panne Électrique",
                description = "Une panne réduit temporairement la sécurité.",
                type = EventType.Negative,
                effect = () => {
                    GameManager.Instance.resources.security = Mathf.Max(0, GameManager.Instance.resources.security - 20);
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnWarning?.Invoke("⚠️ Panne électrique ! Sécurité -20");
                    }
                }
            });

            possibleEvents.Add(new RandomEvent
            {
                name = "Contamination Alimentaire",
                description = "De la nourriture contaminée doit être jetée.",
                type = EventType.Negative,
                effect = () => {
                    GameManager.Instance.resources.food = Mathf.Max(0, GameManager.Instance.resources.food - 100);
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnWarning?.Invoke("⚠️ Nourriture contaminée ! -100 nourriture");
                    }
                }
            });

            // Neutral Events
            possibleEvents.Add(new RandomEvent
            {
                name = "Visite de la Presse",
                description = "La presse visite votre prison. Impact selon votre réputation.",
                type = EventType.Neutral,
                effect = () => {
                    float rep = GameManager.Instance.resources.reputation;
                    if (rep > 70)
                    {
                        GameManager.Instance.resources.reputation += 10;
                        if (GameEvents.Instance != null)
                        {
                            GameEvents.Instance.OnInfoMessage?.Invoke("📰 Article positif ! +10 réputation");
                        }
                    }
                    else if (rep < 40)
                    {
                        GameManager.Instance.resources.reputation -= 10;
                        if (GameEvents.Instance != null)
                        {
                            GameEvents.Instance.OnWarning?.Invoke("📰 Article négatif ! -10 réputation");
                        }
                    }
                }
            });

            possibleEvents.Add(new RandomEvent
            {
                name = "Nouvelle Réglementation",
                description = "De nouvelles lois affectent vos coûts.",
                type = EventType.Neutral,
                effect = () => {
                    GameManager.Instance.resources.money -= 2000;
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnWarning?.Invoke("📋 Nouvelles réglementations : -2000$");
                    }
                }
            });

            // Rare Events
            possibleEvents.Add(new RandomEvent
            {
                name = "Subvention Gouvernementale",
                description = "Le gouvernement vous accorde une grosse subvention !",
                type = EventType.Rare,
                rarity = 0.05f, // 5% chance
                effect = () => {
                    GameManager.Instance.resources.money += 20000;
                    GameManager.Instance.resources.materials += 500;
                    if (GameEvents.Instance != null)
                    {
                        GameEvents.Instance.OnInfoMessage?.Invoke("🎉 Subvention ! +20000$ et +500 matériaux");
                    }
                }
            });

            possibleEvents.Add(new RandomEvent
            {
                name = "Tentative d'Évasion en Masse",
                description = "Plusieurs prisonniers tentent de s'échapper !",
                type = EventType.Rare,
                rarity = 0.03f, // 3% chance
                effect = () => {
                    int escapees = 0;
                    float security = GameManager.Instance.resources.security;

                    for (int i = GameManager.Instance.prisoners.Count - 1; i >= 0; i--)
                    {
                        var prisoner = GameManager.Instance.prisoners[i];
                        if (prisoner.dangerLevel == Data.DangerLevel.High ||
                            prisoner.dangerLevel == Data.DangerLevel.Maximum)
                        {
                            if (Random.value * 100 > security)
                            {
                                escapees++;
                                GameManager.Instance.prisoners.RemoveAt(i);
                                Destroy(prisoner.gameObject);
                            }
                        }
                    }

                    if (escapees > 0)
                    {
                        GameManager.Instance.totalEscapes += escapees;
                        GameManager.Instance.resources.reputation -= escapees * 10;
                        if (GameEvents.Instance != null)
                        {
                            GameEvents.Instance.OnCriticalAlert?.Invoke($"🚨 ALERTE ! {escapees} détenus se sont échappés !");
                        }
                    }
                    else
                    {
                        if (GameEvents.Instance != null)
                        {
                            GameEvents.Instance.OnInfoMessage?.Invoke("🛡️ Tentative d'évasion massive déjouée !");
                        }
                    }
                }
            });
        }

        private void TryTriggerEvent()
        {
            if (Random.value > eventChance)
                return;

            // Filter events by rarity
            List<RandomEvent> availableEvents = new List<RandomEvent>();

            foreach (var evt in possibleEvents)
            {
                float chance = evt.type == EventType.Rare ? evt.rarity : 1f;
                if (Random.value <= chance)
                {
                    availableEvents.Add(evt);
                }
            }

            if (availableEvents.Count > 0)
            {
                RandomEvent selectedEvent = availableEvents[Random.Range(0, availableEvents.Count)];
                TriggerEvent(selectedEvent);
            }
        }

        private void TriggerEvent(RandomEvent evt)
        {
            Debug.Log($"[Random Event] {evt.name}: {evt.description}");
            evt.effect?.Invoke();
        }

        public void ForceEvent(string eventName)
        {
            RandomEvent evt = possibleEvents.Find(e => e.name == eventName);
            if (evt != null)
            {
                TriggerEvent(evt);
            }
        }

        [System.Serializable]
        private class RandomEvent
        {
            public string name;
            public string description;
            public EventType type;
            public float rarity = 1f;
            public System.Action effect;
        }

        private enum EventType
        {
            Positive,
            Negative,
            Neutral,
            Rare
        }
    }
}
