using UnityEngine;

public class State_Idle : IState
{
    private BaseHero hero;

    public State_Idle(BaseHero hero)
    {
        this.hero = hero;
    }

    public override void Enter()
    {
        Debug.Log("Entered Idle State");
    }

    public override void Execute()
    {
        hero.InputMove();

        // 이동 입력이 있으면 이동 상태로 전환
        if (hero.MoveDirection != Vector2.zero)
        {
            hero.stateMachine.ChangeState(new State_Move(hero));
        }

        // 공격 입력 처리
        if (Input.GetMouseButtonDown(0))
        {
            // hero.stateMachine.ChangeState(new State_Attack(hero));
        }
    }

    public override void PhysicsExecute()
    {
        hero.MoveCharacter();
    }

    public override void Exit()
    {
        Debug.Log("Exited Idle State");
    }
}