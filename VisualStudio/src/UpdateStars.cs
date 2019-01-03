using UnityEngine;

namespace BetterNightSky
{
    public class UpdateStars : MonoBehaviour
    {
        private Color baseColor;
        private float lastAlpha = -1;
        private Material material;

        public void Start()
        {
            material = GetComponent<Renderer>().material;
            baseColor = material.GetColor("_TintColor");
        }

        public void Update()
        {
            float currentAlpha = GameManager.GetUniStorm().GetActiveTODState().m_StarsAlpha;
            if (Mathf.Approximately(lastAlpha, currentAlpha))
            {
                return;
            }

            lastAlpha = currentAlpha;
            material.SetColor("_TintColor", baseColor * lastAlpha * lastAlpha);
        }
    }
}