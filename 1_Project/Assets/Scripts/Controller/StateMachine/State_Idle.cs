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
        // 1. 이동 입력(WASD) 체크
        Vector2 inputDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) inputDir += Vector2.up;
        if (Input.GetKey(KeyCode.S)) inputDir += Vector2.down;
        if (Input.GetKey(KeyCode.A)) inputDir += Vector2.left;
        if (Input.GetKey(KeyCode.D)) inputDir += Vector2.right;

        if (inputDir != Vector2.zero)
        {
            inputDir.Normalize();

            // 서버로 이동 요청
            hero.Move(inputDir);

            // 바로 Move 상태로 전환
            hero.stateMachine.ChangeState(new State_Move(hero));
            return;
        }

        // 2. 공격 입력 처리
        if (Input.GetMouseButtonDown(0))
        {
            // hero.stateMachine.ChangeState(new State_Attack(hero));
        }

        // 3) 스킬 입력 처리 (예: Q 키)
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