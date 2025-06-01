using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum HeroType
{
    Warrior,
    Priest,
    Ranger,
    Mage,
    Rogue
}

public abstract class BaseHero : NetworkBehaviour, ICharacter
{
    // 네트워크 변수로 HP, MP, 이동 방향 등을 동기화
    public NetworkVariable<float> Health { get; } = new NetworkVariable<float>(100f);
    public NetworkVariable<float> Mana { get; } = new NetworkVariable<float>(50f);
    public NetworkVariable<Vector2> MoveDirection { get; } = new NetworkVariable<Vector2>(Vector2.zero);

    // abstract
    public abstract HeroType HeroClass { get; }
    public abstract int Defense { get; set; }
    public abstract int Shield { get; set; }
    public abstract float MoveSpeed { get; set; }

    public abstract void Attack(Vector2 direction);
    public abstract void UseSkill(int skillIndex, Vector2 direction);
    public abstract void TakeDamage(int amount);
    public abstract void AddShield(int amount);
    public abstract UniTask DashAsync();

    // non-abstract
    public StateMachine stateMachine;
    public Vector3 mousePosition;                        
    public Vector2 attackDirection { get; private set; }

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Camera mainCamera;

    protected virtual void Awake()
    {
        stateMachine = new StateMachine();

        ChangeState(new State_Idle(this));

        mainCamera = Camera.main;
    }

    protected virtual void FixedUpdate()
    {
        if (!IsServer) return;

        stateMachine.FixedUpdate();
    }

    protected virtual void Update()
    {
        if (!IsOwner) return;

        stateMachine.Update();

        UpdateDirection();

        UpdateSpriteDirection();
    }

    public virtual void UpdateDirection()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        attackDirection = (mousePosition - transform.position).normalized;
    }

    public virtual void UpdateSpriteDirection()
    {
        float directionX = mousePosition.x - transform.position.x;

        if (directionX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (directionX < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    public virtual void MoveCharacter()
    {
        if (rb != null)
        {
            rb.linearVelocity = MoveDirection.Value * MoveSpeed;
        }
    }

    public void ChangeState(IState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public virtual void Move(Vector2 direction)
    {
        if (IsOwner)
        {
            SubmitMoveServerRpc(direction);
        }
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void UseSkill(int skillIndex)
    {
        throw new System.NotImplementedException();
    }

    public void OnAttackAnimationEvent()
    {
        throw new System.NotImplementedException();
    }

    public void OnSkillAnimationEvent(int skillIndex)
    {
        throw new System.NotImplementedException();
    }

    #region Netcode RPC

    [ServerRpc]
    private void SubmitMoveServerRpc(Vector2 direction, ServerRpcParams rpcParams = default)
    {
        MoveDirection.Value = direction;
    }

#endregion
}
