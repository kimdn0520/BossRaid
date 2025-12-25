using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Base Stats")]
    [SerializeField] protected float maxHp = 100f;
    [SerializeField] protected float currentHp;
    [SerializeField] protected int baseDamage = 10;
    [SerializeField] protected string enemyName = "Monster";

    [Header("UI References")]
    [SerializeField] protected Image hpBarFill;
    [SerializeField] protected TextMeshProUGUI hpText;
    [SerializeField] protected SpriteRenderer baseSprite;
    [SerializeField] protected Transform modelTransform;

    protected virtual void Start()
    {
        // 초기화
        currentHp = maxHp;
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

        Debug.Log($"[{enemyName}] took {amount} damage. (HP: {currentHp})");

        if (IsDead())
        {
            Die();
        }
    }

    // 추상 메서드: 자식들이 반드시 구현해야 하는 '공격 행동'
    // BattleManager에서는 이 함수 호출.
    public abstract int Attack();

    // 공통 연출: 피격 (오버라이드 가능)
    protected virtual void PlayHitAnimation()
    {
        if (modelTransform == null) return;

        modelTransform.DOKill(); // 기존 트윈 중단
        modelTransform.DOShakePosition(0.5f, 1f, 20);
        modelTransform.GetComponent<Image>()?.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
    }

    // 공통 로직: UI 갱신
    protected void UpdateUI()
    {
        if (hpBarFill != null)
            hpBarFill.fillAmount = currentHp / maxHp;

        if (hpText != null)
            hpText.text = $"{currentHp}/{maxHp}";
    }

    // 공통 로직: 사망
    protected virtual void Die()
    {
        Debug.Log($"[{enemyName}] Defeated!");

        SpriteRenderer sprite = modelTransform.GetComponent<SpriteRenderer>();
            
        if (baseSprite != null)
        {
            baseSprite.DOFade(0f, 1f).OnComplete(() => gameObject.SetActive(false));
            return;
        }

        gameObject.SetActive(false);
    }

    public bool IsDead() => currentHp <= 0;
}