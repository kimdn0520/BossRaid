using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Common;
using TMPro;
using UnityEngine.EventSystems;

public class CardVisual : MonoBehaviour
{
    [Header("UI Elements")]
    public Image cardBaseImage;
    public Image suitIcon;
    public TextMeshProUGUI rankText;
    public GameObject selectionHighlight;

    [SerializeField] private Button _button;

    public CardData Data => _data;

    private CardData _data;
    private Action<CardData, CardVisual> _onClickCallback;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    // 초기화: 데이터와 콜백 함수를 받습니다.
    public void Setup(CardData data, Action<CardData, CardVisual> onClickCallback)
    {
        data.IsSelected = false;
        _data = data;
        _onClickCallback = onClickCallback;

        // 상태 초기화
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        RefreshVisual();
    }

    // 클릭 감지
    private void OnButtonClicked()
    {
        _onClickCallback?.Invoke(_data, this);
    }

    // 외부(BattleManager)에서 호출 가능하도록 public으로 변경
    public void RefreshVisual()
    {
        if (_data == null) return;

        rankText.text = GetRankString(_data.CurrentRank);
        //suitIcon.color = GetSuitColor(_data.CurrentSuit);

        if (selectionHighlight != null)
            selectionHighlight.SetActive(_data.IsSelected);

        float targetY = _data.IsSelected ? 20f : 0f;

        transform.DOKill();
        transform.DOLocalMoveY(targetY, 0.2f);
    }

    private string GetRankString(Rank rank)
    {
        switch (rank) 
        { 
            case Rank.Jack: 
                return "J";
            case Rank.Queen:
                return "Q";
            case Rank.King:
                return "K"; 
            case Rank.Ace: 
                return "A";
            default: 
                return ((int)rank).ToString(); }
    }

    public void AnimateDraw(Vector3 startWorldPos, Vector3 targetLocalPos, int index)
    {
        // 1. 시작 위치 설정 (월드 좌표 기준 덱 위치)
        transform.position = startWorldPos;

        // 2. 약간의 회전과 스케일로 생동감 (작게 시작해서 커짐)
        transform.localScale = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 45f); // 45도 기울어진 상태

        // 3. DOTween 시퀀스
        Sequence seq = DOTween.Sequence();

        // 딜레이: 카드마다 조금씩 늦게 출발 (index * 0.1f)
        seq.PrependInterval(index * 0.1f);

        // 이동 + 회전 + 스케일 동시에
        seq.Append(transform.DOLocalMove(targetLocalPos, 0.4f).SetEase(Ease.OutBack)); // 쫀득한 움직임
        seq.Join(transform.DOScale(1f, 0.3f));
        seq.Join(transform.DOLocalRotate(Vector3.zero, 0.4f));

        seq.OnComplete(() =>
        {
            _button.interactable = true; // 도착하면 클릭 가능

            // LayoutGroup이 다시 잡을 수 있도록 위치 고정 해제 등의 처리가 필요할 수 있으나,
            // 보통은 도착 후 가만히 있으면 됩니다.
        });
    }

    // 버리기 연출: 내 자리 -> 버린 카드 더미
    public void AnimateDiscard(Vector3 targetWorldPos, Action onComplete)
    {
        _button.interactable = false;
        transform.DOKill(); // 기존 트윈 제거

        // 부모를 잠시 해제하거나 LayoutGroup의 영향을 안받게 해야 자연스럽게 날아감.
        // 여기서는 BattleManager에서 부모를바꿔서 처리하는 것을 가정하고 움직임만 구현.

        Sequence seq = DOTween.Sequence();

        // 카드가 살짝 떴다가 날아가는 느낌
        seq.Append(transform.DOScale(1.2f, 0.1f));

        // 목표 지점으로 날아가면서 회전 + 작아짐
        seq.Append(transform.DOMove(targetWorldPos, 0.4f).SetEase(Ease.InCubic));
        seq.Join(transform.DORotate(new Vector3(0, 0, 180f), 0.4f)); // 휙 돌면서
        seq.Join(transform.DOScale(0f, 0.4f)); // 사라짐

        seq.OnComplete(() =>
        {
            onComplete?.Invoke(); // 풀 반환 콜백
        });
    }

    private Color GetSuitColor(Suit suit) 
    { 
        return (suit == Suit.Heart || suit == Suit.Diamond) ? Color.red : Color.black;
    }

    public void ReturnToPool()
    {
        PoolManager.Instance.Return(this.gameObject);
    }
}
