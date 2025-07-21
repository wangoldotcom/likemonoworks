using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected float minY = -7f;
    private bool isCollected = false; // 중복 충돌 방지 플래그
    [SerializeField] private float jumpForceMin = 4f;
    [SerializeField] private float jumpForceMax = 10f;
    [SerializeField] private float bounceReductionFactor = 0.5f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        Jump();
    }

    protected void Jump()
    {
        float randomJumpForce = Random.Range(jumpForceMin, jumpForceMax);
        Vector2 jumpVelocity = Vector2.up * randomJumpForce;
        jumpVelocity.x = Random.Range(-3f, 3f);
        rb.AddForce(jumpVelocity, ForceMode2D.Impulse);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 bounceDirection = Vector2.Reflect(rb.velocity, normal).normalized;
            rb.velocity = bounceDirection * (rb.velocity.magnitude * bounceReductionFactor);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            OnCollected();
            Destroy(gameObject);
        }
    }

    protected abstract void OnCollected();

    protected virtual void Update()
    {
        if (transform.position.y < minY)
        {
            Destroy(gameObject);
        }
    }
}
