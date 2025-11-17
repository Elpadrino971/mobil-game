using System;
using UnityEngine;

namespace PrisonIsland.Data
{
    [Serializable]
    public class GameResources
    {
        public float money = 1000f;
        public float food = 100f;
        public float materials = 50f;
        public float security = 50f;
        public float reputation = 50f;

        public bool CanAfford(float moneyCost, float foodCost = 0, float materialsCost = 0)
        {
            return money >= moneyCost && food >= foodCost && materials >= materialsCost;
        }

        public void Deduct(float moneyCost, float foodCost = 0, float materialsCost = 0)
        {
            money -= moneyCost;
            food -= foodCost;
            materials -= materialsCost;
        }

        public void Add(float moneyAmount = 0, float foodAmount = 0, float materialsAmount = 0, float securityAmount = 0, float reputationAmount = 0)
        {
            money += moneyAmount;
            food += foodAmount;
            materials += materialsAmount;
            security = Mathf.Clamp(security + securityAmount, 0, 100);
            reputation = Mathf.Clamp(reputation + reputationAmount, 0, 100);
        }

        public void ConsumeDaily(int prisonerCount)
        {
            food -= prisonerCount * 3f;
            money -= prisonerCount * 0.5f;
        }

        public void AddDailyIncome(int prisonerCount)
        {
            money += prisonerCount * 5f;
        }

        public void ClampValues()
        {
            money = Mathf.Max(0, money);
            food = Mathf.Max(0, food);
            materials = Mathf.Max(0, materials);
            security = Mathf.Clamp(security, 0, 100);
            reputation = Mathf.Clamp(reputation, 0, 100);
        }
    }
}
