using System;
using TheForest.Buildings.Creation;
using TheForest.Items.Craft;
using UnityEngine;

namespace SuperEasyBuild.Patches
{
    public class Craft_Structure_Patcher: Craft_Structure
    {
        [ModAPI.Attributes.Priority(2)]
        public override void Initialize()
        {
            ModAPI.Log.Write($"Initializing");
            var buildingType = Enum.GetName(typeof(BuildingTypes), this._type);
            ModAPI.Log.Write($"[{buildingType}] Initialized");
            if (this._initialized)
                return;
            ModAPI.Log.Write($"[{buildingType}] Checking ingredients");
            if (this._presentIngredients == null)
            {
                ModAPI.Log.Write($"[{buildingType}] Ingredients no present, creating {_requiredIngredients.Count}. ");
                this._presentIngredients = new ReceipeIngredient[this._requiredIngredients.Count];
            }

            if (this._presentIngredients.Length != this._requiredIngredients.Count)
            {
                ModAPI.Log.Write($"[{buildingType}] Filling ingredients");
                this._presentIngredients = new ReceipeIngredient[this._requiredIngredients.Count];
            }

            for (int index = 0; index < this._requiredIngredients.Count; ++index)
            {
                ModAPI.Log.Write($"[{buildingType}] Ingredient at {index}");
                Craft_Structure.BuildIngredients requiredIngredient = this._requiredIngredients[index];
                ModAPI.Log.Write($"[{buildingType}:{requiredIngredient._itemID}] Material found for {index} {requiredIngredient._itemID}");
                var prevAmount = requiredIngredient?._amount;
                var toReduce = 1.0f;
                if (requiredIngredient._itemID == 78) // log
                {
                    toReduce = 0.1f;
                }
                else if (prevAmount >= 40)
                {
                    toReduce = 0.15f;
                } else if (prevAmount >=5)
                {
                    toReduce = 0.4f;
                }
                ModAPI.Log.Write($"[{buildingType}:{requiredIngredient._itemID}] Default amount {prevAmount}, reduction {toReduce}");
                requiredIngredient._amount = Mathf.Max(1, (int)Math.Round(requiredIngredient._amount * toReduce));
                ModAPI.Log.Write($"[{buildingType}:{requiredIngredient._itemID}] new amount {requiredIngredient._amount}");
                if (this._presentIngredients[index] == null)
                    this._presentIngredients[index] = new ReceipeIngredient()
                    {
                        _itemID = requiredIngredient._itemID,
                        _amount = 0
                    };
                ReceipeIngredient presentIngredient = this._presentIngredients[index];
                int amount = requiredIngredient._amount - presentIngredient._amount;
                BuildMission.AddNeededToBuildMission(requiredIngredient._itemID, amount, true);
                requiredIngredient.SetBuilt(presentIngredient._amount);
            }
            this._initialized = true;
            if (BoltNetwork.isRunning)
            {
                this.gameObject.AddComponent<CoopConstruction>();
                if (BoltNetwork.isServer && this.entity.isAttached)
                    this.UpdateNetworkIngredients();
            }
            if (BoltNetwork.isClient)
                return;
            this.CheckNeeded();
        }
    }
}