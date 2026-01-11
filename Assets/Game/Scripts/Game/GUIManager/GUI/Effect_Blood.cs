using UnityEngine;

namespace Eclipse.Game
{
    public class Effect_Blood : MonoBehaviour
    {
        ParticleSystem bloodParticle;

        private void Awake()
        {
            bloodParticle = GetComponent<ParticleSystem>();
            transform.position = new Vector3(0, -0.1f, 0);
        }

        public void PlayParticle()
        {
            bloodParticle.Play();
        }
    }
}