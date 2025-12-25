using static Common;

public interface IOnCalculateDamage
{
    // 데미지 계산 시 끼어들어서 수치를 조작 (예: 하트 2개 이상 시 2배)
    void ModifyDamage(HandResult result, ref float finalDamage);
}
