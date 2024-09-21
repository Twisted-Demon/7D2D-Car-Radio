using Audio;
using HarmonyLib;
using UnityEngine;

namespace Harmony;

[HarmonyPatch(typeof(EntityPlayerLocal))]
[HarmonyPatch("Update")]
public class EntityPlayerLocalUpdate
{
    public static void PostFix(EntityPlayerLocal __instance)
    {
        var entitiesAttached = "";

        foreach (var entity in __instance.attachedEntities)
            entitiesAttached += $"{entity.name}, ";
        
        Debug.LogWarning($"Attached to: {entitiesAttached}");
        
        
    }
}
