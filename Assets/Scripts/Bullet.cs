using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f; // 보스 탄환의 데미지 값
    public bool isBossBullet = false; // 보스 탄환 여부

    private Vector3 direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // 체력 감소
            }
            Destroy(gameObject); // 총알 제거
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // 장애물과 충돌 시 총알 제거
        }
        else if (other.CompareTag("Boss"))
        {
            // 보스와 충돌 시 추가 로직
            Destroy(gameObject); // 총알 제거            
        }
    }
}
