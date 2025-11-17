using UnityEngine;
using System.Collections.Generic;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Manages the tutorial and onboarding experience
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [Header("Tutorial Settings")]
        [SerializeField] private bool enableTutorial = true;
        [SerializeField] private GameObject tutorialPanel;

        private Queue<TutorialStep> tutorialSteps = new Queue<TutorialStep>();
        private TutorialStep currentStep;
        private bool tutorialCompleted = false;

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
            LoadTutorialProgress();

            if (enableTutorial && !tutorialCompleted)
            {
                InitializeTutorial();
            }
        }

        private void InitializeTutorial()
        {
            // Step 1: Welcome
            tutorialSteps.Enqueue(new TutorialStep
            {
                title = "Bienvenue !",
                message = "Bienvenue sur Prison Island ! Vous allez gérer une prison sur une île isolée.",
                highlight = null,
                condition = () => true
            });

            // Step 2: Resources
            tutorialSteps.Enqueue(new TutorialStep
            {
                title = "Ressources",
                message = "En haut, vous voyez vos ressources : Argent 💰, Nourriture 🍞, Matériaux 🧱, Sécurité 🛡️ et Réputation ⭐",
                highlight = "ResourceBar",
                condition = () => true
            });

            // Step 3: First Building
            tutorialSteps.Enqueue(new TutorialStep
            {
                title = "Construction",
                message = "Cliquez sur le bouton + pour construire votre première cellule !",
                highlight = "BuildButton",
                condition = () => GameManager.Instance.buildings.Count > 3
            });

            // Step 4: Prisoners
            tutorialSteps.Enqueue(new TutorialStep
            {
                title = "Détenus",
                message = "Les détenus arrivent automatiquement. Cliquez sur un détenu pour voir ses informations.",
                highlight = null,
                condition = () => GameManager.Instance.prisoners.Count > 0
            });

            // Step 5: Feeding
            tutorialSteps.Enqueue(new TutorialStep
            {
                title = "Nourriture",
                message = "N'oubliez pas de nourrir vos détenus ! Un détenu affamé peut devenir dangereux.",
                highlight = null,
                condition = () => true
            });

            // Step 6: Security
            tutorialSteps.Enqueue(new TutorialStep
            {
                title = "Sécurité",
                message = "Construisez des Tours de Garde pour augmenter la sécurité et prévenir les évasions.",
                highlight = null,
                condition = () => true
            });

            // Step 7: Complete
            tutorialSteps.Enqueue(new TutorialStep
            {
                title = "Félicitations !",
                message = "Vous savez maintenant gérer votre prison ! Bonne chance, directeur !",
                highlight = null,
                condition = () => true
            });

            ShowNextStep();
        }

        public void ShowNextStep()
        {
            if (tutorialSteps.Count > 0)
            {
                currentStep = tutorialSteps.Dequeue();
                DisplayStep(currentStep);
            }
            else
            {
                CompleteTutorial();
            }
        }

        private void DisplayStep(TutorialStep step)
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
                // Update UI with step info
                // This would be implemented with actual UI elements
            }

            Debug.Log($"[Tutorial] {step.title}: {step.message}");

            // Highlight element if specified
            if (!string.IsNullOrEmpty(step.highlight))
            {
                HighlightElement(step.highlight);
            }
        }

        private void HighlightElement(string elementName)
        {
            // Find and highlight the UI element
            GameObject element = GameObject.Find(elementName);
            if (element != null)
            {
                // Add highlight effect
                // This would be implemented with actual highlighting
            }
        }

        public void CheckStepCondition()
        {
            if (currentStep != null && currentStep.condition != null)
            {
                if (currentStep.condition.Invoke())
                {
                    ShowNextStep();
                }
            }
        }

        private void CompleteTutorial()
        {
            tutorialCompleted = true;
            SaveTutorialProgress();

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }

            Debug.Log("[Tutorial] Tutorial completed!");
        }

        public void SkipTutorial()
        {
            tutorialSteps.Clear();
            CompleteTutorial();
        }

        private void SaveTutorialProgress()
        {
            PlayerPrefs.SetInt("TutorialCompleted", tutorialCompleted ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadTutorialProgress()
        {
            tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        }

        [System.Serializable]
        private class TutorialStep
        {
            public string title;
            public string message;
            public string highlight;
            public System.Func<bool> condition;
        }
    }
}
