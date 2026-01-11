using UnityEngine;

namespace Eclipse
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] protected Client client;

        public void SetClient(Client client)
        {
            this.client = client;
            this.client.AddManager(this);
        }

        public virtual void Preparing()
        {

        }

        public virtual void Startup()
        {

        }
    }
}