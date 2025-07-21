using UnityEngine;

[CreateAssetMenu(fileName = "NewSkinData", menuName = "Skin Data", order = 51)]
public class SkinData : ScriptableObject
{
    public string skinName;                   // 스킨의 이름
    public Sprite skinSprite;                 // 스킨 미리보기 이미지
    public int price;                         // 스킨 가격 (코스트)
    public RuntimeAnimatorController animatorController; // 스킨 애니메이터 컨트롤러
}
