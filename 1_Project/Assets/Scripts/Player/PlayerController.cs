using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SkillManager))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform visualsTransform;

    public float MoveSpeed = 5f;

    public Animator Animator { get; private set; }
    public PlayerAnimationData AnimationData { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public SkillManager SkillManager { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public bool IsFacingRight { get; private set; } = true;
    
    private Camera mainCamera;

    private void Awake()
    {
        Animator = visualsTransform.GetComponent<Animator>();
        AnimationData = new PlayerAnimationData();
        AnimationData.Initialize();

        Rb = GetComponent<Rigidbody2D>();
        SkillManager = GetComponent<SkillManager>();
        StateMachine = new StateMachine();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        StateMachine.ChangeState(new PlayerIdleState(this, StateMachine));
    }

    private void Update()
    {
        HandleInput();

        CheckFlip();

        StateMachine.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        MoveInput = new Vector2(moveX, moveY).normalized; // 대각선 이동 속도 보정

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SkillManager.TryUseSkill(KeyCode.E);
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    SkillManager.TryUseSkill(KeyCode.R);
        //}
    }

    //private void CheckFlipByMouse()
    //{
    //    // 마우스의 스크린 좌표를 게임 월드 좌표로 변환합니다.
    //    Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

    //    // 마우스가 플레이어의 오른쪽에 있는지 왼쪽에 있는지 판단합니다.
    //    bool isMouseOnRight = mouseWorldPosition.x > transform.position.x;

    //    // 마우스가 오른쪽에 있는데 왼쪽을 보고 있을 경우
    //    // 또는 마우스가 왼쪽에 있는데 오른쪽을 보고 있을 경우에 Flip을 호출합니다.
    //    if ((isMouseOnRight && !IsFacingRight) || (!isMouseOnRight && IsFacingRight))
    //    {
    //        Flip();
    //    }
    //}

    private void CheckFlip()
    {
        if (Mathf.Abs(MoveInput.x) < 0.1f) return;

        bool shouldFlip = (MoveInput.x > 0 && !IsFacingRight) || (MoveInput.x < 0 && IsFacingRight);

        if (shouldFlip)
        {
            Flip();
        }
    }

    // 실제 캐릭터를 뒤집는 메소드입니다.
    private void Flip()
    {
        // 현재 보고 있는 방향을 반대로 바꿔줍니다.
        IsFacingRight = !IsFacingRight;

        // Visuals 자식 오브젝트의 Local Scale의 x값을 -1 곱하여 뒤집습니다.
        visualsTransform.localScale = new Vector3(visualsTransform.localScale.x * -1, 1f, 1f);
    }
}