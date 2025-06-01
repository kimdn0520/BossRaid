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
        // 이동 중에도 입력 변경이 가능한지 체크
        Vector2 inputDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) inputDir += Vector2.up;
        if (Input.GetKey(KeyCode.S)) inputDir += Vector2.down;
        if (Input.GetKey(KeyCode.A)) inputDir += Vector2.left;
        if (Input.GetKey(KeyCode.D)) inputDir += Vector2.right;

        if (inputDir != Vector2.zero)
        {
            // 방향이 바뀌었다면 Normalize 후 서버로 갱신
            inputDir.Normalize();

            hero.Move(inputDir);
        }
        else
        {
            // 이동 키에서 손을 뗀 순간 → 멈춤 처리 및 Idle로 전환
            hero.Move(Vector2.zero);

            hero.stateMachine.ChangeState(new State_Idle(hero));
            return;
        }

        //hero.InputMove();

        //// 이동 입력이 없으면 Idle 상태로 전환
        //if (hero.MoveDirection == Vector2.zero)
        //{
        //    hero.stateMachine.ChangeState(new State_Idle(hero));
        //}

        // 공격 입력 처리
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
