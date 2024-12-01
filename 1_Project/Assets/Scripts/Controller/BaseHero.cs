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

public abstract class BaseHero : NetworkBehaviour
{
    // abstract
    public abstract HeroType HeroClass { get; }
    public abstract int Hp { get; set; }
    public abstract int Mp { get; set; }
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
    public Vector2 MoveDirection { get; private set; }      // 이동 방향
    public Vector3 mousePosition;                           // 마우스 위치
    public Vector2 attackDirection { get; private set; }    // 공격 방향

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // 메인 카메라
    private Camera mainCamera;

    protected virtual void Awake()
    {
        stateMachine = new StateMachine();

        stateMachine.ChangeState(new State_Idle(this));

        mainCamera = Camera.main;
    }

    protected virtual void FixedUpdate()
    {
        // 상태머신에서 물리 연산 처리
        stateMachine.FixedUpdate();
    }

    protected virtual void Update()
    {
        // 상태머신 업데이트
        stateMachine.Update();

        // Direction 계산
        UpdateDirection();

        // 스프라이트 방향 갱신
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
            spriteRenderer.flipX = false; // 오른쪽
        }
        else if (directionX < 0)
        {
            spriteRenderer.flipX = true;  // 왼쪽
        }
    }

    public virtual void InputMove()
    {
        MoveDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) MoveDirection += Vector2.up;
        if (Input.GetKey(KeyCode.S)) MoveDirection += Vector2.down;
        if (Input.GetKey(KeyCode.A)) MoveDirection += Vector2.left;
        if (Input.GetKey(KeyCode.D)) MoveDirection += Vector2.right;

        MoveDirection = MoveDirection.normalized;
    }

    public virtual void MoveCharacter()
    {
        if (rb != null)
        {
            rb.velocity = MoveDirection * MoveSpeed;
        }
    }
}
