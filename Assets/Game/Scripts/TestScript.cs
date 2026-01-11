using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] float theta;
    [SerializeField] float gravity;
    [SerializeField] float velocity;

    [SerializeField] float x;
    [SerializeField] float y;

    Rigidbody2D rigid;

    private void Awake()
    {
        theta = theta * Mathf.Deg2Rad;
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        x = velocity * Mathf.Cos(theta) * time;
        y = velocity * Mathf.Sin(theta) * time - gravity * Mathf.Pow(time, 2) * 0.5f;

        Vector2 vector2 = new Vector2(x, y);

        rigid.MovePosition(vector2);

        time += Time.fixedDeltaTime;

    }

}
