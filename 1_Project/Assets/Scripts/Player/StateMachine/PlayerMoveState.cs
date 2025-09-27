using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        player.Animator.SetBool(player.AnimationData.IsMoving, true);
    }

    public override void Update()
    {
        base.Update();

        // 이동 입력이 없어지면 Idle 상태로 전환합니다.
        if (player.MoveInput.magnitude < 0.1f)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.Rb.linearVelocity = player.MoveInput.normalized * player.MoveSpeed;
    }

    public override void Exit()
    {
        base.Exit();

        // Move 상태를 빠져나갈 때, 관성이 남지 않도록 속도를 0으로 만들어줍니다.
        player.Rb.linearVelocity = Vector2.zero;
    }
}