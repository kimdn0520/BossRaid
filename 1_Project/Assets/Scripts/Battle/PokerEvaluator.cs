using static Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PokerEvaluator
{
    // ==================================================================================
    // 1. 밸런스 설정
    // ==================================================================================
    private static Dictionary<HandType, (int baseScore, float multiplier)> _handRankings;

    // 게임 시작전 반드시 호출해줘야 합니다.
    public static void Initialize(HandRankDataSO dataSO)
    {
        if (dataSO != null)
        {
            _handRankings = dataSO.GetDictionary();
        }
        else
        {
            Debug.LogError("[PokerEvaluator] HandRankDataSO가 누락되었습니다!");
        }
    }

    // ==================================================================================
    // 2. 메인 로직
    // ==================================================================================
    public static HandResult Evaluate(List<CardData> hand)
    {
        // 안전장치: 데이터가 없으면 로그 띄우고 기본값 처리
        if (_handRankings == null || _handRankings.Count == 0)
        {
            Debug.LogError("[PokerEvaluator] 초기화되지 않았습니다. Initialize()를 먼저 호출하세요.");
            return new HandResult();
        }

        // 1. 전처리 (정렬 및 그룹화)
        var sortedHand = hand.OrderByDescending(c => c.CurrentRank).ToList();

        var rankGroups = sortedHand.GroupBy(c => c.CurrentRank)
                                   .OrderByDescending(g => g.Count())
                                   .ThenByDescending(g => g.Key)
                                   .ToList();

        bool isFlush = CheckFlush(sortedHand, out var flushCards);
        bool isStraight = CheckStraight(sortedHand, out var straightCards);

        // 2. 족보 판별 (순수 로직)
        HandType detectedType = HandType.HighCard;
        List<CardData> contributingCards = new List<CardData>();

        // [1] Five of a Kind
        if (rankGroups.Any(g => g.Count() >= 5))
        {
            detectedType = HandType.FiveOfAKind;
            contributingCards = rankGroups.First(g => g.Count() >= 5).Take(5).ToList();
        }
        // [2] Straight Flush
        else if (isFlush && isStraight)
        {
            detectedType = HandType.StraightFlush;
            contributingCards = sortedHand;
        }
        // [3] Four of a Kind
        else if (rankGroups.Any(g => g.Count() == 4))
        {
            detectedType = HandType.FourOfAKind;
            contributingCards = rankGroups.First(g => g.Count() == 4).ToList();
        }
        // [4] Full House
        else if (rankGroups.Count >= 2 && rankGroups[0].Count() == 3 && rankGroups[1].Count() >= 2)
        {
            detectedType = HandType.FullHouse;
            contributingCards.AddRange(rankGroups[0]);
            contributingCards.AddRange(rankGroups[1].Take(2));
        }
        // [5] Flush
        else if (isFlush)
        {
            detectedType = HandType.Flush;
            contributingCards = flushCards;
        }
        // [6] Straight
        else if (isStraight)
        {
            detectedType = HandType.Straight;
            contributingCards = straightCards;
        }
        // [7] Three of a Kind
        else if (rankGroups.Any(g => g.Count() == 3))
        {
            detectedType = HandType.ThreeOfAKind;
            contributingCards = rankGroups.First(g => g.Count() == 3).ToList();
        }
        // [8] Two Pair
        else if (rankGroups.Count(g => g.Count() == 2) >= 2)
        {
            detectedType = HandType.TwoPair;
            var pairs = rankGroups.Where(g => g.Count() == 2).Take(2);
            foreach (var p in pairs) contributingCards.AddRange(p);
        }
        // [9] One Pair
        else if (rankGroups.Any(g => g.Count() == 2))
        {
            detectedType = HandType.OnePair;
            contributingCards = rankGroups.First(g => g.Count() == 2).ToList();
        }
        // [10] High Card
        else
        {
            detectedType = HandType.HighCard;
            
            if (sortedHand.Count > 0)
            {
                contributingCards.Add(sortedHand[0]);
            }
        }

        // ==============================================================================
        // 3. 점수 계산 (데이터 참조)
        // ==============================================================================
        var scoreData = _handRankings[detectedType];

        HandResult result = new HandResult();
        result.handType = detectedType;
        result.contributingCards = contributingCards;

        int cardSum = contributingCards.Sum(c => (int)c.CurrentRank);
        result.baseScore = scoreData.baseScore + cardSum;
        result.multiplier = scoreData.multiplier;

        return result;
    }

    // --- Helper Methods ---
    private static bool CheckFlush(List<CardData> cards, out List<CardData> flushCards)
    {
        flushCards = null;
        if (cards.Count < 5) return false;

        var suitGroup = cards.GroupBy(c => c.CurrentSuit).FirstOrDefault(g => g.Count() >= 5);
        if (suitGroup != null)
        {
            flushCards = suitGroup.ToList();
            return true;
        }
        return false;
    }

    private static bool CheckStraight(List<CardData> sortedCards, out List<CardData> straightCards)
    {
        straightCards = new List<CardData>();
        if (sortedCards.Count < 5) return false;

        var uniqueRankCards = sortedCards.GroupBy(c => c.CurrentRank).Select(g => g.First()).ToList();

        // 1. 일반적인 스트레이트 검사
        int consecutive = 0;
        for (int i = 0; i < uniqueRankCards.Count - 1; i++)
        {
            if (uniqueRankCards[i].CurrentRank - uniqueRankCards[i + 1].CurrentRank == 1)
            {
                consecutive++;
                if (consecutive == 1) straightCards.Add(uniqueRankCards[i]);
                straightCards.Add(uniqueRankCards[i + 1]);

                if (consecutive >= 4) return true;
            }
            else
            {
                consecutive = 0;
                straightCards.Clear();
            }
        }

        // 2. Back Straight (Wheel)
        bool hasAce = uniqueRankCards.Any(c => c.CurrentRank == Rank.Ace);
        bool hasFiveLow = uniqueRankCards.Any(c => c.CurrentRank == Rank.Five) &&
                          uniqueRankCards.Any(c => c.CurrentRank == Rank.Four) &&
                          uniqueRankCards.Any(c => c.CurrentRank == Rank.Three) &&
                          uniqueRankCards.Any(c => c.CurrentRank == Rank.Two);

        if (hasAce && hasFiveLow)
        {
            straightCards.Clear();
            straightCards.Add(uniqueRankCards.First(c => c.CurrentRank == Rank.Ace));
            straightCards.Add(uniqueRankCards.First(c => c.CurrentRank == Rank.Five));
            straightCards.Add(uniqueRankCards.First(c => c.CurrentRank == Rank.Four));
            straightCards.Add(uniqueRankCards.First(c => c.CurrentRank == Rank.Three));
            straightCards.Add(uniqueRankCards.First(c => c.CurrentRank == Rank.Two));
            return true;
        }

        return false;
    }
}