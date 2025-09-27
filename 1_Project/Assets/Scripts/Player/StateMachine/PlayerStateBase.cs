using UnityEngine;

public abstract class PlayerStateBase : IState
{
    protected PlayerController player;
    protected StateMachine stateMachine;

    public PlayerStateBase(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void FixedUpdate() { }

    public virtual void Update()
    {
        // 예시: 모든 상태에서 공통적으로 체력을 체크하는 로직
        // if (player.Stats.IsDead)
        // {
        //     stateMachine.ChangeState(new PlayerDeadState(player, stateMachine));
        // }
    }
}