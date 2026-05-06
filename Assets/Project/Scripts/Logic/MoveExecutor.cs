using System.Collections.Generic;
using Solitaire.Views;
using Solitaire.Core;
using Solitaire.Input;

namespace Solitaire.Logic
{
    public static class MoveExecutor
    {
        public static void ExecuteMove(List<CardView> cardsToMove, PileView sourcePile, PileView targetPile)
        {
            // atualiza a logica de posicionamento da carta nas pilhas
            foreach(CardView card in cardsToMove)
            {
                // atualiza o visual
                card.transform.position = targetPile.GetNextCardPosition();
                card.transform.SetParent(targetPile.transform);

                // atualiza os estados lógicos da pilha
                sourcePile.RemoveCard(card);
                targetPile.AddCard(card);

                card.SetSortingOrder(targetPile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = targetPile;
            }
            
            // vira a ultima carta da pilha antiga 
            if(sourcePile.Type == PileType.Tableau && sourcePile.GetPileCount() > 0)
            {
                CardView cardLeftBehind = sourcePile.GetLastCard();
                if(cardLeftBehind.Presenter.Model.IsFaceUp == false)
                    cardLeftBehind.RequestFlip();
            }

            if(sourcePile.Type == PileType.Waste)
            {
                sourcePile.UpdateWasteVisuals();
            }
        }
    }
}