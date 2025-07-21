using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab; // Coin 프리팹
    [SerializeField] private GameObject candyPrefab; // Candy 프리팹
    [SerializeField] private float candyDropChance = 0.3f; // Candy 드랍 확률 (30%)
    [SerializeField] private int baseCoinDropCount = 1; // 기본 코인 드랍 개수
    [SerializeField] private int baseCandyDropCount = 1; // 기본 캔디 드랍 개수

    [SerializeField]
    private GameObject hitEffectPrefab; // 피격 이펙트 프리팹

    [SerializeField]
    private float moveSpeed = 1.0f; // 기본 이동 속도

    private float minY = -7f;

    [SerializeField]
    private float baseHp = 1f; // 프리팹별로 설정 가능한 기본 체력
    private float currentHp;

    [SerializeField]
    private AudioClip englishSpawnSound; // 영어 등장 사운드
    [SerializeField]
    private AudioClip koreanSpawnSound; // 한국어 등장 사운드
    [SerializeField]
    private AudioClip japaneseSpawnSound; // 일본어 등장 사운드
    [SerializeField]
    private AudioClip chineseSpawnSound; // 중국어 등장 사운드

    [SerializeField]
    private AudioClip hitSound; // 히트 사운드

    private bool isDead = false; // 추가된 변수: 적이 이미 죽었는지 여부

    void Start()
    {
        // 현재 체력 설정
        currentHp = baseHp;

        PlaySpawnSound(); // 몬스터 등장 시 언어에 맞는 사운드 재생
    }

    void Update()
    {
        MoveDown();
        if (transform.position.y < minY)
        {
            Destroy(gameObject);
        }
    }

    private void MoveDown()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon != null)
            {
                TakeDamage(weapon.damage); // 무기의 데미지를 적용
                PlayHitSound(); // 히트 사운드 재생
            }

            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // 플레이어에게 1의 데미지 적용
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // 이미 죽었으면 추가 데미지를 받지 않음

        currentHp -= damage;

        // 데미지를 받을 때마다 피격 이펙트를 생성
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // 중복 처리 방지
        isDead = true;

        DropItems(); // Coin과 Candy 드랍 로직 실행

        Destroy(gameObject);
    }

    private void DropItems()
    {
        // Coin은 항상 드랍
        int coinCount = GameManager.instance.isHardMode ? baseCoinDropCount * 2 : baseCoinDropCount;
        for (int i = 0; i < coinCount; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // Candy는 30% 확률로 드랍
        float randomValue = Random.Range(0f, 1f);
        Debug.Log($"RandomValue for Candy Drop: {randomValue}");
        if (randomValue < candyDropChance)
        {
            Debug.Log("Candy dropped!");
            int candyCount = GameManager.instance.isHardMode ? baseCandyDropCount * 2 : baseCandyDropCount;
            for (int i = 0; i < candyCount; i++)
            {
                Instantiate(candyPrefab, transform.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("Candy did not drop.");
        }
    }

    // 히트 사운드 재생
    private void PlayHitSound()
    {
        if (hitSound != null)
        {
            // 새로운 오디오 소스를 생성하고 설정
            GameObject soundObject = new GameObject("HitSound");
            AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();
            tempAudioSource.clip = hitSound;
            tempAudioSource.volume = 0.8f; // 볼륨을 0.8으로 설정 (필요시 더 높이거나 낮출 수 있음)
            tempAudioSource.Play();

            // 사운드가 끝나면 오브젝트 제거
            Destroy(soundObject, hitSound.length);
        }
    }

    // 언어에 맞는 등장 사운드를 재생하는 메서드
    private void PlaySpawnSound()
    {
        int language = GameManager.instance.GetCurrentLanguage(); // 현재 선택된 언어를 가져옴
        AudioClip selectedClip = null;

        switch (language)
        {
            case 0: // 영어
                selectedClip = englishSpawnSound;
                break;
            case 1: // 한국어
                selectedClip = koreanSpawnSound;
                break;
            case 2: // 일본어
                selectedClip = japaneseSpawnSound;
                break;
            case 3: // 중국어
                selectedClip = chineseSpawnSound;
                break;
            default:
                selectedClip = englishSpawnSound; // 기본값 영어
                break;
        }

        if (selectedClip != null && SpawnSoundManager.Instance != null)
        {
            SpawnSoundManager.Instance.PlaySpawnSound(selectedClip); // SpawnSoundManager를 통해 스폰 사운드 재생
        }
    }
}
