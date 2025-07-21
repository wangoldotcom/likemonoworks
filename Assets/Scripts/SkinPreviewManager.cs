using UnityEngine;
using UnityEngine.UI;

public class SkinPreviewManager : MonoBehaviour
{
    [SerializeField] private PlayerSkinManager previewPlayerSkinManager;
    
    /// <summary>
    /// 스킨 정보를 받아 미리보기 객체에 적용합니다.
    /// </summary>
    public void UpdatePreview(SkinData skinData)
    {
        if (previewPlayerSkinManager != null && skinData != null)
        {
            previewPlayerSkinManager.ChangeSkin(skinData);
        }
        else
        {
            Debug.LogWarning("미리보기 객체나 스킨 데이터가 없습니다.");
        }
    }
}
