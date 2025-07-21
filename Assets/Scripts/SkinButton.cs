using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinButton : MonoBehaviour
{
    [SerializeField] private Image skinImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI skinNameText;
    [SerializeField] private Button button;
    [SerializeField] private Image lockIcon;

    private SkinData skinData;

    public void Setup(SkinData data)
    {
        skinData = data;

        if (skinImage != null)
            skinImage.sprite = data.skinSprite;
        if (priceText != null)
            priceText.text = data.price.ToString();
        if (skinNameText != null)
            skinNameText.text = data.skinName;

        // 초기 잠금 아이콘 상태 설정
        UpdateLockIconState();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSkinButtonClicked);
    }

    private void OnSkinButtonClicked()
    {
        SkinShopManager shopManager = FindObjectOfType<SkinShopManager>();
        if (shopManager != null)
        {
            shopManager.PurchaseOrSelectSkin(skinData);
        }
        else
        {
            Debug.LogWarning("SkinShopManager를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 스킨 구매 여부에 따라 잠금 아이콘을 갱신합니다.
    /// </summary>
    public void UpdateLockIconState()
    {
        if (skinData == null || lockIcon == null)
            return;

        bool isPurchased = PlayerPrefs.GetInt("Skin_" + skinData.skinName, 0) == 1;
        lockIcon.gameObject.SetActive(!isPurchased);
        Canvas.ForceUpdateCanvases();
    }
    

    /// <summary>
    /// 잠금 아이콘을 즉시 비활성화하고 레이아웃을 재구성합니다.
    /// </summary>
    public void DisableLockIconImmediately()
    {
        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(lockIcon.transform.parent as RectTransform);
        }
    }
}
