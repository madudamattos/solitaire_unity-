using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Solitaire.Views;
using Solitaire.Logic;
using System;

public class BoardManager : MonoBehaviour
{
    [Header("Structures")]
    public PileView _stockPile;
    public PileView _wastePile;
    public List<PileView> _foundationPiles;
    public List<PileView> _tableauPiles;

    public event Action OnAutoCompleteFinished;

    // public static BoardManager Instance { get; private set; }

    // private void Awake()
    // {
    //     if (Instance == null) Instance = this;
    //     else Destroy(gameObject);
    // }

    public void RunAutoComplete()
    {
        StartCoroutine(AutoCompleteRoutine());
    }

    private IEnumerator AutoCompleteRoutine()
    {
        bool isWon = false;
        List<PileView> pilesToScan = new List<PileView>(_tableauPiles);
        pilesToScan.Add(_wastePile);

        while (!isWon)
        {
            bool movedCardInThisLoop = false;

            foreach (PileView pile in pilesToScan)
            {
                if (pile.GetPileCount() == 0) continue;

                CardView topCard = pile.GetLastCard();

                PileView targetFoundation = MoveValidator.GetValidFoundation(topCard.Presenter.Model, _foundationPiles);

                if (targetFoundation != null)
                {
                    MoveExecutor.ExecuteMove(new List<CardView> { topCard }, pile, targetFoundation);
                    movedCardInThisLoop = true;
                    
                    yield return new WaitForSeconds(0.1f); 
                    break;
                }
            }

            if (!movedCardInThisLoop) break;

            isWon = WinValidator.CheckForVictory(_foundationPiles);
        }

        if (isWon)
        {
            OnAutoCompleteFinished?.Invoke();
        }
    }

    public void ClearBoard()
    {
        // clear stock
        int count = _stockPile.GetPileCount();

        List<CardView> cards = _stockPile.GetCardsInPile();

        for(int i = 0; i < count; i++)
        {
            _stockPile.RemoveCard(cards[i]);
        }

        cards.Clear();

        // clear waste
        count = _wastePile.GetPileCount();

        cards = _wastePile.GetCardsInPile();

        for(int i = 0; i < count; i++)
        {
            _wastePile.RemoveCard(cards[i]);
        }

        cards.Clear();

        // clear foundations
        for(int i=0; i<4; i++)
        {
            PileView pile = _foundationPiles[i];

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
            PileView pile = _tableauPiles[i];

            cards = pile.GetCardsInPile();

            for(int j = 0; j < count; j++)
            {
                pile.RemoveCard(cards[j]);
            }

            cards.Clear();
        }

    }
}