using HarmonyLib;
using SpaceCraft;
using System.Collections.Generic;
using UnityEngine;

namespace RecycletoInventory.Patches
{
    [HarmonyPatch(typeof(ActionRecycle))]
    [HarmonyPatch(nameof(ActionRecycle.OnAction))]
    public static class ActionRecycle_Patches
    {
        [HarmonyPrefix]
        static bool Prefix(ActionRecycle __instance)
        {
            if (!Plugin.ModisActive.Value) return true;

            OnAction_Patch(__instance);

            return false;
        }

        static void OnAction_Patch(ActionRecycle __instance)
        {
            __instance.tempCage.SetActive(true);
            Inventory inventory = __instance.GetComponentInParent<InventoryAssociated>().GetInventory();
            if (inventory.GetInsideWorldObjects().Count == 0)
            {
                return;
            }
            WorldObject worldObject = inventory.GetInsideWorldObjects()[0];
            List<Group> ingredientsGroupInRecipe = worldObject.GetGroup().GetRecipe().GetIngredientsGroupInRecipe();

            //Get Play Inventory for Later Checks
            Inventory inventory_Player = Managers.GetManager<PlayersManager>().GetActivePlayerController().GetPlayerBackpack().GetInventory();
            //
            GlobalAudioHandler globalAudioHandler = Managers.GetManager<GlobalAudioHandler>();
            //Set Warning Flag to intial State
            bool alreadywarned = false;

            if (ingredientsGroupInRecipe.Count == 0 || ((GroupItem)worldObject.GetGroup()).GetCantBeRecycled())
            {
                WorldObject worldObject_tmp = WorldObjectsHandler.CreateAndDropOnFloor(worldObject.GetGroup(), __instance.craftSpawn.transform.position, 0f);
                if (inventory_Player.IsFull())
                {
                    if (!alreadywarned)
                    {
                        alreadywarned = true;
                    }
                }
                else
                {
                    if (inventory_Player.AddItem(worldObject_tmp))
                    {
                        Object.Destroy(worldObject_tmp.GetGameObject());
                        worldObject_tmp.SetDontSaveMe(false);
                    }
                }
            }
            else
            {

                foreach (Group group in ingredientsGroupInRecipe)
                {
                    Vector3 vector = new Vector3(Random.Range(__instance.craftSpawn.bounds.min.x, __instance.craftSpawn.bounds.max.x), Random.Range(__instance.craftSpawn.bounds.min.y, __instance.craftSpawn.bounds.max.y), Random.Range(__instance.craftSpawn.bounds.min.z, __instance.craftSpawn.bounds.max.z));
                    WorldObject worldObject_tmp = WorldObjectsHandler.CreateAndDropOnFloor(group, vector, 0.6f);

                    if (inventory_Player.IsFull())
                    {
                        if (!alreadywarned)
                        {
                            alreadywarned = true;
                        }
                    }
                    else
                    {
                        if (inventory_Player.AddItem(worldObject_tmp))
                        {
                            Object.Destroy(worldObject_tmp.GetGameObject());
                            worldObject_tmp.SetDontSaveMe(false);
                        }
                    }
                }
            }

            if (alreadywarned)
            {
                Managers.GetManager<BaseHudHandler>().DisplayCursorText("UI_WARN_RECYLCE_INV_FULL", 2f, "Inventory Full");
                if (Plugin.Sound.Value)
                {
                    globalAudioHandler.PlayDropObject();
                }
            }
            else
            {
                Managers.GetManager<BaseHudHandler>().DisplayCursorText("UI_INFO_RECYLCE_TO_INV", 2f, "Transfered to Inventory");
                if (Plugin.Sound.Value)
                {
                    globalAudioHandler.PlayUiSelectElement();
                }
            }

            inventory.RemoveItem(worldObject, true);
            __instance.Invoke("DisableCage", 1.5f);
            __instance.OnAction();
        }
    }
}
