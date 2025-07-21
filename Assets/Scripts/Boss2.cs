using UnityEngine;
using System.Collections;

public class Boss2 : Boss
{
    [SerializeField]
    private float boss2Health = 100f;

    protected override void Start()
    {
        health = boss2Health;
        base.Start();        
    }

    protected override void SpecialAttack()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("bulletPrefab or firePoint is not set.");
            return;
        }

        int numberOfBullets = 8;
        float angleStep = 360f / numberOfBullets;
        float angle = 0f;

        for (int i = 0; i < numberOfBullets; i++)
        {
            float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
            float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 bulletMoveVector = new Vector3(dirX, dirY, 0);
            Vector3 bulletDir = bulletMoveVector.normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.SetDirection(bulletDir);
            }

            angle += angleStep;
        }
    }
    public override void Die()
    {
        Debug.Log("Boss2 Die method called");
        base.Die(); // base.Die()가 GameManager의 BossDefeated를 호출합니다.
        Debug.Log("Boss2 Die method finished");
    }  
}
