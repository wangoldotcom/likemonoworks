using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float speed = 2f;
    public float shootInterval = 3f;
    public int bulletsToShoot = 1;

    [SerializeField]
    protected float health = 100f;

    private bool shouldChase = true;
    private bool isDead = false;

    // 각 언어별 스폰 사운드
    [SerializeField] private AudioClip englishSpawnSound;
    [SerializeField] private AudioClip koreanSpawnSound;
    [SerializeField] private AudioClip japaneseSpawnSound;
    [SerializeField] private AudioClip chineseSpawnSound;

    // 각 언어별 데스 사운드
    [SerializeField] private AudioClip englishDeathSound;
    [SerializeField] private AudioClip koreanDeathSound;
    [SerializeField] private AudioClip japaneseDeathSound;
    [SerializeField] private AudioClip chineseDeathSound;

    [SerializeField]
    private GameObject hitEffectPrefab;

    [SerializeField]
    private AudioClip hitSound; // 히트 사운드

    private AudioSource audioSource;

    protected virtual void Start()
    {
        shouldChase = true;
        isDead = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        PlaySpawnSound(); // 보스 등장 시 사운드 재생

        StartCoroutine(ShootBullets());
        StartCoroutine(SpecialAttackPattern());
    }

    void Update()
    {
        if (shouldChase && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    IEnumerator ShootBullets()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(shootInterval);
            for (int i = 0; i < bulletsToShoot; i++)
            {
                ShootBullet();
            }
        }
    }

    protected void ShootBullet()
    {
        if (firePoint != null && bulletPrefab != null)
        {
            float randomAngle = Random.Range(-60f, 60f);
            Vector3 direction = Quaternion.Euler(0, 0, randomAngle) * -firePoint.up;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.SetDirection(direction);
                bulletComponent.isBossBullet = true;
                Debug.Log("Bullet Fired: " + bullet);
            }
            else
            {
                Debug.LogWarning("Bullet component is missing on the prefab.");
            }
        }
        else
        {
            if (firePoint == null) Debug.LogWarning("FirePoint is not set");
            if (bulletPrefab == null) Debug.LogWarning("BulletPrefab is not set");
        }
    }

    IEnumerator SpecialAttackPattern()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(10f);
            SpecialAttack();
        }
    }

    protected virtual void SpecialAttack()
    {
        // 각 보스마다 고유한 특수 공격 패턴을 정의
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon != null)
            {
                TakeDamage(weapon.damage); // 무기의 데미지를 적용
                PlayHitSound(); // 보스가 맞았을 때 히트 사운드 재생
            }
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log($"Boss took damage: {damage}, remaining health: {health}");

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Hit effect prefab is not set.");
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        PlayDeathSound(); // 보스가 죽을 때 사운드 재생

        // 코인 드롭
        DropCoins(10);

        if (GameManager.instance != null)
        {
            GameManager.instance.BossDefeated();
        }

        Destroy(gameObject); // 보스 오브젝트 파괴
    }    

    private void DropCoins(int count)
    {
        if (GameManager.instance != null && GameManager.instance.coinPrefab != null)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(GameManager.instance.coinPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    // 언어에 맞는 등장 사운드를 재생하는 메서드
    private void PlaySpawnSound()
    {
        int language = GameManager.instance.GetCurrentLanguage(); // 현재 선택된 언어를 가져옴

        AudioClip spawnSound = null;
        switch (language)
        {
            case 0: // 영어
                spawnSound = englishSpawnSound;
                break;
            case 1: // 한국어
                spawnSound = koreanSpawnSound;
                break;
            case 2: // 일본어
                spawnSound = japaneseSpawnSound;
                break;
            case 3: // 중국어
                spawnSound = chineseSpawnSound;
                break;
            default:
                spawnSound = englishSpawnSound; // 기본값 영어
                break;
        }

        if (spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }

    // 히트 사운드 재생
    private void PlayHitSound()
    {
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    // 언어에 맞는 죽음 사운드를 재생하는 메서드
    private void PlayDeathSound()
    {
        int language = GameManager.instance.GetCurrentLanguage(); // 현재 선택된 언어를 가져옴

        AudioClip deathSound = null;
        switch (language)
        {
            case 0: // 영어
                deathSound = englishDeathSound;
                break;
            case 1: // 한국어
                deathSound = koreanDeathSound;
                break;
            case 2: // 일본어
                deathSound = japaneseDeathSound;
                break;
            case 3: // 중국어
                deathSound = chineseDeathSound;
                break;
            default:
                deathSound = englishDeathSound; // 기본값 영어
                break;
        }

        if (deathSound != null)
        {
            // 새로운 오브젝트 생성
            GameObject soundObject = new GameObject("DeathSound");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();

            // 오디오 소스 설정
            audioSource.clip = deathSound;
            audioSource.playOnAwake = false;
            audioSource.Play();

            // 사운드가 끝나면 오브젝트 제거
            Destroy(soundObject, deathSound.length);
        }
    }
}
