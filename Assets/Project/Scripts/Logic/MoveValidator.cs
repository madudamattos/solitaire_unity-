using Solitaire.Models;
using Solitaire.Views;
using Solitaire.Core;
using System.Collections.Generic;

namespace Solitaire.Logic
{
    public static class MoveValidator
    {
        public static bool CanDrag(CardView cardToDrag, PileView currentPile)
        {
            if(GameManager.Instance.CurrentState != GameState.Playing) 
                return false;

            if(currentPile.Type == PileType.Stock)
                return false;

            if(currentPile.Type == PileType.Waste && currentPile.GetLastCard() != cardToDrag) 
                return false;

            if (!cardToDrag.Presenter.Model.IsFaceUp) 
                return false;
            
            return true;
        }
        public static bool IsValidMove(CardModel cardToMove, PileView targetPile, int draggetCardsCount)
        {
            // Regra 1 : Soltou a carta no tableau
            if(targetPile.Type == PileType.Tableau)
            {
                if(targetPile.GetPileCount() == 0)
                {
                    return cardToMove.Rank == Rank.King; // se o tableau esta vazio, so aceita um rei
                }

                CardModel targetCard = targetPile.GetLastCard().Presenter.Model;

                bool isDifferentColor = cardToMove.IsRed != targetCard.IsRed;
                bool isDescendingOrder = (int) cardToMove.Rank == (int) targetCard.Rank - 1;

                return isDifferentColor && isDescendingOrder;
            }
            else if(targetPile.Type == PileType.Foundation)
            {
                if(draggetCardsCount > 1) return false;

                if(targetPile.GetPileCount() == 0)
                {
                    return cardToMove.Rank == Rank.Ace && cardToMove.Suit == targetPile.Suit; // se o tableau esta vazio, so aceita um as do nipe do tableu
                }

                if(targetPile.GetPileCount() >= System.Enum.GetValues(typeof(Rank)).Length) 
                {
                    return false; // pilha cheia não cabe mais nada
                }

                CardModel targetCard = targetPile.GetLastCard().Presenter.Model;

                bool isSameSuit = cardToMove.Suit == targetPile.Suit;
                bool isAscendingOrder = (int) cardToMove.Rank == (int) targetCard.Rank + 1;

                return isSameSuit && isAscendingOrder;
            }

            return false;
        }

        public static PileView GetValidFoundation(CardModel cardModel, List<PileView> foundations)
        {
            foreach (var foundation in foundations)
            {
                if (IsValidMove(cardModel, foundation, 1))
                {
                    return foundation;
                }
            }
            return null;
        }


    }
}