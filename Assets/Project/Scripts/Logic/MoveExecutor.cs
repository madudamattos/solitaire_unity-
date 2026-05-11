using System.Collections.Generic;
using System.Linq;
using System;
using Solitaire.Views;
using Solitaire.Core;
using Solitaire.Input;
using UnityEngine;

namespace Solitaire.Logic
{
    public static class MoveExecutor
    {
        public static event Action OnBoardStateChanged;
        public static bool ExecuteMove(List<CardView> cardsToMove, PileView sourcePile, PileView targetPile)
        {
            bool flippedCard = false;
            
            bool stockToWaste = sourcePile.Type == PileType.Stock && targetPile.Type == PileType.Waste;
            bool wasteToStock = sourcePile.Type == PileType.Waste && targetPile.Type == PileType.Stock;

            // Se for reciclar (Waste->Stock), vira o bolo de ponta cabeça invertendo a lista
            List<CardView> cardsToProcess = wasteToStock ? Enumerable.Reverse(cardsToMove).ToList() : cardsToMove;

            int count = cardsToProcess.Count;
            for(int i=0; i<count; i++)
            {
                CardView card = cardsToProcess[i];
                Vector3 targetPosition = targetPile.GetNextCardPosition();
                card.transform.SetParent(targetPile.transform);

                sourcePile.RemoveCard(card);
                targetPile.AddCard(card);

                card.SetSortingOrder(targetPile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = targetPile;

                if(stockToWaste)
                {
                    card.RequestFlip();
                    card.MoveTo(targetPosition, 0.12f, 0, () => targetPile.UpdateWasteVisuals());

                } else if ( wasteToStock)
                {
                    card.RequestFlip();
                    card.MoveTo(targetPosition, 0.12f, 0, () => sourcePile.UpdateWasteVisuals());
                }
                else
                {
                    card.MoveTo(targetPosition, 0.30f, 0);
                }
            }
            
            if(sourcePile.Type == PileType.Tableau && sourcePile.GetPileCount() > 0)
            {
                CardView cardLeftBehind = sourcePile.GetLastCard();
                if(cardLeftBehind.Presenter.Model.IsFaceUp == false)
                {
                    cardLeftBehind.RequestFlip();
                    flippedCard = true;    
                }
            }
            
            OnBoardStateChanged?.Invoke();

            return flippedCard;
        }

        public static void UndoMove(List<CardView> cardsToMove, PileView sourcePile, PileView targetPile, bool revertFlip)
        {
            if(revertFlip && sourcePile.GetPileCount() > 0)
            {
                sourcePile.GetLastCard().RequestFlip();
            }

            bool stockToWaste = sourcePile.Type == PileType.Stock && targetPile.Type == PileType.Waste;
            bool wasteToStock = sourcePile.Type == PileType.Waste && targetPile.Type == PileType.Stock;


            List<CardView> cardsToProcess = stockToWaste ? Enumerable.Reverse(cardsToMove).ToList() : cardsToMove;

            int count = cardsToProcess.Count;

            for(int i=0; i< count; i++)
            {
                CardView card = cardsToProcess[i];
                card.transform.SetParent(sourcePile.transform);
                Vector3 targetPosition = sourcePile.GetNextCardPosition();

                targetPile.RemoveCard(card);
                sourcePile.AddCard(card);
                
                card.SetSortingOrder(sourcePile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = sourcePile;

                if(stockToWaste)
                {
                    card.RequestFlip();
                    card.MoveTo(targetPosition, 0.12f, 0, () => targetPile.UpdateWasteVisuals());

                } else if ( wasteToStock)
                {
                    card.RequestFlip();
                    card.MoveTo(targetPosition, 0.12f, 0, () => sourcePile.UpdateWasteVisuals());
                }
                else
                {
                    card.MoveTo(targetPosition, 0.30f, 0);
                }
            }
        
            OnBoardStateChanged?.Invoke();
        }

    }
}