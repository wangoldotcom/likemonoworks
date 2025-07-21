using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f; // 총알의 데미지 값
    public float lifeTime = 2f; // 총알이 사라지는 시간 (초)

    void Start()
    {
        Destroy(gameObject, lifeTime); // lifeTime 이후에 총알을 자동으로 파괴
    }

    void Update()
    {
        // Transform을 직접 변경하여 총알을 움직임
        transform.position += Vector3.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            if (other.CompareTag("Boss"))
            {
                Boss boss = other.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.TakeDamage(damage); // 보스에게 데미지를 입힘
                }
            }
            else
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); // 적에게 데미지를 입힘
                }
            }

            Destroy(gameObject); // 총알 제거
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // 장애물과 충돌 시 총알 제거
        }
    }
}
