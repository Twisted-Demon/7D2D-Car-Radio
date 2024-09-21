using HarmonyLib;
using UnityEngine;

namespace Harmony;

[HarmonyPatch(typeof(EntityVehicle))]
[HarmonyPatch("Init")]
public class EntityVehicleInit
{
    public static void Postfix(EntityVehicle __instance)
    {
        var gameObject = __instance.gameObject;
        var carRadio = gameObject.AddComponent<CarRadioComponent>();

        carRadio.entityVehicle = __instance;
    }
}