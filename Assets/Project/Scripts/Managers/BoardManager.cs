using UnityEngine;
using System.Collections.Generic;
using Solitaire.Views;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    [Header("Structures")]
    public PileView stockPile;
    public PileView wastePile;
    public List<PileView> foundationPiles;
    public List<PileView> tableauPiles;
    public void ClearBoard()
    {
        // clear stock
        int count = stockPile.GetPileCount();

        List<CardView> cards = stockPile.GetCardsInPile();

        for(int i = 0; i < count; i++)
        {
            stockPile.RemoveCard(cards[i]);
        }

        cards.Clear();

        // clear waste
        count = wastePile.GetPileCount();

        cards = wastePile.GetCardsInPile();

        for(int i = 0; i < count; i++)
        {
            wastePile.RemoveCard(cards[i]);
        }

        cards.Clear();

        // clear foundations
        for(int i=0; i<4; i++)
        {
            PileView pile = foundationPiles[i];

            cards = pile.GetCardsInPile();

            for(int j = 0; j < count; j++)
            {
                pile.RemoveCard(cards[j]);
            }

            cards.Clear();
        }

        // clear tableaus
        for(int i=0; i<7; i++)
        {
            PileView pile = tableauPiles[i];

            cards = pile.GetCardsInPile();

            for(int j = 0; j < count; j++)
            {
                pile.RemoveCard(cards[j]);
            }

            cards.Clear();
        }

    }
}