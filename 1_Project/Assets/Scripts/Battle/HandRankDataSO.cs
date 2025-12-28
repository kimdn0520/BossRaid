using System.Collections.Generic;
using UnityEngine;
using static Common;

[CreateAssetMenu(fileName = "HandRankData", menuName = "CardGame/Hand Rank Data")]
public class HandRankDataSO : ScriptableObject
{
    [System.Serializable]
    public struct HandRankItem
    {
        public HandType handType;
        public int baseScore;
        public float multiplier;
    }

    [Header("족보별 점수 설정")]
    public List<HandRankItem> rankSettings = new List<HandRankItem>()
    {
        // 기본값 세팅 (생성 시 편의를 위해)
        new HandRankItem { handType = HandType.FiveOfAKind,  baseScore = 120, multiplier = 10.0f },
        new HandRankItem { handType = HandType.StraightFlush,baseScore = 100, multiplier = 8.0f },
        new HandRankItem { handType = HandType.FourOfAKind,  baseScore = 60,  multiplier = 7.0f },
        new HandRankItem { handType = HandType.FullHouse,    baseScore = 40,  multiplier = 4.0f },
        new HandRankItem { handType = HandType.Flush,        baseScore = 35,  multiplier = 4.0f },
        new HandRankItem { handType = HandType.Straight,     baseScore = 30,  multiplier = 4.0f },
        new HandRankItem { handType = HandType.ThreeOfAKind, baseScore = 30,  multiplier = 3.0f },
        new HandRankItem { handType = HandType.TwoPair,      baseScore = 20,  multiplier = 2.0f },
        new HandRankItem { handType = HandType.OnePair,      baseScore = 10,  multiplier = 2.0f },
        new HandRankItem { handType = HandType.HighCard,     baseScore = 0,   multiplier = 1.0f }
    };

    public Dictionary<HandType, (int baseScore, float multiplier)> GetDictionary()
    {
        var dic = new Dictionary<HandType, (int, float)>();
        foreach (var item in rankSettings)
        {
            if (!dic.ContainsKey(item.handType))
            {
                dic.Add(item.handType, (item.baseScore, item.multiplier));
            }
        }
        return dic;
    }
}