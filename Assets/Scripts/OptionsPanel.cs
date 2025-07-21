using TMPro;
using UnityEngine;

public class OptionsPanel : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;  // TextMeshPro Dropdown 컴포넌트

    void Start()
    {
        int savedLanguageIndex = PlayerPrefs.GetInt("Language", 0);
        languageDropdown.value = savedLanguageIndex;

        languageDropdown.onValueChanged.AddListener(SetLanguage);
        SetLanguage(savedLanguageIndex);
    }

    public void SetLanguage(int languageIndex)
    {
        PlayerPrefs.SetInt("Language", languageIndex);
        PlayerPrefs.Save();
        GameManager.instance.UpdateLanguage(languageIndex);
    }
}
