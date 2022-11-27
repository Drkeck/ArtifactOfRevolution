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
                            Logger.LogInfo($"{(Run.instance.stageClearCount + 1)/whiteMod} white items added to enemy team");
                            if (Run.instance.stageClearCount/whiteMod < 1)
                            {
                                return 1;
                            }
                            return Run.instance.stageClearCount/whiteMod;
                        case ItemTier.Tier2:
                        case ItemTier.VoidTier2:
                            Logger.LogInfo($"{(Run.instance.stageClearCount + 1)/greenMod} green items added to enemy team");
                            if (Run.instance.stageClearCount/greenMod < 1)
                            {
                                return 1;
                            }
                            return Run.instance.stageClearCount/greenMod;
                        case ItemTier.Tier3:
                        case ItemTier.VoidTier3:
                            Logger.LogInfo($"{(Run.instance.stageClearCount + 1)/redMod} red items added to enemy team");
                            if (Run.instance.stageClearCount/redMod < 1)
                            {
                                return 1;
                            }
                            return Run.instance.stageClearCount/redMod;
                        default:
                            return itemCount;
                    }
                });
            };

            // we have other methods we can call but i am still trying to figure out what sort of manipulation i am able to do with this thing
            On.RoR2.SceneDirector.Start += (orig, self) =>
            {
            Logger.LogInfo(Run.instance.stageClearCount / 2);
                orig(self);
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
    }
}
