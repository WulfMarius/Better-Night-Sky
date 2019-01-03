using ModSettings;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BetterNightSky
{
    internal class Settings : ModSettingsBase
    {
        [Name("Shooting Stars Frequency")]
        [Description("How often shootings stars will appear in the sky.")]
        [Choice("None", "Low", "Medium", "High")]
        public int ShootingStarsFrequency = 2;

        [Name("Replace Sky")]
        [Description("If enabled, the night sky and moon will be replaced.")]
        public bool Sky = true;

        private static readonly string MODS_FOLDER_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string SETTINGS_PATH = Path.Combine(MODS_FOLDER_PATH, "Better-Night-Sky.json");

        internal static Settings Load()
        {
            if (!File.Exists(SETTINGS_PATH))
            {
                Implementation.Log("Settings file did not exist, using default settings.");
                return new Settings();
            }

            try
            {
                string json = File.ReadAllText(SETTINGS_PATH, System.Text.Encoding.UTF8);
                return JsonUtility.FromJson<Settings>(json);
            }
            catch (Exception ex)
            {
                Implementation.Log("Error while trying to read config file:");
                Debug.LogException(ex);

                // Re-throw to make error show up in main menu
                throw new IOException("Error while trying to read config file", ex);
            }
        }

        internal void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(this, true);
                File.WriteAllText(SETTINGS_PATH, json, System.Text.Encoding.UTF8);
                Implementation.Log("Config file saved to " + SETTINGS_PATH);
            }
            catch (Exception ex)
            {
                Implementation.Log("Error while trying to write config file:");
                Debug.LogException(ex);
            }
        }

        protected override void OnConfirm()
        {
            this.Save();
            Implementation.Install();
        }
    }
}