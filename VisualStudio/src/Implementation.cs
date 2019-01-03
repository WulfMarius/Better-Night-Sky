using System.IO;
using System.Reflection;
using UnityEngine;

namespace BetterNightSky
{
    internal class Implementation
    {
        private const string NAME = "Better-Night-Sky";

        private static AssetBundle assetBundle;

        private static Settings settings;

        private static GameObject moon;
        private static UpdateMoon updateMoon;

        private static GameObject starSphere;

        private static GameObject shootingStar;
        private static UpdateShootingStar updateShootingStar;

        public static int ShootingStarsFrequency {
            get => settings.ShootingStarsFrequency;
        }

        public static void OnLoad()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Log("Version " + assemblyName.Version);

            settings = Settings.Load();
            settings.AddToModSettings(NAME, ModSettings.MenuType.MainMenuOnly);

            Initialize();
        }

        internal static void ForcePhase(int phase)
        {
            if (updateMoon == null)
            {
                return;
            }

            updateMoon.SetForcedPhase(phase);
        }

        internal static void Install()
        {
            if (settings.Sky && starSphere == null)
            {
                starSphere = Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/StarSphere.prefab"));
                starSphere.transform.parent = GameManager.GetUniStorm().m_StarSphere.transform.parent;
                starSphere.transform.localEulerAngles = new Vector3(0,90,0);
                starSphere.layer = GameManager.GetUniStorm().m_StarSphere.layer;
                starSphere.AddComponent<UpdateStars>();

                moon = Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/Moon.prefab"));
                moon.transform.parent = GameManager.GetUniStorm().m_StarSphere.transform.parent.parent;
                moon.layer = GameManager.GetUniStorm().m_StarSphere.layer;
                updateMoon = moon.AddComponent<UpdateMoon>();
                updateMoon.MoonPhaseTextures = GetMoonPhaseTextures();

                GameManager.GetUniStorm().m_StarSphere.SetActive(false);
            }

            if (!settings.Sky && starSphere != null)
            {
                Object.Destroy(starSphere);
                Object.Destroy(moon);
                GameManager.GetUniStorm().m_StarSphere.SetActive(true);
            }

            if (settings.ShootingStarsFrequency > 0 && shootingStar == null)
            {
                shootingStar = Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/ShootingStar.prefab"));
                shootingStar.transform.parent = GameManager.GetUniStorm().m_StarSphere.transform.parent.parent;
                updateShootingStar = shootingStar.AddComponent<UpdateShootingStar>();
            }

            if (settings.ShootingStarsFrequency == 0 && shootingStar != null)
            {
                Object.Destroy(shootingStar);
            }
        }

        internal static void RescheduleShootingStars()
        {
            if (shootingStar == null)
            {
                return;
            }

            updateShootingStar.Reschedule();
        }

        internal static void Log(string message)
        {
            Debug.LogFormat("[" + NAME + "] {0}", message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            string preformattedMessage = string.Format("[" + NAME + "] {0}", message);
            Debug.LogFormat(preformattedMessage, parameters);
        }

        internal static void UpdateMoonPhase()
        {
            if (updateMoon == null)
            {
                return;
            }

            updateMoon.UpdatePhase();
        }

        private static Texture2D[] GetMoonPhaseTextures()
        {
            Texture2D[] result = new Texture2D[24];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = assetBundle.LoadAsset<Texture2D>("assets/MoonPhase/Moon_" + i + ".png");
            }

            return result;
        }

        private static void Initialize()
        {
            string modDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetBundlePath = Path.Combine(modDirectory, "Better-Night-Sky/Better-Night-Sky.unity3d");

            assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (assetBundle == null)
            {
                throw new FileNotFoundException("Could not load asset bundle from path '" + assetBundlePath + "'.");
            }

            uConsole.RegisterCommand("toggle-night-sky", new uConsole.DebugCommand(ToggleNightSky));
            uConsole.RegisterCommand("moon-phase", new uConsole.DebugCommand(MoonPhase));
            uConsole.RegisterCommand("shooting-star", new uConsole.DebugCommand(ShootingStar));
        }

        private static void MoonPhase()
        {
            int numParameter = uConsole.GetNumParameters();
            if (numParameter != 1)
            {
                Debug.LogError("Expected one parameter: Moon Phase Index");
                return;
            }

            ForcePhase(uConsole.GetInt());
        }

        private static void ShootingStar()
        {
            if (shootingStar == null)
            {
                Log("Shooting Stars are disabled");
                return;
            }

            int duration = 5;
            if (uConsole.GetNumParameters() == 1)
            {
                duration = uConsole.GetInt();
            }

            updateShootingStar.Trigger(duration);
        }

        private static void ToggleNightSky()
        {
            GameObject originalStarSphere = GameManager.GetUniStorm().m_StarSphere;

            starSphere.SetActive(originalStarSphere.activeSelf);
            moon.SetActive(originalStarSphere.activeSelf);
            originalStarSphere.SetActive(!originalStarSphere.activeSelf);
        }
    }
}