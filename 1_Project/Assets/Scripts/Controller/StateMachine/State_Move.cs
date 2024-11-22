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

        // �̵� �Է��� ������ Idle ���·� ��ȯ
        if (hero.MoveDirection == Vector2.zero)
        {
            hero.stateMachine.ChangeState(new State_Idle(hero));
        }

        // ���� �Է� ó��
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
