using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;

public abstract class BaseHero : NetworkBehaviour, ICharacter
{
    // Network Variables
    public NetworkVariable<float> Health { get; } = new NetworkVariable<float>(100f);
    public NetworkVariable<bool> IsFlipped { get; } = new NetworkVariable<bool>(false);

    // Components & StateMachine
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public StateMachine stateMachine;

    // Server-side state
    public InputPayload LastReceivedInput { get; private set; }
    public abstract float MoveSpeed { get; set; }


    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
    }

    public override void OnNetworkSpawn()
    {
        // 방향 동기화 콜백 등록
        IsFlipped.OnValueChanged += (prev, next) => spriteRenderer.flipX = next;

        // 서버에서만 상태머신 초기화
        if (IsServer)
        {
            stateMachine.ChangeState(new State_Idle(this));
        }
    }

    // 클라이언트의 Update: 입력 수집 및 서버 전송 역할만 수행
    protected virtual void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        // 1. 입력 데이터를 수집하여 Payload 생성
        var payload = new InputPayload
        {
            MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized,
            LookDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized,
            IsAttackPressed = Input.GetMouseButtonDown(0),
            IsSkillPressed = Input.GetKeyDown(KeyCode.Q)
        };

        // 2. 즉각적인 시각적 피드백 (스프라이트 방향)
        spriteRenderer.flipX = payload.LookDirection.x < 0;

        // 3. 수집한 입력 데이터를 서버로 전송
        SubmitInputServerRpc(payload);
    }

    // 서버의 FixedUpdate: 상태 머신 실행 역할만 수행
    protected virtual void FixedUpdate()
    {
        if (!IsServer) return;
        stateMachine.FixedUpdate();
    }


    // 실제 이동 로직 (서버의 상태 클래스가 호출)
    public virtual void HandleMovement()
    {
        rb.linearVelocity = LastReceivedInput.MoveInput * MoveSpeed;
    }

    public virtual void HandleDirection()
    {
        IsFlipped.Value = LastReceivedInput.LookDirection.x < 0;
    }

    // 실제 공격 로직 (서버의 상태 클래스가 호출)
    public abstract void PerformAttack();


    #region RPCs

    [ServerRpc]
    private void SubmitInputServerRpc(InputPayload payload)
    {
        // 서버는 클라이언트로부터 받은 최신 입력 데이터를 저장합니다.
        LastReceivedInput = payload;

        IsFlipped.Value = payload.LookDirection.x < 0;
    }

    #endregion

    // ICharacter 인터페이스 구현 (지금 구조에서는 사용되지 않지만, 외부 시스템 연동을 위해 남겨둘 수 있음)
    public void Attack() { /* 의도적으로 비워둠. 입력은 Payload로 처리 */ }
    public void UseSkill(int skillIndex) { /* 의도적으로 비워둠 */ }
}