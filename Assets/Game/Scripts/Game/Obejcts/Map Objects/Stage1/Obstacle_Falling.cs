using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Obstacle_Falling : MonoBehaviour
    {
        Rigidbody2D rigid;
        int speed = 60;
        public float height;
        float cooltime;
        float cooltimer = 0f;
    
        // Start is called before the first frame update
        void Start()
        {
            
            rigid = GetComponent<Rigidbody2D>();
            StartCoroutine(ObjectDuration());
            cooltime = height / speed;
          
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            cooltimer += Time.fixedDeltaTime;
    
    
            if(cooltimer <= cooltime)
            {
                MoveToPoint();
                GetComponent<PolygonCollider2D>().enabled = false;
                GetComponentInChildren<BoxCollider2D>().enabled = false;
            }
            else
            {
                GetComponent<PolygonCollider2D>().enabled = true;
                GetComponentInChildren<BoxCollider2D>().enabled = true;
                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            }
    
            if (StageManager.instance.isStageClear)
                Destroy(gameObject);
    
        }
    
        private void MoveToPoint()
        {
    
            rigid.MovePosition(rigid.position + new Vector2(0,-1) * speed * Time.fixedDeltaTime);
    
    
        }
    
    
        IEnumerator ObjectDuration()
        {
            yield return new WaitForSeconds(10f);
            Destroy(gameObject);
        }
    }
}