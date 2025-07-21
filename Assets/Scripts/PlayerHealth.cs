using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private GameObject deathEffectPrefab; // 죽음 이펙트 프리팹

    public int maxHealth = 2;
    public int currentHealth;
    private bool isInvincible = false; // 무적 상태를 나타내는 변수 추가

    public event System.Action OnHealthChanged;

    // 최대 체력을 설정하는 메서드
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(); // 체력이 변경될 때 이벤트를 호출하여 UI 갱신
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(); // 체력이 변경될 때 이벤트를 호출하여 UI 갱신
        Debug.Log("Player's health reset to: " + currentHealth);
    }

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(); // 처음 시작할 때 체력 UI 갱신
    }

    public bool IsHealthIncreased(int amount)
    {
        return maxHealth >= (2 + amount);  // 기본 체력이 2일 때 추가된 체력과 비교
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return; // 무적 상태거나 이미 체력이 0이면 데미지 없음

        currentHealth -= damage;
        OnHealthChanged?.Invoke(); // 체력이 변경될 때 이벤트를 호출하여 UI 갱신
        Debug.Log("Player took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayDeathEffect();  // 죽음 효과 실행
            GameManager.instance.SetGameOver(); // 체력이 0 이하가 되면 게임오버 상태로 전환
            gameObject.SetActive(false); // 플레이어 오브젝트 비활성화
        }
        else
        {
            StartCoroutine(BlinkEffect());
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth; // 최대 체력이 증가하면 현재 체력도 최대치로 설정
        OnHealthChanged?.Invoke(); // UI 갱신
    }

    private IEnumerator BlinkEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // 무적 상태 시작
        yield return new WaitForSeconds(0.5f); // 0.5초 동안 무적
        isInvincible = false; // 무적 상태 종료
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
            StartCoroutine(InvincibilityCoroutine()); // 무적 상태만 유지
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject); // 총알 제거
        }
    }

    private void PlayDeathEffect()
    {
        // 죽음 이펙트 재생 코드 추가
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    }
}
