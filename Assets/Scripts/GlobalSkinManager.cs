using UnityEngine;

public class GlobalSkinManager : MonoBehaviour
{
    public static GlobalSkinManager Instance { get; private set; }

    // 선택된 스킨 데이터를 저장
    public SkinData SelectedSkinData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


