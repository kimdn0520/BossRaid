using UnityEngine;

public class State_Attack : IState
{
    private readonly BaseHero hero;
    private float attackTimer;
    private const float ATTACK_DURATION = 0.5f; // ���� ���� ���� �ð�

    public State_Attack(BaseHero hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {
        Debug.Log("����: Attack ���� ����");
        attackTimer = 0f;

        // ���� �������� ĳ���� ���� ����
        hero.IsFlipped.Value = hero.LastReceivedInput.LookDirection.x < 0;

        // ���� ���� ����
        hero.PerformAttack();
    }

    public void PhysicsExecute()
    {
        // ���� �ð� ���� ���� ���¸� ����
        attackTimer += Time.fixedDeltaTime;
        if (attackTimer >= ATTACK_DURATION)
        {
            // ������ ������ �ٽ� Idle ���·� ��ȯ
            hero.stateMachine.ChangeState(new State_Idle(hero));
        }
    }

    public void Exit()
    {
        Debug.Log("����: Attack ���� Ż��");
    }
}