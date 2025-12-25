// ==========================================
// 덱 생성기 (Helper)
// ==========================================
using static Common;
using System.Collections.Generic;

public static class DeckGenerator
{
    public static List<CardData> CreateStarterDeck()
    {
        List<CardData> deck = new List<CardData>();
        // 스페이드 2~6
        for (int i = 2; i <= 6; i++) deck.Add(new CardData(Suit.Spade, (Rank)i));
        // 하트 2~6
        for (int i = 2; i <= 6; i++) deck.Add(new CardData(Suit.Heart, (Rank)i));
        // 다이아 2~6
        for (int i = 2; i <= 6; i++) deck.Add(new CardData(Suit.Diamond, (Rank)i));
        // 클로버 2~6
        for (int i = 2; i <= 6; i++) deck.Add(new CardData(Suit.Clover, (Rank)i));

        return deck;
    }
}