using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace RoomTool
{
    [HarmonyPatch]
    public static class Patches
    {
        public static MethodInfo fbs_fbl_rl = AccessTools.Method(typeof(Patches), nameof(FixBossSelection_FixBossesList_ReplaceList));

        [HarmonyPatch(typeof(PrototypeDungeonRoom), nameof(PrototypeDungeonRoom.CheckPrerequisites))]
        [HarmonyPostfix]
        public static void InvalidateBlacklistedRoom_Postfix(PrototypeDungeonRoom __instance, ref bool __result)
        {
            if (!__result || __instance == null || !BlacklistManager.Blacklist.Contains(__instance.name))
                return;

            __result = false;
        }

        [HarmonyPatch(typeof(IndividualBossFloorEntry), nameof(IndividualBossFloorEntry.GlobalPrereqsValid))]
        [HarmonyPostfix]
        public static void FixBossSelection_InvalidateBlacklistedBoss_Postfix(IndividualBossFloorEntry __instance, ref bool __result)
        {
            if (!__result)
                return;

            if (__instance == null || __instance.TargetRoomTable == null || __instance.TargetRoomTable.includedRooms == null)
                return;

            if (__instance.TargetRoomTable.includedRooms.elements == null || __instance.TargetRoomTable.includedRooms.elements.Count <= 0)
                return;

            var elements = __instance.TargetRoomTable.includedRooms.elements;

            foreach (var e in elements)
            {
                if (e == null || e.room == null || !BlacklistManager.Blacklist.Contains(e.room.name))
                    return;
            }

            __result = false;
        }

        [HarmonyPatch(typeof(OverrideBossFloorEntry), nameof(OverrideBossFloorEntry.GlobalPrereqsValid))]
        [HarmonyPostfix]
        public static void FixBossSelection_InvalidateBlacklistedOverrideBoss_Postfix(OverrideBossFloorEntry __instance, ref bool __result)
        {
            if (!__result)
                return;

            if (__instance == null || __instance.TargetRoomTable == null || __instance.TargetRoomTable.includedRooms == null)
                return;

            if (__instance.TargetRoomTable.includedRooms.elements == null || __instance.TargetRoomTable.includedRooms.elements.Count <= 0)
                return;

            var elements = __instance.TargetRoomTable.includedRooms.elements;

            foreach (var e in elements)
            {
                if (e == null || e.room == null || !BlacklistManager.Blacklist.Contains(e.room.name))
                    return;
            }

            __result = false;
        }

        [HarmonyPatch(typeof(BossFloorEntry), nameof(BossFloorEntry.SelectBoss))]
        [HarmonyILManipulator]
        public static void FixBossSelection_FixBossesList_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchLdfld<BossFloorEntry>(nameof(BossFloorEntry.Bosses)), 8))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.Emit(OpCodes.Call, fbs_fbl_rl);
        }

        public static List<IndividualBossFloorEntry> FixBossSelection_FixBossesList_ReplaceList(List<IndividualBossFloorEntry> _, List<IndividualBossFloorEntry> list)
        {
            return list;
        }
    }
}
