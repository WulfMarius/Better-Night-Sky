using Harmony;

namespace BetterNightSky
{
    [HarmonyPatch(typeof(TODStateConfig), "SetBlended")]
    internal class TODStateConficSetBlendedPatch
    {
        public static void Postfix(TODStateConfig __instance, int nightStates)
        {
            if (nightStates > 0)
            {
                __instance.m_SkyBloomIntensity *= 0.3f;
            }
        }
    }

    [HarmonyPatch(typeof(UniStormWeatherSystem), "Init")]
    internal class UniStormWeatherSystemInitPatch
    {
        public static void OnLoad()
        {
            Implementation.Initialize();
        }

        public static void Prefix()
        {
            Implementation.Install();
        }
    }

    [HarmonyPatch(typeof(UniStormWeatherSystem), "SetMoonPhaseIndex")]
    internal class UniStormWeatherSystemSetMoonPhaseIndexPatch
    {
        public static void Postfix()
        {
            Implementation.UpdateMoonPhase();
        }
    }

    [HarmonyPatch(typeof(UniStormWeatherSystem), "SetMoonPhase")]
    internal class UniStormWeatherSystemSetMoonPhasePatch
    {
        public static void Postfix()
        {
            Implementation.UpdateMoonPhase();
        }
    }
}