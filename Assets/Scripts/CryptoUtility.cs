using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class CryptoUtility
{
    // AES 암호화 메서드
    public static string Encrypt(string text, string key)
    {
        // AES 인스턴스 생성
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16]; // 고정된 IV, 더 안전하게 하기 위해서는 랜덤 IV를 고려해야 합니다.

            // 암호화 수행
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(text);
                    }
                }
                // 암호화된 바이트 배열을 Base64 문자열로 변환
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    // AES 복호화 메서드 (옵션으로 추가)
    public static string Decrypt(string cipherText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16]; // IV는 암호화에 사용된 것과 동일해야 합니다.

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        // 복호화된 문자열 반환
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
