using UnityEngine;

public class State_Move : IState
{
    private BaseHero hero;

    public State_Move(BaseHero hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {
        Debug.Log("Entered Move State");
    }

    public void Execute()
    {
        // �̵� �߿��� �Է� ������ �������� üũ
        Vector2 inputDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) inputDir += Vector2.up;
        if (Input.GetKey(KeyCode.S)) inputDir += Vector2.down;
        if (Input.GetKey(KeyCode.A)) inputDir += Vector2.left;
        if (Input.GetKey(KeyCode.D)) inputDir += Vector2.right;

        if (inputDir != Vector2.zero)
        {
            // ������ �ٲ���ٸ� Normalize �� ������ ����
            inputDir.Normalize();

            hero.Move(inputDir);
        }
        else
        {
            // �̵� Ű���� ���� �� ���� �� ���� ó�� �� Idle�� ��ȯ
            hero.Move(Vector2.zero);

            hero.stateMachine.ChangeState(new State_Idle(hero));
            return;
        }

        //hero.InputMove();

        //// �̵� �Է��� ������ Idle ���·� ��ȯ
        //if (hero.MoveDirection == Vector2.zero)
        //{
        //    hero.stateMachine.ChangeState(new State_Idle(hero));
        //}

        // ���� �Է� ó��
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }

    public void PhysicsExecute()
    {
        hero.MoveCharacter();
    }

    public void Exit()
    {
        Debug.Log("Exited Move State");
    }
}
