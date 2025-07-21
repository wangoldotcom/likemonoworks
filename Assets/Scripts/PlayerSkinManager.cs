using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;

    private void Awake()
    {
        if (targetAnimator == null)
        {
            targetAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void OnEnable()
    {
        // 비활성 상태였다가 활성화될 때마다 스킨 업데이트
        UpdateSkinFromPrefs();
    }

    private void Start()
    {
        // 게임 시작 시 저장된 스킨 정보를 불러옵니다.
        string selectedSkinName = PlayerPrefs.GetString("SelectedSkin", "defaultSkin");

        // Resources/Skins 폴더 내의 모든 SkinData 에셋을 불러옵니다.
        SkinData[] allSkins = Resources.LoadAll<SkinData>("Skins");
        SkinData skinData = null;

        // 불러온 에셋 중에서 저장된 스킨 이름과 일치하는 것을 찾습니다.
        foreach (SkinData sd in allSkins)
        {
            // SkinData에 skinName 필드가 있어야 합니다. (또는 sd.name을 사용할 수 있음)
            if (sd.skinName == selectedSkinName)
            {
                skinData = sd;
                break;
            }
        }

        if (skinData != null)
        {
            ChangeSkin(skinData);
        }
        else
        {
            Debug.LogWarning("저장된 스킨 데이터를 찾을 수 없습니다: " + selectedSkinName);
        }
    }

    private void UpdateSkinFromPrefs()
    {
        // 저장된 스킨 이름 불러오기 (기본값은 "defaultSkin")
        string selectedSkinName = PlayerPrefs.GetString("SelectedSkin", "defaultSkin");

        // Resources 폴더를 통해 SkinData 불러오기 (SkinData 에셋들은 반드시 Assets/Resources/Skins 폴더에 있어야 함)
        SkinData[] allSkins = Resources.LoadAll<SkinData>("Skins");
        SkinData skinData = null;
        foreach (SkinData sd in allSkins)
        {
            if (sd.skinName == selectedSkinName)
            {
                skinData = sd;
                break;
            }
        }

        if (skinData != null)
        {
            ChangeSkin(skinData);
        }
        else
        {
            Debug.LogWarning("저장된 스킨 데이터를 찾을 수 없습니다: " + selectedSkinName);
        }
    }


    public void ChangeSkin(SkinData skinData)
    {
        if (skinData == null)
        {
            Debug.LogWarning("SkinData가 null입니다.");
            return;
        }

        if (targetAnimator != null)
        {
            targetAnimator.runtimeAnimatorController = skinData.animatorController;
        }
        else
        {
            Debug.LogWarning("Animator 컴포넌트를 찾을 수 없습니다.");
        }
    }
}
