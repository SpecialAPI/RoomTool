using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoomTool
{
    public static class BlacklistManager
    {
        public static readonly HashSet<string> Blacklist = [];

        public const string BlacklistFileName = "RoomBlacklist.txt";
        public static string BlacklistFilePath;

        public static void Init()
        {
            BlacklistFilePath = Path.Combine(Paths.ConfigPath, BlacklistFileName);
            LoadBlacklist();
        }

        public static void LoadBlacklist()
        {
            Blacklist.Clear();

            if (!File.Exists(BlacklistFilePath))
                return;

            foreach (var l in File.ReadAllLines(BlacklistFilePath))
                Blacklist.Add(l);
        }

        public static bool AddToBlacklist(string elem)
        {
            if (!Blacklist.Add(elem))
                return false;

            WriteBlacklist();
            return true;
        }

        public static bool RemoveFromBlacklist(string elem)
        {
            if (!Blacklist.Remove(elem))
                return false;

            WriteBlacklist();
            return true;
        }

        public static bool ClearBlacklist()
        {
            if (Blacklist.Count <= 0)
                return false;

            Blacklist.Clear();
            WriteBlacklist();
            return true;
        }

        public static void WriteBlacklist()
        {
            var dirName = Path.GetDirectoryName(BlacklistFilePath);

            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            File.WriteAllLines(BlacklistFilePath, [.. Blacklist]);
        }
    }
}
