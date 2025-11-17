using System;
using UnityEngine;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Central event system for the game
    /// Subscribe to events to react to game state changes
    /// </summary>
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents Instance { get; private set; }

        // Building Events
        public event Action<Buildings.Building> OnBuildingConstructed;
        public event Action<Buildings.Building> OnBuildingDemolished;
        public event Action<Buildings.Building> OnBuildingSelected;

        // Prisoner Events
        public event Action<Prisoners.Prisoner> OnPrisonerArrived;
        public event Action<Prisoners.Prisoner> OnPrisonerReleased;
        public event Action<Prisoners.Prisoner> OnPrisonerEscaped;
        public event Action<Prisoners.Prisoner> OnPrisonerDied;
        public event Action<Prisoners.Prisoner> OnPrisonerSelected;
        public event Action<Prisoners.Prisoner> OnEscapeAttemptFailed;

        // Resource Events
        public event Action<float> OnMoneyChanged;
        public event Action<float> OnFoodChanged;
        public event Action<float> OnMaterialsChanged;
        public event Action OnResourcesInsufficient;

        // Game State Events
        public event Action<int> OnNewDay;
        public event Action<int> OnSecurityLevelChanged;
        public event Action<int> OnReputationChanged;
        public event Action OnGamePaused;
        public event Action OnGameResumed;
        public event Action OnGameSaved;
        public event Action OnGameLoaded;

        // Alert Events
        public event Action<string> OnWarning;
        public event Action<string> OnCriticalAlert;
        public event Action<string> OnInfoMessage;

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

        #region Building Event Triggers
        public void TriggerBuildingConstructed(Buildings.Building building)
        {
            OnBuildingConstructed?.Invoke(building);
            OnInfoMessage?.Invoke($"{building.data.buildingName} construit !");
        }

        public void TriggerBuildingDemolished(Buildings.Building building)
        {
            OnBuildingDemolished?.Invoke(building);
            OnInfoMessage?.Invoke($"{building.data.buildingName} démoli.");
        }

        public void TriggerBuildingSelected(Buildings.Building building)
        {
            OnBuildingSelected?.Invoke(building);
        }
        #endregion

        #region Prisoner Event Triggers
        public void TriggerPrisonerArrived(Prisoners.Prisoner prisoner)
        {
            OnPrisonerArrived?.Invoke(prisoner);
            OnInfoMessage?.Invoke($"Nouveau détenu : {prisoner.prisonerName}");
        }

        public void TriggerPrisonerReleased(Prisoners.Prisoner prisoner)
        {
            OnPrisonerReleased?.Invoke(prisoner);
            OnInfoMessage?.Invoke($"{prisoner.prisonerName} a été libéré.");
        }

        public void TriggerPrisonerEscaped(Prisoners.Prisoner prisoner)
        {
            OnPrisonerEscaped?.Invoke(prisoner);
            OnCriticalAlert?.Invoke($"🚨 {prisoner.prisonerName} s'est échappé !");
        }

        public void TriggerPrisonerDied(Prisoners.Prisoner prisoner)
        {
            OnPrisonerDied?.Invoke(prisoner);
            OnWarning?.Invoke($"💀 {prisoner.prisonerName} est décédé.");
        }

        public void TriggerPrisonerSelected(Prisoners.Prisoner prisoner)
        {
            OnPrisonerSelected?.Invoke(prisoner);
        }

        public void TriggerEscapeAttemptFailed(Prisoners.Prisoner prisoner)
        {
            OnEscapeAttemptFailed?.Invoke(prisoner);
            OnWarning?.Invoke($"⚠️ {prisoner.prisonerName} a tenté de s'échapper !");
        }
        #endregion

        #region Resource Event Triggers
        public void TriggerMoneyChanged(float newAmount)
        {
            OnMoneyChanged?.Invoke(newAmount);
        }

        public void TriggerFoodChanged(float newAmount)
        {
            OnFoodChanged?.Invoke(newAmount);

            if (newAmount < 50)
            {
                OnWarning?.Invoke("⚠️ Stock de nourriture faible !");
            }
        }

        public void TriggerMaterialsChanged(float newAmount)
        {
            OnMaterialsChanged?.Invoke(newAmount);
        }

        public void TriggerResourcesInsufficient()
        {
            OnResourcesInsufficient?.Invoke();
            OnWarning?.Invoke("❌ Ressources insuffisantes !");
        }
        #endregion

        #region Game State Event Triggers
        public void TriggerNewDay(int day)
        {
            OnNewDay?.Invoke(day);
            OnInfoMessage?.Invoke($"📅 Jour {day}");
        }

        public void TriggerSecurityLevelChanged(int newLevel)
        {
            OnSecurityLevelChanged?.Invoke(newLevel);
        }

        public void TriggerReputationChanged(int newReputation)
        {
            OnReputationChanged?.Invoke(newReputation);

            if (newReputation < 30)
            {
                OnWarning?.Invoke("⚠️ Votre réputation est très basse !");
            }
        }

        public void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        public void TriggerGameResumed()
        {
            OnGameResumed?.Invoke();
        }

        public void TriggerGameSaved()
        {
            OnGameSaved?.Invoke();
            OnInfoMessage?.Invoke("💾 Partie sauvegardée");
        }

        public void TriggerGameLoaded()
        {
            OnGameLoaded?.Invoke();
            OnInfoMessage?.Invoke("📂 Partie chargée");
        }
        #endregion
    }
}
