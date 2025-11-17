using UnityEngine;

namespace PrisonIsland.Data
{
    public enum DangerLevel
    {
        Low,
        Medium,
        High,
        Maximum
    }

    public enum PrisonerState
    {
        Idle,
        Sleeping,
        Eating,
        Working,
        Exercising,
        Plotting,
        Fighting
    }

    [System.Serializable]
    public class PrisonerStats
    {
        public float health = 100f;
        public float hunger = 30f;
        public float morale = 50f;
        public float hygiene = 80f;

        public void UpdateNeeds(float deltaTime)
        {
            hunger = Mathf.Min(100, hunger + deltaTime * 0.5f);

            if (hunger > 60 || hygiene < 40)
            {
                morale = Mathf.Max(0, morale - deltaTime * 0.3f);
            }
            else if (hunger < 40 && hygiene > 60)
            {
                morale = Mathf.Min(100, morale + deltaTime * 0.1f);
            }

            hygiene = Mathf.Max(0, hygiene - deltaTime * 0.2f);

            if (hunger > 80 || morale < 20)
            {
                health = Mathf.Max(0, health - deltaTime * 0.1f);
            }
            else if (hunger < 40 && morale > 60)
            {
                health = Mathf.Min(100, health + deltaTime * 0.05f);
            }
        }

        public void Feed()
        {
            hunger = Mathf.Max(0, hunger - 40);
            morale = Mathf.Min(100, morale + 5);
        }

        public void Shower()
        {
            hygiene = 100;
            morale = Mathf.Min(100, morale + 10);
        }

        public float GetEscapeRisk(DangerLevel dangerLevel, int escapeAttempts)
        {
            float risk = (int)dangerLevel * 10f;

            if (morale < 30) risk += 20;
            else if (morale < 50) risk += 10;

            if (hunger > 70) risk += 15;

            risk += escapeAttempts * 5;

            return Mathf.Clamp(risk, 0, 100);
        }
    }
}
