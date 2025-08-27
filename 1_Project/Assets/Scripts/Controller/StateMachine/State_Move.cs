using UnityEngine;

public class State_Move : IState
{
    private readonly BaseHero hero;

    public State_Move(BaseHero hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {
        Debug.Log("서버: Move 상태 진입");
    }

    public void PhysicsExecute()
    {
        // 이동 로직 실행
        hero.HandleMovement();

        var input = hero.LastReceivedInput;

        // 공격 입력이 들어오면 공격 상태로 전환
        if (input.IsAttackPressed)
        {
            hero.stateMachine.ChangeState(new State_Attack(hero));
            return;
        }

        // 이동 입력이 멈추면 대기 상태로 전환
        if (input.MoveInput == Vector2.zero)
        {
            hero.stateMachine.ChangeState(new State_Idle(hero));
            return;
        }
    }

    public void Exit()
    {
        Debug.Log("서버: Move 상태 탈출");
    }
}