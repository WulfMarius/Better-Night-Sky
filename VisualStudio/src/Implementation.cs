using System.IO;
using System.Reflection;
using UnityEngine;

namespace BetterNightSky
{
    internal class Implementation
    {
        private static AssetBundle assetBundle;

        private static GameObject moon;
        private static GameObject originalStarSphere;
        private static GameObject starSphere;
        private static UpdateMoon updateMoon;

        public static void OnLoad()
        {
            Debug.Log("[Better-Night-Sky]: Version " + Assembly.GetExecutingAssembly().GetName().Version);

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
            UniStormWeatherSystem uniStorm = GameManager.GetUniStorm();
            originalStarSphere = uniStorm.m_StarSphere;
            originalStarSphere.SetActive(false);

            starSphere = Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/StarSphere.prefab"));
            starSphere.transform.parent = originalStarSphere.transform.parent;
            starSphere.layer = originalStarSphere.layer;
            starSphere.AddComponent<UpdateStars>();

            moon = Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/Moon.prefab"));
            moon.transform.parent = originalStarSphere.transform.parent.parent;
            moon.layer = originalStarSphere.layer;
            updateMoon = moon.AddComponent<UpdateMoon>();
            updateMoon.MoonPhaseTextures = GetMoonPhaseTextures();
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
            string assetBundlePath = Path.Combine(modDirectory, "better-night-sky/better-night-sky.unity3d");

            assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (assetBundle == null)
            {
                throw new FileNotFoundException("Could not load asset bundle from path '" + assetBundlePath + "'.");
            }

            uConsole.RegisterCommand("toggle-night-sky", new uConsole.DebugCommand(ToggleNightSky));
            uConsole.RegisterCommand("moon-phase", new uConsole.DebugCommand(MoonPhase));
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

        private static void ToggleNightSky()
        {
            if (originalStarSphere.activeSelf)
            {
                originalStarSphere.SetActive(false);
                starSphere.SetActive(true);
                moon.SetActive(true);
            }
            else
            {
                originalStarSphere.SetActive(true);
                starSphere.SetActive(false);
                moon.SetActive(false);
            }
        }
    }
}