using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f; // 배경 이동 속도

    [SerializeField]
    private SpriteRenderer backgroundRenderer; // 배경 이미지의 스프라이트 렌더러

    [SerializeField]
    private Sprite[] backgroundSprites; // 배경 스프라이트 배열

    private const float resetPositionY = -5f; // 배경이 다시 위로 이동할 y 위치
    private const float resetOffsetY = 18f; // 배경이 위로 이동할 때의 y 오프셋

    private int spriteIndex = 0;

    void Start()
    {
        ResetSprite();
    }

    void Update()
    {
        MoveBackground();
    }

    private void MoveBackground()
    {
        // 배경을 아래로 이동
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        // 배경이 특정 위치에 도달하면 다시 위로 이동
        if (transform.position.y < resetPositionY)
        {
            transform.position += new Vector3(0, resetOffsetY, 0);
        }
    }

    // 배경 스프라이트를 변경하는 메서드
    public void ChangeBackgroundSprite()
    {
        // ① 스프라이트 교체
        spriteIndex = (spriteIndex + 1) % backgroundSprites.Length;
        backgroundRenderer.sprite = backgroundSprites[spriteIndex];

        // ② 스테이지 진행 (GameManager에 최대 4까지만)
        if (GameManager.instance != null)
        {
            int next = Mathf.Clamp(GameManager.instance.CurrentStage + 1, 1, 4);
            GameManager.instance.SetCurrentStage(next);   // ← 새 메서드로 값만 갱신
        }
    }

    // 배경 스프라이트를 초기화하는 메서드
    public void ResetSprite()
    {
        spriteIndex = 0;
        if (backgroundSprites.Length > 0)
        {
            backgroundRenderer.sprite = backgroundSprites[spriteIndex];
            Debug.Log("Background sprite reset successfully to index: 0");
        }
        else
        {
            Debug.LogError("Failed to reset background. No sprites available.");
        }
    }
}
