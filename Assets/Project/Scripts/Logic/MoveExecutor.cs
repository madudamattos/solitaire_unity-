using System.Collections.Generic;
using System.Linq;
using Solitaire.Views;
using Solitaire.Core;
using Solitaire.Input;

namespace Solitaire.Logic
{
    public static class MoveExecutor
    {
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
                card.transform.position = targetPile.GetNextCardPosition();
                card.transform.SetParent(targetPile.transform);

                sourcePile.RemoveCard(card);
                targetPile.AddCard(card);

                card.SetSortingOrder(targetPile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = targetPile;

                if(stockToWaste || wasteToStock) card.RequestFlip();
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
            
            // Sempre atualiza o leque do Waste se ele estiver envolvido
            if(sourcePile.Type == PileType.Waste) sourcePile.UpdateWasteVisuals();
            if(targetPile.Type == PileType.Waste) targetPile.UpdateWasteVisuals();

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
                card.transform.position = sourcePile.GetNextCardPosition();

                targetPile.RemoveCard(card);
                sourcePile.AddCard(card);
                
                card.SetSortingOrder(sourcePile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = sourcePile;

                if(stockToWaste || wasteToStock) card.RequestFlip();
            }

            if(sourcePile.Type == PileType.Waste) sourcePile.UpdateWasteVisuals();
            if(targetPile.Type == PileType.Waste) targetPile.UpdateWasteVisuals();
        }
    }
}