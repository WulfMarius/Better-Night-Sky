using UnityEngine;

namespace BetterNightSky
{
    public class UpdateShootingStar : MonoBehaviour
    {
        private const float ALPHA_MAX = 0.75f;
        private const float COLOR_MAX = 0.90f;
        private const float COLOR_MIN = 0.60f;

        private const int DELAY_MAX = 1800;
        private const int DELAY_MIN = 900;
        private const int DURATION_MAX = 30;
        private const int DURATION_MIN = 2;

        private const float HEIGHT = 2000;
        private const float POSITION_MAX = 2000;
        private const float POSITION_MIN = -2000;

        private const int PARTICLES_MAX = 30;
        private const int PARTICLES_MIN = 1;

        private ParticleSystem particleSystem;

        internal void Trigger(int duration = 0)
        {
            this.CancelInvoke();

            int actualDuration = duration < 1 ? Random.Range(DURATION_MIN, DURATION_MAX) : duration;
            this.Invoke("StopEmitting", actualDuration);

            this.StartEmitting();
        }

        private static int GetNextDelay()
        {
            int result = Random.Range(DELAY_MIN, DELAY_MAX) / Implementation.ShootingStarsFrequency;

            if (IsMainMenu())
            {
                result /= 10;
            }

            return result;
        }

        private static int GetNextDuration()
        {
            return Random.Range(DURATION_MIN, DURATION_MAX);
        }

        internal static bool IsMainMenu()
        {
            return "MainMenu" == GameManager.m_ActiveScene;
        }

        private bool CanEmit()
        {
            if (GameManager.GetWeatherComponent().IsIndoorScene())
            {
                return false;
            }

            if (GameManager.GetUniStorm().GetActiveTODState().m_MoonAlpha < 0.05)
            {
                return false;
            }

            return true;
        }

        private void Start()
        {
            this.particleSystem = this.GetComponentInChildren<ParticleSystem>();
            if (this.particleSystem == null)
            {
                Implementation.Log("Particle system not found!");
                this.gameObject.SetActive(false);
                return;
            }

            this.StopEmitting();
        }

        private void StartEmitting()
        {
            if (!this.CanEmit())
            {
                return;
            }

            UpdatePosition();
            UpdateColor();

            ParticleSystem.EmissionModule emissionModule = this.particleSystem.emission;
            emissionModule.enabled = true;
        }

        private void StopEmitting()
        {
            if (this.particleSystem == null)
            {
                return;
            }

            ParticleSystem.EmissionModule emissionModule = this.particleSystem.emission;
            emissionModule.enabled = false;
        }

        internal void Reschedule()
        {
            this.CancelInvoke();
            this.StopEmitting();

            int delay = GetNextDelay();
            this.Invoke("StartEmitting", delay);

            int duration = GetNextDuration();
            this.Invoke("Reschedule", delay + duration);

            Implementation.Log("Scheduled next shooting stars in " + delay + " seconds for " + duration + " seconds.");
        }

        private void UpdateColor()
        {
            float currentAlpha = Mathf.Clamp(GameManager.GetUniStorm().GetActiveTODState().m_MoonAlpha, 0, ALPHA_MAX);

            Color minColor = new Color(Random.Range(COLOR_MIN, COLOR_MAX), Random.Range(COLOR_MIN, COLOR_MAX), Random.Range(COLOR_MIN, COLOR_MAX), currentAlpha);
            Color maxColor = new Color(COLOR_MAX, COLOR_MAX, COLOR_MAX, currentAlpha);
            ParticleSystem.MainModule mainModule = this.particleSystem.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(minColor, maxColor);

            mainModule.maxParticles = Random.Range(PARTICLES_MIN, PARTICLES_MAX);
        }

        private void UpdatePosition()
        {
            if (IsMainMenu())
            {
                this.particleSystem.transform.position = new Vector3(-2500, 2000, -2000);
                this.particleSystem.transform.rotation = Quaternion.identity;
                return;
            }

            this.particleSystem.transform.position = new Vector3(Random.Range(POSITION_MIN, POSITION_MAX), HEIGHT, Random.Range(POSITION_MIN, POSITION_MAX));
            this.particleSystem.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
        }
    }
}