using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Artifacts;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;



namespace MyFirstPlugin
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<string> configAoE;
        private ConfigEntry<bool> configAoEDisplay;

        int whiteMod = 2;
        int greenMod = 3;
        int redMod = 4;

        private void Awake()
        {
            // binding for config generation.
            configAoE = Config.Bind("Difficulty",
                "DifficultySetting",
                "Normal",
                "A difficulty setting for how hard the item generation gets. Vanilla, Normal, Hard, Chaos");

            configAoEDisplay = Config.Bind("Toggles",
                "DisplayDifficulty",
                true,
                "show what diff you're on.");

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            //other logic
            if(configAoEDisplay.Value)
                Logger.LogInfo(configAoE.Value);


            // this is ripped from the other mod in the folder.
            IL.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCallvirt<Inventory>("GiveItem")
                    );
                c.Emit(OpCodes.Ldloc_2);
                c.EmitDelegate<Func<int, PickupDef, int>>((itemCount, pickup) =>
                {
                    switch (pickup.itemTier)
                    {
                        case ItemTier.Tier1:
                        case ItemTier.VoidTier1:
                            Logger.LogInfo($"{AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(2, configAoE.Value)} white items added to enemy team");
                            if (AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(2, configAoE.Value) < 1)
                            {
                                return 1;
                            }
                            return AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(2, configAoE.Value);
                        case ItemTier.Tier2:
                        case ItemTier.VoidTier2:
                            Logger.LogInfo($"{AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(3, configAoE.Value)} green items added to enemy team");
                            if (AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(3, configAoE.Value) < 1)
                            {
                                return 1;
                            }
                            return AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(3, configAoE.Value);
                        case ItemTier.Tier3:
                        case ItemTier.VoidTier3:
                            Logger.LogInfo($"{AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(5, configAoE.Value)} red items added to enemy team");
                            if (AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(5, configAoE.Value) < 1)
                            {
                                return 1;
                            }
                            return AccurateStageCount(Run.instance.stageClearCount)/DifficultyTable(5, configAoE.Value);
                        default:
                            return itemCount;
                    }
                });
            };
        }

        public static void GrantMonsterTeamItem(string itemName, int count)
        {
            ItemIndex item = ItemCatalog.FindItemIndex(itemName);
            if (item != ItemIndex.None)
            {
                MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.GiveItem(item, count);
            }
            else
            {
                Debug.LogError("Item not found. If you have DebugToolkit installed, type list_item to view a list of all internal item names. You must enter the item name exactly as it appears in the list.");
            }
        }
        public static void GrantMonsterTeamItem(ItemIndex itemIndex, int count)
        {
            MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.GiveItem(itemIndex, count);
        }

        private static int DifficultyTable(int itemTier, string diff)
        {
            switch (diff)
            {
                case "Normal":
                    return itemTier;
                case "Hard":
                    return itemTier/2;
                case "Chaos":
                    return itemTier/3;
                default:
                    return Run.instance.stageClearCount;
            }
        }

        private static int AccurateStageCount(int stageCount)
        {
            return stageCount + 1;
        }
    }
}
