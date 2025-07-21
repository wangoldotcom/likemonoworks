using UnityEngine;
using System.Collections;

public class Boss3 : Boss
{
    [SerializeField]
    private float boss3Health = 150f; // Boss3의 체력을 개별적으로 설정

    protected override void Start()
    {
        health = boss3Health; // Boss3의 체력을 설정
        base.Start();
        // Boss3의 추가적인 초기화 코드
    }

    protected override void SpecialAttack()
    {
        // 플레이어를 향해 다수의 탄환 발사
        int numberOfBullets = 4;
        float spreadAngle = 60f; // 발사 각도 범위
        float angleStep = spreadAngle / (numberOfBullets - 1);
        float angle = -spreadAngle / 2;

        for (int i = 0; i < numberOfBullets; i++)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            direction = Quaternion.Euler(0, 0, angle) * direction;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.SetDirection(direction);
            }

            angle += angleStep;
        }
    }
    public override void Die()
    {
        Debug.Log("Boss3 Die method called");
        base.Die(); // base.Die()가 GameManager의 BossDefeated를 호출합니다.
        Debug.Log("Boss3 Die method finished");
    }      
}
