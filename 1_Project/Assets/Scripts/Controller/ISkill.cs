using UnityEngine;

public interface ISkill
{
    string SkillName { get; }
    float Cooldown { get; }
    bool IsReady { get; }

    // 스킬 발동
    void Execute();
}
