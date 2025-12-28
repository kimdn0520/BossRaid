using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class CardSoldier : EnemyBase
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

    // 공격 거리를 고정값 대신 '비율'이나 '최소 접근 거리'로 사용
    [SerializeField] private float stopDistance = 1.0f; // 목표지점 앞에서 얼마나 멈출지

    [Header("Juice Settings")]
    [SerializeField] private float windUpTime = 0.4f;
    [SerializeField] private float thrustTime = 0.1f;
    [SerializeField] private float recoveryTime = 0.3f;

    protected override void Start()
    {
        base.Start();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    // ★ 변경점: 목표 위치(targetPosition)를 인자로 받습니다.
    public override async UniTask Attack(Vector3 targetPosition, Action onHit)
    {
        transform.DOKill();
        modelTransform.DOKill();

        Vector3 originalPos = transform.position;

        // 고정 거리가 아니라, 목표 지점을 향해 계산합니다.
        // 목표 지점(플레이어)까지 가되, 너무 딱 붙지 않게 stopDistance만큼 덜 갑니다.
        // Vector3.Lerp를 써도 되고, 방향 벡터를 구해서 빼도 됩니다.
        Vector3 direction = (targetPosition - originalPos).normalized;
        Vector3 actualAttackPos = targetPosition - (direction * stopDistance);

        // =============================================================
        // Phase 1: Wind-up (기 모으기)
        // =============================================================
        animator.SetTrigger("DoAttack");

        // 반대 방향(뒤)으로 살짝 빠짐
        Vector3 windUpPos = originalPos - (direction * 0.5f);
        transform.DOMove(windUpPos, windUpTime).SetEase(Ease.OutQuad);
        modelTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), windUpTime);

        await UniTask.Delay(TimeSpan.FromSeconds(windUpTime));

        // =============================================================
        // Phase 2: Thrust (찌르기)
        // =============================================================

        // 계산된 실제 타격 위치로 이동
        transform.DOMove(actualAttackPos, thrustTime).SetEase(Ease.OutBack);

        modelTransform.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.1f).SetLoops(2, LoopType.Yoyo);

        if (Camera.main != null)
            Camera.main.transform.DOShakePosition(0.2f, 0.5f, 20, 90, false, true);

        await UniTask.Delay(TimeSpan.FromSeconds(thrustTime));

        // =============================================================
        // Phase 3: Hit & Recovery
        // =============================================================
        onHit?.Invoke();

        await UniTask.Delay(100);

        transform.DOMove(originalPos, recoveryTime).SetEase(Ease.InOutQuad);
        modelTransform.DOScale(Vector3.one, recoveryTime);

        await UniTask.Delay(TimeSpan.FromSeconds(recoveryTime));
    }
}