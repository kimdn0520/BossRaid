using UnityEngine;

public class State_Move : IState
{
    private BaseHero hero;

    public State_Move(BaseHero hero)
    {
        this.hero = hero;
    }

    public override void Enter()
    {
        Debug.Log("Entered Move State");
    }

    public override void Execute()
    {
        hero.InputMove();

        // 이동 입력이 없으면 Idle 상태로 전환
        if (hero.MoveDirection == Vector2.zero)
        {
            hero.stateMachine.ChangeState(new State_Idle(hero));
        }

        // 공격 입력 처리
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }

    public override void PhysicsExecute()
    {
        hero.MoveCharacter();
    }

    public override void Exit()
    {
        Debug.Log("Exited Move State");
    }
}
