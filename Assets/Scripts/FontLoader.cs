using UnityEngine;
using TMPro;

public class FontLoader : MonoBehaviour
{
    private TMP_FontAsset fontAsset;

    void Start()
    {
        LoadFont();
        ApplyFontToAllText();
    }

    void LoadFont()
    {
        // 정확한 경로로 수정
        fontAsset = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
        if (fontAsset == null)
        {
            Debug.LogError("Failed to load font from Resources/Fonts/NanumGothic SDF.");
        }
        else
        {
            Debug.Log("Font loaded successfully.");
        }
    }

    void ApplyFontToAllText()
    {
        if (fontAsset == null) return;

        TextMeshProUGUI[] textComponents = FindObjectsOfType<TextMeshProUGUI>();
        foreach (var textComponent in textComponents)
        {
            textComponent.font = fontAsset;
        }
    }
}