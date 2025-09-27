public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        player.Rb.linearVelocity = UnityEngine.Vector2.zero;

        player.Animator.SetBool(player.AnimationData.IsMoving, false);
    }

    public override void Update()
    {
        base.Update();

        // 이동 입력이 감지되면 Move 상태로 전환합니다.
        if (player.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(new PlayerMoveState(player, stateMachine));
        }
    }
}