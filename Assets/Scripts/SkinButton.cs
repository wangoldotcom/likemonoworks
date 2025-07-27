using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;   // 미리보기 이미지
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI skinNameText;
    [SerializeField] private Button button;

    private SkinData skinData;

    // 원하는 “흐릿한” 색 (채도↓·투명도↓) – 필요에 따라 조절
    private readonly Color lockedColor   = new(1f, 1f, 1f, 0.35f);
    private readonly Color unlockedColor = Color.white;

    public void Setup(SkinData data)
    {
        skinData = data;
        iconImage.sprite   = data.skinSprite;
        priceText.text     = data.price.ToString();
        skinNameText.text  = data.skinName;

        RefreshVisual();              // 처음 상태 적용
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        FindObjectOfType<SkinShopManager>()?
            .PurchaseOrSelectSkin(skinData, this);   // ← this 추가
    }

    public void RefreshVisual()
    {
        if (this == null || iconImage == null) return;   // 파괴·누락 보호

        bool purchased = PlayerPrefs.GetInt($"Skin_{skinData.skinName}", 0) == 1;

        // 1) 아이콘 색
        iconImage.color = purchased ? unlockedColor : lockedColor;

        // 2) Button 루트(targetGraphic) 색도 동일하게
        if (button.targetGraphic != null)
            button.targetGraphic.color = purchased ? unlockedColor : lockedColor;

        // 3) 가격 표시는 미구매 상태에서만
        priceText?.gameObject.SetActive(!purchased);
    }
}
