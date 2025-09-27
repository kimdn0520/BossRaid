using UnityEngine;

public interface ISkill
{
    float Cooldown { get; }      // 스킬 쿨타임
    bool IsReady { get; }        // 스킬을 지금 사용할 수 있는지 여부

    void Use(GameObject owner);  // 스킬 사용 함수 (owner: 스킬을 사용하는 캐릭터)
    void Tick();                 // 매 프레임 호출되어 쿨타임을 계산하는 함수
}