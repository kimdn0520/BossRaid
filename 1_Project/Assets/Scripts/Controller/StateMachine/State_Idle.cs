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
        Debug.Log("����: Idle ���� ����");

        hero.rb.linearVelocity = Vector2.zero;
    }

    public void PhysicsExecute()
    {
        // ������ Ŭ���̾�Ʈ�� ���� �ֽ� �Է� �����͸� Ȯ���մϴ�.
        var input = hero.LastReceivedInput;

        // 1.���� �Է��� �־��°�?
        if (input.IsAttackPressed)
        {
            hero.stateMachine.ChangeState(new State_Attack(hero));
            return;
        }

        // 2. �̵� �Է��� �־��°�?
        if (input.MoveInput != Vector2.zero)
        {
            hero.stateMachine.ChangeState(new State_Move(hero));
            return;
        }
    }

    public void Exit()
    {
        Debug.Log("����: Idle ���� Ż��");
    }
}