// ==========================================
// 4. 전투 매니저 (The Conductor)
// ==========================================

using Cysharp.Threading.Tasks;
using static Common;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.EventSystems.EventTrigger;

public class BattleManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int drawCount = 5;
    [SerializeField] int maxRedrawCount = 1; // 턴당 교체 기회

    [Header("State")]
    [SerializeField] List<CardData> allPile = new List<CardData>();
    [SerializeField] List<CardData> handPile = new List<CardData>();
    [SerializeField] List<CardData> discardPile = new List<CardData>();
    [SerializeField] Transform allPileTr;
    [SerializeField] Transform discardPileTr;

    private int currentRedrawCount = 0;
    private bool isPlayerTurnActive = false;

    // 유물/아이템 리스트
    private List<IBattleObserver> activeRelics = new List<IBattleObserver>();

    // UI 연결 (실제 유니티 버튼과 연결해야 함)
    [SerializeField] private Transform handTransform;

    private void Start()
    {
        // 테스트용: 게임 시작
        InitializeBattle();

        GameLoop().Forget(); // UniTask 실행
    }

    private void InitializeBattle()
    {
        // 덱 생성 (기획서의 20장 덱)
        allPile = DeckGenerator.CreateStarterDeck();

        ShuffleDeck();
    }

    // 메인 게임 루프 (State Machine via UniTask)
    private async UniTaskVoid GameLoop()
    {
        while (true) // 전투 종료 조건까지 반복
        {
            // 테스트 1초 대기
            await UniTask.Delay(1000);

            // 1. 턴 시작 단계
            await StartTurnPhase();

            // 2. 플레이어 조작 단계 (입력 대기)
            await PlayerActionPhase();

            // 3. 공격 및 판정 단계
            await ResolutionPhase();

            // 승패 체크 로직 추가 필요

            // 4. 턴 종료 및 정리
            await EndTurnPhase();

            // 5. 적 턴
            await EnemyTurnPhase();

            // 승패 체크 로직 추가 필요

        }
    }

    private async UniTask StartTurnPhase()
    {
        Debug.Log("--- Turn Start ---");

        // 덱 섞기 연출이 필요하다면 여기서 체크
        if (allPile.Count < drawCount)
        {
            await ReshuffleDiscardToDraw();
        }

        currentRedrawCount = 0;

        // 유물들의 턴 시작 효과 발동
        foreach (var observer in activeRelics.OfType<IOnTurnStart>())
        {
            await observer.OnTurnStartAsync();
        }

        // 드로우
        await DrawCards(drawCount);
    }

    private async UniTask DrawCards(int count)
    {
        // allPile에 카드가 부족하다면 버린더미에서 가져온다.
        if (allPile.Count < count)
        {
            await ReshuffleDiscardToDraw();
        }

        List<CardVisual> spawnedVisuals = new List<CardVisual>();

        // 1. 데이터 처리 및 카드 생성
        for (int i = 0; i < count; i++)
        {
            if (allPile.Count == 0) break;

            CardData card = allPile[0];
            allPile.RemoveAt(0);
            handPile.Add(card);

            CardVisual visual = PoolManager.Instance.Get<CardVisual>("CardVisual", handTransform);
            visual.Setup(card, OnCardClicked);

            visual.transform.localScale = Vector3.zero;

            // Layout 계산을 위해 일단 활성화는 되어있어야 함
            spawnedVisuals.Add(visual);
        }

        // 2. 프레임 대기 (LayoutGroup이 자식들을 인식하도록)
        await UniTask.Yield(PlayerLoopTiming.Update);

        // 3. 레이아웃 강제 갱신 (모든 카드가 정렬된 최종 위치를 계산)
        LayoutRebuilder.ForceRebuildLayoutImmediate(handTransform as RectTransform);

        // 4. 각 카드별로 목표 위치 저장 후 애니메이션 시작
        for (int i = 0; i < spawnedVisuals.Count; i++)
        {
            CardVisual visual = spawnedVisuals[i];

            // 레이아웃 그룹에 의해 계산된 현재 로컬 위치가 '목표 지점'임
            Vector3 targetLocalPos = visual.transform.localPosition;

            // 덱 위치에서 시작
            // AnimateDraw 내부에서 position을 덱 위치로 옮기고 targetLocalPos로 이동시킴
            visual.AnimateDraw(allPileTr.position, targetLocalPos, i);
        }

        // 5. 연출이 끝날 때까지 대기 (가장 마지막 카드의 도착 시간 고려)
        // (딜레이 0.1f * count) + 이동시간 0.4f + 여유시간
        await UniTask.Delay(TimeSpan.FromSeconds((count * 0.1f) + 0.5f));
    }

    private async UniTask PlayerActionPhase()
    {
        Debug.Log("Waiting for Player...");

        isPlayerTurnActive = true;

        // 아이템 사용/ 공격 버튼 / 교체
        // 로비로 나가는 경우 -> 배틀매니저 종료하는 쪽으로 넘어가도록 해줘야함.

        // Submit 버튼이 눌리면 isPlayerTurnActive가 false가 됩니다.
        await UniTask.WaitWhile(() => isPlayerTurnActive);

        // 여기에 아이템 사용 로직도 포함됨
    }

    private async UniTask ResolutionPhase()
    {
        // 사용자가 '선택한 카드'만 가져옴
        List<CardData> attackingCards = handPile.Where(c => c.IsSelected).ToList();

        Debug.Log($"Attacking with {attackingCards.Count} cards...");

        // 1. 선택된 카드로 족보 계산
        HandResult handResult = PokerEvaluator.Evaluate(attackingCards);
        Debug.Log($"Hand: {handResult.handType}, Base: {handResult.baseScore}");

        // 2. 데미지 계산
        float finalDamage = handResult.baseScore * handResult.multiplier;

        foreach (var relic in activeRelics.OfType<IOnCalculateDamage>())
        {
            relic.ModifyDamage(handResult, ref finalDamage);
        }

        Debug.Log($"Final Damage to Enemy: {finalDamage}");

        // 실제 공격 (선택된 카드들 버림패로 이동시키거나 소멸시키는 연출)
        await UniTask.Delay(1000);
    }

    private async UniTask EnemyTurnPhase()
    {
        Debug.Log("Enemy Turn...");

        //if (enemy != null && !enemy.IsDead())
        //{
        //    // 적의 공격을 기다림
        //    // 콜백(람다)을 통해 실제로 때리는 시점에 데미지 처리를 수행
        //    await enemy.Attack(() =>
        //    {
        //        // 실제 데미지 로직 (여기서 Player.TakeDamage 호출)
        //        Debug.Log($"[BattleManager] Player hit by {enemy.EnemyName} for {enemy.BaseDamage} damage!");

        //        // 추후 구현: player.TakeDamage(enemy.BaseDamage);
        //    });
        //}

        await UniTask.Delay(500); // 턴 종료 전 잠시 대기
    }

    private async UniTask EndTurnPhase()
    {
        // 손패 버리기
        discardPile.AddRange(handPile);
        handPile.Clear();


        List<CardVisual> allVisuals = new List<CardVisual>();
        foreach (Transform child in handTransform)
        {
            var v = child.GetComponent<CardVisual>();
            if (v) allVisuals.Add(v);
        }

        // 모든 카드를 순차적으로 버림 더미로 날림
        for (int i = 0; i < allVisuals.Count; i++)
        {
            CardVisual v = allVisuals[i];

            // 약간의 딜레이를 두고 날아감
            DOVirtual.DelayedCall(i * 0.05f, () => {
                v.AnimateDiscard(discardPileTr.position, () => PoolManager.Instance.Return(v.gameObject));
            });
        }

        await UniTask.Delay(500); // 연출 시간 확보

        Debug.Log("End Turn");
    }

    // ============================================================
    // 카드가 클릭되었을 때 실행되는 함수 (Controller Logic)
    // ============================================================
    public void OnCardClicked(CardData card, CardVisual visual)
    {
        // 1. 권한 체크 (내 턴이 아니면 무시)
        if (!isPlayerTurnActive) return;

        // 2. 데이터 수정 (선택/해제 토글)
        card.IsSelected = !card.IsSelected;

        // 3. 화면 갱신
        visual.RefreshVisual();

        Debug.Log($"Controller processed click. Card Selected: {card.IsSelected}");
    }

    // 교체(Discard & Redraw) 버튼 클릭 시
    public async void OnDiscardButtonClicked()
    {
        if (!isPlayerTurnActive) return;

        if (currentRedrawCount >= maxRedrawCount)
        {
            Debug.Log("남은 교체 기회가 없습니다!");
            return;
        }

        List<CardData> selectedCards = handPile.Where(c => c.IsSelected).ToList();

        if (selectedCards.Count == 0)
        {
            Debug.Log("교체할 카드를 선택해주세요.");
            return;
        }

        // 로직 실행
        currentRedrawCount++;

        // 화면상의 카드 찾기 및 버리기 연출
        List<CardVisual> selectedVisuals = new List<CardVisual>();
        foreach (Transform child in handTransform)
        {
            CardVisual v = child.GetComponent<CardVisual>();
            if (v != null && v.Data.IsSelected)
            {
                selectedVisuals.Add(v);
            }
        }

        foreach (var visual in selectedVisuals)
        {
            CardData data = visual.Data;

            handPile.Remove(data);
            discardPile.Add(data);

            visual.transform.SetParent(handTransform.parent);

            visual.AnimateDiscard(discardPileTr.position, () => PoolManager.Instance.Return(visual.gameObject));
        }

        // 버린 만큼 새로 드로우 (부족하면 알아서 리셔플됨)
        await DrawCards(selectedCards.Count);
    }

    // 공격(Submit) 버튼 클릭 시
    public void OnAttackButtonClicked()
    {
        if (!isPlayerTurnActive) return;

        List<CardData> selectedCards = handPile.Where(c => c.IsSelected).ToList();

        if (selectedCards.Count == 0)
        {
            Debug.Log("공격할 카드를 1장 이상 선택해주세요!");
            return;
        }

        // 턴 종료 플래그 설정 -> GameLoop가 ResolutionPhase로 넘어감
        isPlayerTurnActive = false;
    }

    private async UniTask ReshuffleDiscardToDraw()
    {
        Debug.Log("Reshuffling...");

        // 1. 데이터 이동
        allPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();

        // 2. 시각적 연출 (카드 뒷면이 날아가는 효과)
        int visualCount = Mathf.Min(allPile.Count, 8);

        for (int i = 0; i < visualCount; i++)
        {
            GameObject dummy = PoolManager.Instance.Get("CardBackEffect", discardPileTr);

            dummy.transform.position = discardPileTr.position;
            dummy.transform.localScale = Vector3.one;

            // 덱으로 날리기
            Sequence seq = DOTween.Sequence();
            seq.PrependInterval(i * 0.05f);
            seq.Append(dummy.transform.DOMove(allPileTr.position, 0.5f).SetEase(Ease.InOutQuad));
            seq.Join(dummy.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360));

            seq.OnComplete(() => {
                PoolManager.Instance.Return(dummy);
            });
        }

        // 덱이 흔들리는 연출 추가
        allPileTr.DOShakePosition(0.5f, 10f, 20);

        await UniTask.Delay(600); // 연출 끝날 때까지 대기
    }

    private void ShuffleDeck()
    {
        allPile = allPile.OrderBy(x => UnityEngine.Random.value).ToList();
    }
}