using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;
using System;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Data Source")]
    [SerializeField] protected EnemyDataSO enemyData; // 스크립터블 오브젝트 참조

    [Header("Runtime State")]
    [SerializeField] protected float currentHp; // 변하는 값은 로컬 변수로 유지

    [Header("Hierarchy References")]
    [SerializeField] protected Transform modelTransform;
    [SerializeField] protected Transform damageSpawnPoint;

    [Header("UI References")]
    [SerializeField] protected Image hpBarFill;
    [SerializeField] protected TextMeshProUGUI hpText;

    public float MaxHp => enemyData != null ? enemyData.maxHp : 0;
    public int BaseDamage => enemyData != null ? enemyData.baseDamage : 0;
    public string EnemyName => enemyData != null ? enemyData.enemyName : "Unknown";

    protected virtual void Start()
    {
        // 데이터가 없으면 경고
        if (enemyData == null)
        {
            Debug.LogError($"[EnemyBase] {gameObject.name}에 EnemyDataSO가 연결되지 않았습니다!");
            return;
        }

        // 초기화: SO의 MaxHp를 가져와서 현재 체력으로 설정
        currentHp = MaxHp;

        UpdateUI();
    }

    // 공통 로직: 데미지 처리
    public virtual void TakeDamage(float amount)
    {
        if (IsDead()) return;

        currentHp -= amount;
        if (currentHp < 0) currentHp = 0;

        UpdateUI();

        PlayHitAnimation();

        if (IsDead())
        {
            Die();
        }
    }

    // 추상 메서드: 자식들이 반드시 구현해야 하는 '공격 행동'
    // BattleManager에서는 이 함수 호출.
    public abstract UniTask Attack(Vector3 targetPosition, Action onHit);

    // 공통 연출: 피격 (오버라이드 가능)
    protected virtual void PlayHitAnimation()
    {
        if (modelTransform == null) return;

        modelTransform.DOKill();
        modelTransform.DOShakePosition(0.5f, 0.5f, 20);

        SpriteRenderer sprite = modelTransform.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
        }
    }

    // 공통 로직: UI 갱신
    protected void UpdateUI()
    {
        if (hpBarFill != null && MaxHp > 0)
            hpBarFill.fillAmount = currentHp / MaxHp;

        if (hpText != null)
            hpText.text = $"{currentHp}/{MaxHp}";
    }

    // 공통 로직: 사망
    protected virtual void Die()
    {
        Debug.Log($"[{EnemyName}] Defeated!");

        if (modelTransform != null)
        {
            SpriteRenderer sprite = modelTransform.GetComponentInChildren<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.DOFade(0f, 1f).OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public bool IsDead() => currentHp <= 0;

    public Vector3 GetDamageSpawnPosition()
    {
        return damageSpawnPoint != null ? damageSpawnPoint.position : transform.position + Vector3.up;
    }
}