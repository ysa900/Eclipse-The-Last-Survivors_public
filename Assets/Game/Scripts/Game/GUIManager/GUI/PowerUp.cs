using UnityEngine;

namespace Eclipse.Game
{
    public class PowerUp : MonoBehaviour
    {
        ParticleSystem powerUpParticle;

        private void Awake()
        {
            powerUpParticle = GetComponent<ParticleSystem>();
            transform.position = new Vector3(0, -0.1f, 0);
            StopParticle();
        }

        public void PlayParticle()
        {
            powerUpParticle.Play();
        }

        public void StopParticle()
        {
            powerUpParticle.Stop();
        }
    }
}