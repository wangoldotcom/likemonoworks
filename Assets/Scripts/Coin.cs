using UnityEngine;

public class Coin : BaseItem
{
    protected override void OnCollected()
    {
        Debug.Log("Coin collected!");
        GameManager.instance.IncreaseCoin();

        // 플레이어 찾기 (씬에 유일한 Player가 있다고 가정)
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.PlayCoinSound(); // 플레이어 쪽 사운드 호출
        }
    }
}
