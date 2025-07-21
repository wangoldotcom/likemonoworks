using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinShopManager : MonoBehaviour
{
    [Header("동적 UI 생성 관련")]
    [SerializeField] private Transform contentParent; // 스크롤뷰 Content 오브젝트
    [SerializeField] private GameObject skinButtonPrefab; // 스킨 버튼 프리팹

    [Header("Skin Data 목록")]
    [SerializeField] private List<SkinData> availableSkins; // Inspector에서 할당할 SkinData 에셋 목록

    [Header("Candy 표시 UI")]
    [SerializeField] private TextMeshProUGUI candyDisplayText;

    [Header("참조")]
    [SerializeField] private PlayerSkinManager playerSkinManager;    // 실제 플레이어 스킨 적용용
    [SerializeField] private SkinPreviewManager skinPreviewManager;    // 스킨 미리보기용

    [Header("Purchase Confirmation Popup")]
    [SerializeField] private GameObject purchaseConfirmationPopup; // 구매 확인 팝업 패널
    [SerializeField] private TextMeshProUGUI confirmationText;       // 팝업 내 메시지 텍스트
    [SerializeField] private Button yesButton;                       // Yes 버튼
    [SerializeField] private Button noButton;                        // No 버튼

    private Dictionary<string, SkinButton> skinButtonDict = new Dictionary<string, SkinButton>();

    private void Start()
    {
        PopulateSkinList();
        UpdateCandyDisplay();
    }

    // 동적으로 스킨 버튼을 생성합니다.
    private void PopulateSkinList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        skinButtonDict.Clear();
        foreach (SkinData skin in availableSkins)
        {
            GameObject newButton = Instantiate(skinButtonPrefab, contentParent);
            SkinButton skinButton = newButton.GetComponent<SkinButton>();
            if (skinButton != null)
            {
                skinButton.Setup(skin);
                if (!skinButtonDict.ContainsKey(skin.skinName))
                    skinButtonDict.Add(skin.skinName, skinButton);
            }
        }
    }

    private IEnumerator RefreshLockIcon(SkinButton button)
    {
        yield return new WaitForEndOfFrame();
        yield return null;
        button.UpdateLockIconState();
    }

    // Candy 수를 UI에 표시합니다.
    private void UpdateCandyDisplay()
    {
        if (candyDisplayText != null)
        {
            int totalCandies = PlayerPrefs.GetInt("TotalCandies", 0);
            Debug.Log("TotalCandies in PlayerPrefs: " + totalCandies);
            candyDisplayText.text = totalCandies.ToString();
        }
    }

    public bool TrySpendCandy(int cost)
    {
        int totalCandies = PlayerPrefs.GetInt("TotalCandies", 0);
        if (totalCandies >= cost)
        {
            totalCandies -= cost;
            PlayerPrefs.SetInt("TotalCandies", totalCandies);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    // 스킨 구매 또는 선택을 처리합니다.
    public void PurchaseOrSelectSkin(SkinData skinData)
    {
        if (skinData == null)
        {
            Debug.LogWarning("SkinData가 null입니다.");
            return;
        }

        bool isPurchased = PlayerPrefs.GetInt("Skin_" + skinData.skinName, 0) == 1;

        if (isPurchased)
        {
            // 이미 구매된 경우, 선택한 스킨을 저장하고 미리보기 업데이트
            PlayerPrefs.SetString("SelectedSkin", skinData.skinName);
            PlayerPrefs.Save();

            if (skinPreviewManager != null)
            {
                skinPreviewManager.UpdatePreview(skinData);
            }
            else
            {
                Debug.LogWarning("미리보기 관리자(SkinPreviewManager)가 연결되어 있지 않습니다.");
            }
        }
        else
        {
            // 아직 구매되지 않은 경우, 구매 확인 팝업을 띄웁니다.
            ShowPurchaseConfirmationPopup(skinData);
        }

        if (skinButtonDict.TryGetValue(skinData.skinName, out SkinButton targetButton))
        {
            targetButton.DisableLockIconImmediately();
            // 그리고 이후 RefreshLockIcon 코루틴 호출 (옵션)
            StartCoroutine(RefreshLockIcon(targetButton));
        }
    }

    private void ShowPurchaseConfirmationPopup(SkinData skinData)
    {
        if (purchaseConfirmationPopup != null)
        {
            purchaseConfirmationPopup.SetActive(true);
            if (confirmationText != null)
            {
                confirmationText.text = $"Would you like to purchase {skinData.skinName} for {skinData.price} candy?";
            }
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();

            yesButton.onClick.AddListener(() =>
            {
                if (TrySpendCandy(skinData.price))
                {
                    // 구매 처리: 구매 상태 저장
                    PlayerPrefs.SetInt("Skin_" + skinData.skinName, 1);
                    PlayerPrefs.SetString("SelectedSkin", skinData.skinName);
                    PlayerPrefs.Save();

                    UpdateCandyDisplay();

                    if (skinPreviewManager != null)
                    {
                        skinPreviewManager.UpdatePreview(skinData);
                    }
                    else
                    {
                        Debug.LogWarning("미리보기 관리자(SkinPreviewManager)가 연결되어 있지 않습니다.");
                    }
                }
                else
                {
                    Debug.Log("Not enough candy");
                }
                purchaseConfirmationPopup.SetActive(false);
            });

            noButton.onClick.AddListener(() =>
            {
                purchaseConfirmationPopup.SetActive(false);
            });
        }
        else
        {
            Debug.LogWarning("Purchase confirmation popup is not assigned.");
        }
    }

    private void ApplySkin(SkinData skinData)
    {
        if (skinData == null)
        {
            Debug.LogWarning("skinData가 할당되어 있지 않습니다.");
            return;
        }

        if (playerSkinManager != null)
        {
            playerSkinManager.ChangeSkin(skinData);
        }
        else
        {
            Debug.LogWarning("PlayerSkinManager를 찾을 수 없습니다. 선택된 스킨 정보만 저장합니다.");
        }
    }
}
