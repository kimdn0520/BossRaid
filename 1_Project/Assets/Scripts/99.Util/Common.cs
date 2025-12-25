using System.Collections.Generic;
using System;

// ==========================================
// 1. 기본 데이터 정의 (Data Definitions)
// ==========================================
public static class Common
{
    public enum Suit { Spade, Heart, Diamond, Clover, None } // None은 조커용
    public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack = 11, Queen = 12, King = 13, Ace = 14, Joker = 99 }
    public enum HandType { HighCard, OnePair, TwoPair, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StraightFlush, FiveOfAKind }

    [Serializable]
    public class CardData
    {
        public string id; // 고유 ID (생성 시 부여)
        public Suit baseSuit;
        public Rank baseRank;

        // 런타임 변조를 위한 프로퍼티 (페인트, 수정액 등)
        public Suit CurrentSuit { get; set; }
        public Rank CurrentRank { get; set; }
        public bool IsBurned { get; set; } // 라이터 등으로 탔는지

        // UI 상호작용 상태
        public bool IsSelected { get; set; } = false;

        public CardData(Suit suit, Rank rank)
        {
            id = System.Guid.NewGuid().ToString();
            baseSuit = suit;
            baseRank = rank;
            CurrentSuit = suit;
            CurrentRank = rank;
        }
    }

    // 족보 계산 결과 반환용 구조체
    public struct HandResult
    {
        public HandType handType;

        public int baseScore;

        public float multiplier;

        public List<CardData> contributingCards; // 족보 형성에 기여한 카드들
    }
}