using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;      // LayoutRebuilder, Button
using TMPro;

public class SkinShopManager : MonoBehaviour
{
    /*─────────── Inspector ───────────*/
    [Header("동적 UI 생성")]
    [SerializeField] private Transform contentParent;      // Scroll-View Content
    [SerializeField] private GameObject skinButtonPrefab;   // 버튼 프리팹

    [Header("데이터")]
    [SerializeField] private List<SkinData> availableSkins;

    [Header("Candy UI")]
    [SerializeField] private TextMeshProUGUI candyDisplayText;

    [Header("참조")]
    [SerializeField] private PlayerSkinManager playerSkinManager;
    [SerializeField] private SkinPreviewManager skinPreviewManager;

    [Header("구매 확인 팝업")]
    [SerializeField] private GameObject purchaseConfirmationPopup;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    /*──────────────────────────────────*/

    private readonly Dictionary<string, SkinButton> skinButtonDict = new();

    /*─────────── Life-cycle ───────────*/
    private void OnEnable()
    {
        if (skinButtonDict.Count == 0) PopulateSkinList();  // 최초 1회만
        UpdateCandyDisplay();
    }

    #region 동적 버튼 생성
    private void PopulateSkinList()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);
        skinButtonDict.Clear();

        foreach (SkinData skin in availableSkins)
        {
            GameObject obj = Instantiate(skinButtonPrefab, contentParent);
            SkinButton btn = obj.GetComponent<SkinButton>();
            if (btn == null) continue;

            btn.Setup(skin);
            skinButtonDict.TryAdd(skin.skinName, btn);
        }
        StartCoroutine(RebuildLayoutNextFrame());
    }

    private IEnumerator RebuildLayoutNextFrame()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            contentParent.GetComponent<RectTransform>());
    }
    #endregion

    /*──────────── Candy UI ────────────*/
    private void UpdateCandyDisplay()
    {
        if (!candyDisplayText) return;
        int total = PlayerPrefs.GetInt("TotalCandies", 0);
        candyDisplayText.text = total.ToString();
    }

    public bool TrySpendCandy(int cost)
    {
        int total = PlayerPrefs.GetInt("TotalCandies", 0);
        if (total < cost) return false;

        PlayerPrefs.SetInt("TotalCandies", total - cost);
        PlayerPrefs.Save();
        return true;
    }
    /*──────────────────────────────────*/

    #region 구매·선택 처리
    // 외부에서 버튼 참조 없이 호출할 때
    public void PurchaseOrSelectSkin(SkinData skin) =>
        PurchaseOrSelectSkin(skin, null);

    // 버튼 자신을 함께 넘겨 주는 오버로드
    public void PurchaseOrSelectSkin(SkinData skin, SkinButton callerBtn)
    {
        if (!skin) return;

        bool purchased = PlayerPrefs.GetInt($"Skin_{skin.skinName}", 0) == 1;
        if (purchased) SelectSkin(skin, callerBtn);
        else ShowPurchaseConfirmationPopup(skin, callerBtn);
    }

    private void SelectSkin(SkinData skin, SkinButton btn = null)
    {
        PlayerPrefs.SetString("SelectedSkin", skin.skinName);
        PlayerPrefs.Save();

        skinPreviewManager?.UpdatePreview(skin);

        // 즉시 시각 갱신
        if (btn != null) btn.RefreshVisual();
        else if (skinButtonDict.TryGetValue(skin.skinName, out var b))
            b.RefreshVisual();
    }

    private void ShowPurchaseConfirmationPopup(SkinData skin, SkinButton callerBtn)
    {
        if (!purchaseConfirmationPopup) return;

        purchaseConfirmationPopup.SetActive(true);
        confirmationText.text =
            $"Buy <color=yellow>{skin.skinName}</color> for {skin.price} Candy ?";

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            if (TrySpendCandy(skin.price))
            {
                // █ 구매 성공 █
                PlayerPrefs.SetInt($"Skin_{skin.skinName}", 1);
                PlayerPrefs.Save();
                UpdateCandyDisplay();
                SelectSkin(skin, callerBtn);        // 색·락 바로 갱신
                purchaseConfirmationPopup.SetActive(false);
            }
            else
            {
                // █ 잔액 부족 █
                confirmationText.text = "<color=red>Not enough candy!</color>";
                StartCoroutine(ClosePopupAfterDelay(1f));
            }
        });

        noButton.onClick.AddListener(() =>
            purchaseConfirmationPopup.SetActive(false));
    }
    #endregion

    /*──────────── 유틸 ────────────*/
    private IEnumerator ClosePopupAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (purchaseConfirmationPopup != null &&
            purchaseConfirmationPopup.activeSelf)
            purchaseConfirmationPopup.SetActive(false);
    }
}
