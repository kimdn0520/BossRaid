using UnityEngine;

public class State_Attack : IState
{
    private readonly BaseHero hero;
    private float attackTimer;
    private const float ATTACK_DURATION = 0.5f; // 공격 상태 지속 시간

    public State_Attack(BaseHero hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {
        Debug.Log("서버: Attack 상태 진입");
        attackTimer = 0f;

        // 공격 방향으로 캐릭터 방향 고정
        hero.IsFlipped.Value = hero.LastReceivedInput.LookDirection.x < 0;

        // 실제 공격 실행
        hero.PerformAttack();
    }

    public void PhysicsExecute()
    {
        // 일정 시간 동안 공격 상태를 유지
        attackTimer += Time.fixedDeltaTime;
        if (attackTimer >= ATTACK_DURATION)
        {
            // 공격이 끝나면 다시 Idle 상태로 전환
            hero.stateMachine.ChangeState(new State_Idle(hero));
        }
    }

    public void Exit()
    {
        Debug.Log("서버: Attack 상태 탈출");
    }
}