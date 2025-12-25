using static Common;
using System.Collections.Generic;
using System.Linq;

public static class PokerEvaluator
{
    /// <summary>
    /// 입력된 카드 리스트를 분석하여 가장 높은 족보와 점수 정보를 반환합니다.
    /// </summary>
    public static HandResult Evaluate(List<CardData> hand)
    {
        // 1. 분석을 위해 카드 정렬 (Rank 내림차순: A -> K -> ... -> 2)
        var sortedHand = hand.OrderByDescending(c => c.CurrentRank).ToList();

        // 2. 기본 패턴 분석 (플러시 여부, 숫자 그룹화 등)
        bool isFlush = CheckFlush(sortedHand, out var flushCards);
        bool isStraight = CheckStraight(sortedHand, out var straightCards);

        // 숫자별 그룹화 (예: [K, K, 7, 7, 7] -> {K:2, 7:3})
        var rankGroups = sortedHand.GroupBy(c => c.CurrentRank)
                                   .OrderByDescending(g => g.Count()) // 장수가 많은 순서대로 (트리플 > 페어)
                                   .ThenByDescending(g => g.Key)    // 같은 장수면 숫자가 높은 순서대로
                                   .ToList();

        HandResult result = new HandResult();
        result.contributingCards = new List<CardData>();

        // 3. 족보 판별 (높은 등급부터 순차 확인)

        // [1] Five of a Kind (5장 동일)
        if (rankGroups.Any(g => g.Count() >= 5))
        {
            SetResult(ref result, HandType.FiveOfAKind, 120, 10.0f);
            result.contributingCards = rankGroups.First(g => g.Count() >= 5).Take(5).ToList();
        }
        // [2] Straight Flush (스트레이트 + 플러시)
        else if (isFlush && isStraight)
        {
            // 주의: 플러시이면서 동시에 스트레이트여야 함 (단순히 조건 2개가 참인게 아니라, 같은 카드들이 만족해야 함)
            // 여기서는 5장 덱 기준이므로 간단히 처리하지만, 7장 포커라면 교집합 확인이 필요함.
            // 20장 덱 빌딩 게임의 특성상 현재 손패 5장 전체가 대상이라 가정.
            SetResult(ref result, HandType.StraightFlush, 100, 8.0f);
            result.contributingCards = sortedHand;
        }
        // [3] Four of a Kind (포카드)
        else if (rankGroups.Any(g => g.Count() == 4))
        {
            SetResult(ref result, HandType.FourOfAKind, 60, 7.0f);
            result.contributingCards = rankGroups.First(g => g.Count() == 4).ToList();
        }
        // [4] Full House (트리플 + 원페어)
        else if (rankGroups.Count >= 2 && rankGroups[0].Count() == 3 && rankGroups[1].Count() >= 2)
        {
            SetResult(ref result, HandType.FullHouse, 40, 4.0f);
            result.contributingCards.AddRange(rankGroups[0]); // 트리플 부분
            result.contributingCards.AddRange(rankGroups[1].Take(2)); // 페어 부분
        }
        // [5] Flush (무늬 동일)
        else if (isFlush)
        {
            SetResult(ref result, HandType.Flush, 35, 4.0f);
            result.contributingCards = flushCards;
        }
        // [6] Straight (숫자 연속)
        else if (isStraight)
        {
            SetResult(ref result, HandType.Straight, 30, 4.0f);
            result.contributingCards = straightCards;
        }
        // [7] Three of a Kind (트리플)
        else if (rankGroups.Any(g => g.Count() == 3))
        {
            SetResult(ref result, HandType.ThreeOfAKind, 30, 3.0f);
            result.contributingCards = rankGroups.First(g => g.Count() == 3).ToList();
        }
        // [8] Two Pair (투 페어)
        else if (rankGroups.Count(g => g.Count() == 2) >= 2)
        {
            SetResult(ref result, HandType.TwoPair, 20, 2.0f);
            var pairs = rankGroups.Where(g => g.Count() == 2).Take(2).ToList();
            foreach (var p in pairs) result.contributingCards.AddRange(p);
        }
        // [9] One Pair (원 페어)
        else if (rankGroups.Any(g => g.Count() == 2))
        {
            SetResult(ref result, HandType.OnePair, 10, 2.0f);
            result.contributingCards = rankGroups.First(g => g.Count() == 2).ToList();
        }
        // [10] High Card (탑)
        else
        {
            SetResult(ref result, HandType.HighCard, 5, 1.0f);
            result.contributingCards = sortedHand; // 하이카드는 전체가 기여한다고 보거나, 가장 높은 1장만 볼 수도 있음 (기획 의존)
        }

        // 4. 점수 계산 (족보 구성 카드들의 합산 점수 + 족보 기본 점수)
        int cardSum = result.contributingCards.Sum(c => (int)c.CurrentRank);
        result.baseScore += cardSum;

        return result;
    }

    // --- Helper Methods ---
    private static void SetResult(ref HandResult result, HandType type, int score, float mult)
    {
        result.handType = type;
        result.baseScore = score;
        result.multiplier = mult;
    }

    private static bool CheckFlush(List<CardData> cards, out List<CardData> flushCards)
    {
        flushCards = null;

        if (cards.Count < 5) return false;

        // 문양별로 그룹화하여 5장 이상인 문양이 있는지 확인
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

        // 중복 숫자는 스트레이트 계산 방해되므로 제거한 리스트 생성 (단, 원본 카드 참조는 유지해야 함)
        var uniqueRankCards = sortedCards.GroupBy(c => c.CurrentRank).Select(g => g.First()).ToList();

        // 1. 일반적인 스트레이트 검사 (10, 9, 8, 7, 6)
        int consecutive = 0;
        for (int i = 0; i < uniqueRankCards.Count - 1; i++)
        {
            if (uniqueRankCards[i].CurrentRank - uniqueRankCards[i + 1].CurrentRank == 1)
            {
                consecutive++;
                if (consecutive == 1) straightCards.Add(uniqueRankCards[i]); // 첫 시작 카드 추가
                straightCards.Add(uniqueRankCards[i + 1]);

                if (consecutive >= 4) return true; // 5장 연속 (간격이 4번)
            }
            else
            {
                consecutive = 0;
                straightCards.Clear();
            }
        }

        // 2. Back Straight (Wheel) 검사: A, 5, 4, 3, 2
        // 조건: A가 있고, 5,4,3,2가 있어야 함.
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