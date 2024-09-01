using BepInEx;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoomTool
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.roomtool";
        public const string NAME = "Room Tool";
        public const string VERSION = "1.0.0";

        public void Awake()
        {
            BlacklistManager.Init();
            new Harmony(GUID).PatchAll();

            ETGModConsole.Commands.AddGroup("roomtool");
            var gr = ETGModConsole.Commands.GetGroup("roomtool");

            gr.AddUnit("roomname", LogRoomName);

            gr.AddGroup("blacklist");
            var bGr = gr.GetGroup("blacklist");

            bGr.AddUnit("addcurrent", BlacklistCurrent);
            bGr.AddUnit("addbyname", BlacklistByName);
            bGr.AddUnit("remove", UnblacklistRoom);
            bGr.AddUnit("clear", UnblacklistAll);
            bGr.AddUnit("list", LogBlacklist);

            ETGModConsole.CommandDescriptions["roomtool roomname"] = "Logs the name of the current room to the console.";
            ETGModConsole.CommandDescriptions["roomtool blacklist addcurrent"] = "Blacklists the current room.";
            ETGModConsole.CommandDescriptions["roomtool blacklist addbyname"] = "Blacklist a room with a given name.";
            ETGModConsole.CommandDescriptions["roomtool blacklist remove"] = "Unblacklists a room with a given name.";
            ETGModConsole.CommandDescriptions["roomtool blacklist clear"] = "Clears the room blacklist.";
            ETGModConsole.CommandDescriptions["roomtool blacklist list"] = "Logs the names of all blacklisted rooms to the console.";
        }

        public static void LogRoomName(string[] args)
        {
            if(!GameManager.HasInstance || GameManager.Instance.PrimaryPlayer == null)
            {
                ETGModConsole.Log("No active player.").Foreground = Color.red;
                return;
            }

            if(GameManager.Instance.PrimaryPlayer.CurrentRoom == null)
            {
                ETGModConsole.Log("The player is not currently in a room.").Foreground = Color.red;
                return;
            }

            ETGModConsole.Log(GameManager.Instance.PrimaryPlayer.CurrentRoom.GetRoomName());
        }

        public static void BlacklistCurrent(string[] args)
        {
            if (!GameManager.HasInstance || GameManager.Instance.PrimaryPlayer == null)
            {
                ETGModConsole.Log("No active player.").Foreground = Color.red;
                return;
            }

            if (GameManager.Instance.PrimaryPlayer.CurrentRoom == null)
            {
                ETGModConsole.Log("The player is not currently in a room.").Foreground = Color.red;
                return;
            }

            var name = GameManager.Instance.PrimaryPlayer.CurrentRoom.GetRoomName();

            if (BlacklistManager.AddToBlacklist(name))
            {
                ETGModConsole.Log($"Current room \"{name}\" successfully blacklisted.").Foreground = Color.green;
                return;
            }

            ETGModConsole.Log($"Current room \"{name}\" is already blacklisted.").Foreground = Color.yellow;
        }

        public static void BlacklistByName(string[] args)
        {
            var name = "";

            if (args.Length > 0)
                name = args[0];

            if (BlacklistManager.AddToBlacklist(name))
            {
                ETGModConsole.Log($"Room \"{name}\" successfully blacklisted.").Foreground = Color.green;
                return;
            }

            ETGModConsole.Log($"Room \"{name}\" is already blacklisted.").Foreground = Color.yellow;
        }

        public static void UnblacklistRoom(string[] args)
        {
            var name = "";

            if (args.Length > 0)
                name = args[0];

            if (BlacklistManager.RemoveFromBlacklist(name))
            {
                ETGModConsole.Log($"Room \"{name}\" successfully removed from the blacklist.").Foreground = Color.green;
                return;
            }

            ETGModConsole.Log($"Room \"{name}\" is not in the blacklist.").Foreground = Color.yellow;
        }

        public static void UnblacklistAll(string[] args)
        {
            if (BlacklistManager.ClearBlacklist())
            {
                ETGModConsole.Log("Blacklist successfully cleared.").Foreground = Color.green;
                return;
            }

            ETGModConsole.Log("The room blacklist is empty.").Foreground = Color.yellow;
        }

        public static void LogBlacklist(string[] args)
        {
            if(BlacklistManager.Blacklist.Count <= 0)
            {
                ETGModConsole.Log("The room blacklist is empty.").Foreground = Color.yellow;
                return;
            }

            ETGModConsole.Log("Current room blacklist:");

            foreach (var r in BlacklistManager.Blacklist)
                ETGModConsole.Log(r);
        }
    }
}
