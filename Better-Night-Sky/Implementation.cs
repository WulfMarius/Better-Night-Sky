using System.IO;
using System.Reflection;

using UnityEngine;

namespace BetterNightSky
{
    internal class Implementation
    {
        private static AssetBundle assetBundle;

        private static GameObject originalStarSphere;
        private static GameObject starSphere;
        private static GameObject moon;
        private static UpdateMoon updateMoon;

        public static void Initialize()
        {
            string modDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetBundlePath = Path.Combine(modDirectory, "better-night-sky");

            assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (assetBundle == null)
            {
                throw new FileNotFoundException("Could not load asset bundle from path '" + assetBundlePath + "'.");
            }

            uConsole.RegisterCommand("toggle-night-sky", new uConsole.DebugCommand(ToggleNightSky));
            uConsole.RegisterCommand("moon-phase", new uConsole.DebugCommand(MoonPhase));
        }

        public static void Install()
        {
            if (assetBundle == null)
            {
                return;
            }

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

        public static void UpdateMoonPhase()
        {
            if (updateMoon == null)
            {
                return;
            }

            updateMoon.UpdatePhase();
        }

        public static void ForcePhase(int phase)
        {
            if (updateMoon == null)
            {
                return;
            }

            updateMoon.SetForcedPhase(phase);
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
    }
}
