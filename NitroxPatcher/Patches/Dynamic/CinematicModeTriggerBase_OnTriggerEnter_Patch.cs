using System.Reflection;
using HarmonyLib;
using NitroxClient.Communication.Abstract;
using NitroxClient.MonoBehaviours;
using NitroxClient.MonoBehaviours.Overrides;
using NitroxClient.Unity.Helper;
using NitroxModel.Helper;
using UnityEngine;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Two players in the elevator breaks the game hard with synced animation. So we are looking if it's already playing and pushing the player back if so.
/// </summary>
public class CinematicModeTriggerBase_OnTriggerEnter_Patch : NitroxPatch, IDynamicPatch
{
    private static readonly MethodInfo targetMethod = Reflect.Method((CinematicModeTriggerBase t) => t.OnTriggerEnter(default));
    private static readonly MethodInfo targetMethod2 = Reflect.Method((CinematicModeTriggerBase t) => t.OnTriggerExit(default));

    private static readonly int elevatorAscend = Animator.StringToHash("elevator_ascend");
    private static readonly int elevatorAscendPrepare = Animator.StringToHash("elevator_ascend_prepare");
    private static readonly int elevatorDecend = Animator.StringToHash("elevator_decend");
    private static readonly int elevatorDecendPrepare = Animator.StringToHash("elevator_decend_prepare");

    public static bool Prefix(CinematicModeTriggerBase __instance, Collider collider)
    {
        if (__instance.triggerType != CinematicModeTriggerBase.TriggerType.Volume)
        {
            return false;
        }

        Player componentInHierarchy = UWE.Utils.GetComponentInHierarchy<Player>(collider.gameObject);
        if (!componentInHierarchy)
        {
            return false;
        }

        if (__instance.cinematicController.animator.GetBool(elevatorAscend) ||
            __instance.cinematicController.animator.GetBool(elevatorDecend) ||
            __instance.cinematicController.animator.GetBool(elevatorAscendPrepare) ||
            __instance.cinematicController.animator.GetBool(elevatorDecendPrepare))
        {
            Player.main.transform.position += Vector3.right;
            Log.InGame("The Elevator is occupied at the moment");
            return false;
        }

        return true;
    }

    public override void Patch(Harmony harmony)
    {
        //PatchPrefix(harmony, targetMethod);
    }
}
