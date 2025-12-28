using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "CardGame/Enemy Data", order = 1)]
public class EnemyDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName = "Monster";

    [TextArea] public string description = "Basic Monster"; // 설명 (선택사항)

    [Header("Stats")]
    public float maxHp = 100f;
    public int baseDamage = 10;

    // 필요하다면 여기에 방어력, 속도, 드랍 아이템, 전용 스프라이트 등을 추가할 수 있습니다.
    // public Sprite icon; 
}