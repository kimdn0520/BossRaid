using UnityEngine;

public class State_Idle : IState
{
    private BaseHero hero;

    public State_Idle(BaseHero hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {
        Debug.Log("Entered Idle State");
    }

    public void Execute()
    {
        // 1. �̵� �Է�(WASD) üũ
        Vector2 inputDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) inputDir += Vector2.up;
        if (Input.GetKey(KeyCode.S)) inputDir += Vector2.down;
        if (Input.GetKey(KeyCode.A)) inputDir += Vector2.left;
        if (Input.GetKey(KeyCode.D)) inputDir += Vector2.right;

        if (inputDir != Vector2.zero)
        {
            inputDir.Normalize();

            // ������ �̵� ��û
            hero.Move(inputDir);

            // �ٷ� Move ���·� ��ȯ
            hero.stateMachine.ChangeState(new State_Move(hero));
            return;
        }

        // 2. ���� �Է� ó��
        if (Input.GetMouseButtonDown(0))
        {
            // hero.stateMachine.ChangeState(new State_Attack(hero));
        }

        // 3) ��ų �Է� ó�� (��: Q Ű)
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    hero.stateMachine.ChangeState(new State_Skill(hero, 0));
        //    return;
        //}
    }

    public void PhysicsExecute()
    {
        hero.MoveCharacter();
    }

    public void Exit()
    {
        Debug.Log("Exited Idle State");
    }
}