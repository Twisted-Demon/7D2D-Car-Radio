using UnityEngine;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Harmony;

public class CarRadioMod : IModApi
{
    public void InitMod(Mod modInstance)
    {
        var harmony = new HarmonyLib.Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Debug.Log("Loading Asset Bundle");
        
        SAssetBundleManager.Instance.Initialize();
        
        if(SAssetBundleManager.Instance.Data == null)
            Debug.LogError("Could not Load bundle");
    }
}