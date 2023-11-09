using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace RecycletoInventory
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }
        public const string GUID = "Desperationfighter.TPC.RecycletoInventory";
        public const string Name = "Recycle to Inventory";
        public const string Version = "1.0.0.0"; //Remmber to Update Assembly Version too !

        public static ConfigEntry<bool> ModisActive;
        public static ConfigEntry<bool> Debuglogging;
        public static ConfigEntry<bool> Sound;

        private void Awake()
        {
            ModisActive = Config.Bind("1_General", "ModisActive", true, "Set if the Mod should running or not. If you don't want to remove Files or for Later Ingame Menu. Please reload your Savegame after Change as there are Setting that only apply once when World is loaded up.");
            Debuglogging = Config.Bind("9_Advanced", "Debuglogging", false, "Enables Debug Logging. Should be only activated when you know what you do.");
            Sound = Config.Bind("2_Config", "Sound", true, "Gives Audio Feedback or not.");

            // set project-scoped logger instance
            Logger = base.Logger;

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
            Logger.LogInfo($"Plugin {GUID} is loaded!");
        }

        public static void MyDebugLogger(string message)
        {
            if (Debuglogging.Value)
            {
                Logger.LogDebug($"[{Name}][Debug] : {message} [/Debug]");
            }
        }
    }
}
