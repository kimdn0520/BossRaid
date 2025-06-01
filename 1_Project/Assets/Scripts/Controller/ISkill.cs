using UnityEngine;

public interface ISkill
{
    string SkillName { get; }
    float Cooldown { get; }
    bool IsReady { get; }

    // ��ų �ߵ�
    void Execute();
}
