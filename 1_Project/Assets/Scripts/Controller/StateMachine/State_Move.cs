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
        Debug.Log("����: Move ���� ����");
    }

    public void PhysicsExecute()
    {
        // �̵� ���� ����
        hero.HandleMovement();

        var input = hero.LastReceivedInput;

        // ���� �Է��� ������ ���� ���·� ��ȯ
        if (input.IsAttackPressed)
        {
            hero.stateMachine.ChangeState(new State_Attack(hero));
            return;
        }

        // �̵� �Է��� ���߸� ��� ���·� ��ȯ
        if (input.MoveInput == Vector2.zero)
        {
            hero.stateMachine.ChangeState(new State_Idle(hero));
            return;
        }
    }

    public void Exit()
    {
        Debug.Log("����: Move ���� Ż��");
    }
}