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

        // �̵� �Է��� ������ �̵� ���·� ��ȯ
        if (hero.MoveDirection != Vector2.zero)
        {
            hero.stateMachine.ChangeState(new State_Move(hero));
        }

        // ���� �Է� ó��
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