using UnityEngine;

public class State_Idle : IState
{
    private readonly BaseHero hero;

    public State_Idle(BaseHero hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {
        Debug.Log("서버: Idle 상태 진입");

        hero.rb.linearVelocity = Vector2.zero;
    }

    public void PhysicsExecute()
    {
        // 서버는 클라이언트가 보낸 최신 입력 데이터를 확인합니다.
        var input = hero.LastReceivedInput;

        // 1.공격 입력이 있었는가?
        if (input.IsAttackPressed)
        {
            hero.stateMachine.ChangeState(new State_Attack(hero));
            return;
        }

        // 2. 이동 입력이 있었는가?
        if (input.MoveInput != Vector2.zero)
        {
            hero.stateMachine.ChangeState(new State_Move(hero));
            return;
        }
    }

    public void Exit()
    {
        Debug.Log("서버: Idle 상태 탈출");
    }
}