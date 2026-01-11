using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Obstacle_Setting : MonoBehaviour
    {
        float damage;
        Animator animator;
    
        // Start is called before the first frame update
        void Start()
        {
            damage = 0.15f;
            animator = GetComponent<Animator>();
        }
    
        // Update is called once per frame
        void Update()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("NetherSummoning_Start"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    animator.SetTrigger("Idle");
                    StartCoroutine(ObjectDuration());
                }
    
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("NetherSummoning_End"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                 
                }
            }
    
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "Player")
            {
                IPlayer iPlayer = collision.GetComponent<IPlayer>();
    
                if (iPlayer == null)
                {
                    return;
                }
                
                iPlayer.TakeDamageConstantly(true, damage);
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                IPlayer iPlayer = collision.GetComponent<IPlayer>();

                if (iPlayer == null)
                {
                    return;
                }

                iPlayer.TakeDamageConstantly(true, damage);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                IPlayer iPlayer = collision.GetComponent<IPlayer>();
    
                if (iPlayer == null)
                {
                    return;
                }
    
                iPlayer.TakeDamageConstantly(false);
            }
        }
        IEnumerator ObjectDuration()
        {
            yield return new WaitForSeconds(4f);
            animator.SetTrigger("End");
        }
    }
}