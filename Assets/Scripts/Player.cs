using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject[] weapons;
    private int weaponIndex = 0;

    [SerializeField]
    private Transform shootTransform;

    [SerializeField]
    private float shootInterval = 0.05f;
    private float lastShotTime = 0f;

    [SerializeField]
    private GameObject deathEffectPrefab; // 죽음 이펙트 프리팹

    [SerializeField]
    private AudioClip deathSound; // 죽음 사운드

    [SerializeField]
    private AudioClip coinSound; // 코인 획득 사운드

    private float minX = -2.35f;
    private float maxX = 2.35f;

    private AudioSource audioSource; // 사운드 재생기

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true; // 플레이어의 Collider를 트리거로 설정
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameState != GameManager.GameState.Playing || GameManager.instance.isNetworkErrorActive)
            return;

        HandleMovement();

        if (!GameManager.instance.isGameOver)
        {
            Shoot();
        }
    }

    private void HandleMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float clampedX = Mathf.Clamp(mousePos.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private void Shoot()
    {
        if (Time.time - lastShotTime > shootInterval)
        {
            Instantiate(weapons[weaponIndex], shootTransform.position, Quaternion.identity);
            lastShotTime = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);                
            }
        }        
    }


    private void PlayDeathEffect()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void PlayDeathSound()
    {
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }

    public void PlayCoinSound()
    {
        if (coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    public void Upgrade()
    {
        weaponIndex = Mathf.Min(weaponIndex + 1, weapons.Length - 1);
    }

    public void ResetWeapon()
    {
        weaponIndex = 0;
    }
}
