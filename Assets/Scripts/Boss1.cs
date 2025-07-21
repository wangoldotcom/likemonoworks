using UnityEngine;
using System.Collections;

public class Boss1 : Boss
{
    [SerializeField]
    private float boss1Health = 50f; // Boss1의 체력을 개별적으로 설정

    protected override void Start()
    {
        health = boss1Health; // Boss1의 체력을 설정
        base.Start();
        // Boss1의 추가적인 초기화 코드
    }

    protected override void SpecialAttack()
    {
        // 보스1의 특수 공격 패턴을 정의합니다.
        // 예시: 기본 한 발 발사
        ShootBullet();
    }
}
