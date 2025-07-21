using UnityEngine;
using System.Collections;

public class Boss4 : Boss
{
    [SerializeField]
    private float boss4Health = 200f; // Boss4의 체력을 개별적으로 설정

    protected override void Start()
    {
        health = boss4Health; // Boss4의 체력을 설정
        base.Start();
        // Boss4의 추가적인 초기화 코드
    }

    protected override void SpecialAttack()
    {
        // 연속적인 탄환 발사
        StartCoroutine(ContinuousShooting());
    }

    IEnumerator ContinuousShooting()
    {
        for (int i = 0; i < 10; i++)
        {
            ShootBullet();
            yield return new WaitForSeconds(0.5f);
        }
    }
    public override void Die()
    {
        Debug.Log("Boss4 Die method called");
        base.Die(); // base.Die()가 GameManager의 BossDefeated를 호출합니다.
        Debug.Log("Boss4 Die method finished");
    }
}
