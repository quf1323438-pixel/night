using UnityEngine;

public class enemymove : MonoBehaviour
{
    public float movespeed;
    public float dir_ = 1; 
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(dir_ * movespeed, rb.linearVelocity.y);
    }
}
