using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinListManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent; // 스크롤뷰의 Content 오브젝트
    [SerializeField] private GameObject skinButtonPrefab; // 스킨 버튼 프리팹
    [SerializeField] private List<SkinData> availableSkins; // 추가한 SkinData 에셋 리스트

    private void Start()
    {
        PopulateSkinList();
    }

    private void PopulateSkinList()
    {
        // 기존의 자식들 제거 (필요하다면)
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (SkinData skin in availableSkins)
        {
            GameObject newButton = Instantiate(skinButtonPrefab, contentParent);
            // 예를 들어, SkinButton 컴포넌트에서 스킨 데이터와 버튼 이벤트를 초기화
            SkinButton skinButton = newButton.GetComponent<SkinButton>();
            if (skinButton != null)
            {
                skinButton.Setup(skin);
            }
        }
    }
}
