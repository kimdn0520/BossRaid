using Unity.Netcode;
using UnityEngine;

public interface ICharacter
{
    // ��Ʈ��ũ �󿡼� ����ȭ�� �ֿ� �Ӽ�
    NetworkVariable<float> Health { get; }
    NetworkVariable<float> Mana { get; }
    NetworkVariable<Vector2> MoveDirection { get; }

    // ���� ���� (ȣ�� �� ���¸ӽſ��� ���� ���� or ���� ���� ����)
    void ChangeState(IState newState);
    void Move(Vector2 direction);
    void Attack();
    void UseSkill(int skillIndex);

    // �������̽��� ���� ȣ��� �ݹ� (ex: �ִϸ��̼� �̺�Ʈ)
    void OnAttackAnimationEvent();
    void OnSkillAnimationEvent(int skillIndex);
}
