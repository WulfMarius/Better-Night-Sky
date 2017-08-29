using UnityEngine;

namespace BetterNightSky
{
    internal class UpdateMoon : MonoBehaviour
    {
        public const int MOON_CYCLE_DAYS = 29;

        public Texture2D[] MoonPhaseTextures;

        private Material material;
        private Color baseColor;

        private float lastAlpha;
        private int lastPhaseTextureIndex;
        private int forcedPhase = -1;

        public void Start()
        {
            material = GetComponentInChildren<Renderer>().material;
            baseColor = material.GetColor("_TintColor");
        }

        public void Update()
        {
            UpdateAlpha();
            UpdateDirection();
        }

        public void UpdatePhase()
        {
            int phaseTextureIndex = GetPhaseTextureIndex();
            if (lastPhaseTextureIndex == phaseTextureIndex)
            {
                return;
            }

            lastPhaseTextureIndex = phaseTextureIndex;
            material.mainTexture = MoonPhaseTextures[lastPhaseTextureIndex];
        }

        public void SetForcedPhase(int forcedPhase)
        {
            this.forcedPhase = forcedPhase;
            UpdatePhase();
        }

        private void UpdateAlpha()
        {
            float currentAlpha = GameManager.GetUniStorm().GetActiveTODState().m_MoonAlpha;
            if (Mathf.Approximately(lastAlpha, currentAlpha))
            {
                return;
            }

            lastAlpha = currentAlpha;
            material.SetColor("_TintColor", baseColor * lastAlpha);
        }

        private void UpdateDirection()
        {
            transform.forward = -GameManager.GetUniStorm().m_MoonLight.transform.forward;
        }

        private int GetPhaseTextureIndex()
        {
            if (forcedPhase >= 0)
            {
                return forcedPhase;
            }

            UniStormWeatherSystem uniStormWeatherSystem = GameManager.GetUniStorm();
            int day = uniStormWeatherSystem.GetDayNumber() + uniStormWeatherSystem.m_MoonCycleStartDay;
            return day % MOON_CYCLE_DAYS * MoonPhaseTextures.Length / MOON_CYCLE_DAYS;
        }
    }
}
