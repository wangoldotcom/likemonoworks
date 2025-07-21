using UnityEngine;

public static class EncryptedPlayerPrefs
{
    private static string encryptionKey = "*dragon3011";  // AES 암호화에 사용할 키

    // 암호화된 데이터 저장
    public static void SetString(string key, string value)
    {
        string encryptedValue = CryptoUtility.Encrypt(value, encryptionKey);
        PlayerPrefs.SetString(key, encryptedValue);
        PlayerPrefs.Save();
    }

    // 암호화된 데이터 불러오기
    public static string GetString(string key, string defaultValue = "")
    {
        string encryptedValue = PlayerPrefs.GetString(key, null);

        if (string.IsNullOrEmpty(encryptedValue))
        {
            return defaultValue;
        }

        return CryptoUtility.Decrypt(encryptedValue, encryptionKey);
    }

    // 암호화된 데이터 삭제
    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    // 저장된 모든 데이터 삭제
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    // 저장된 데이터를 디스크에 기록
    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
