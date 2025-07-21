using UnityEngine;
using UnityEngine.UI;

public class SkinPanelController : MonoBehaviour
{
    [Header("패널 연결")]
    [SerializeField] private GameObject skinPanel;  // 스킨 패널 (SkinShopManager가 붙어 있는 패널)
    [SerializeField] private GameObject mainPanel;  // 메인(스타트) 패널

    [Header("버튼 연결")]
    [SerializeField] private Button openSkinButton;   // 메인 패널에 있는 스킨 열기 버튼
    [SerializeField] private Button closeSkinButton;  // 스킨 패널 내 닫기 버튼

    private void Start()
    {
        if (openSkinButton != null)
            openSkinButton.onClick.AddListener(OpenSkinPanel);

        if (closeSkinButton != null)
            closeSkinButton.onClick.AddListener(CloseSkinPanel);
    }

    public void OpenSkinPanel()
    {
        // 스킨 패널을 활성화하고 메인 패널은 숨깁니다.
        if (skinPanel != null)
            skinPanel.SetActive(true);

        if (mainPanel != null)
            mainPanel.SetActive(false);
    }

    public void CloseSkinPanel()
    {
        // 스킨 패널을 닫고 메인 패널을 다시 활성화합니다.
        if (skinPanel != null)
            skinPanel.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }
}
