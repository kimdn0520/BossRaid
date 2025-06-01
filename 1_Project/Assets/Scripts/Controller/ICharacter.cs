using Unity.Netcode;
using UnityEngine;

public interface ICharacter
{
    // 네트워크 상에서 동기화할 주요 속성
    NetworkVariable<float> Health { get; }
    NetworkVariable<float> Mana { get; }
    NetworkVariable<Vector2> MoveDirection { get; }

    // 공통 동작 (호출 시 상태머신에서 상태 전이 or 내부 로직 수행)
    void ChangeState(IState newState);
    void Move(Vector2 direction);
    void Attack();
    void UseSkill(int skillIndex);

    // 인터페이스를 통해 호출될 콜백 (ex: 애니메이션 이벤트)
    void OnAttackAnimationEvent();
    void OnSkillAnimationEvent(int skillIndex);
}
