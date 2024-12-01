using UnityEngine;

[CreateAssetMenu(fileName = "Hero Data", menuName = "Scriptable Object/Hero Data", order = int.MaxValue)]
public class HeroData : ScriptableObject
{
    [System.Serializable]
    public class Hero
    {
        public HeroType heroType;
        public int hp;
        public int mp;
        public int defense;
        public int shield;
        public float moveSpeed;
    }

    [SerializeField]
    private Hero[] heroes;  
    public Hero[] Heroes { get { return heroes; } }

    // 배열을 통해 히어로 정보에 접근
    public Hero GetHero(int index)
    {
        if (index >= 0 && index < heroes.Length)
            return heroes[index];
        else
            return null;
    }
}
