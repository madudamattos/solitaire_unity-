using Solitaire.Models;
using Solitaire.Views;

namespace Solitaire.Logic
{
    public static class MoveValidator
    {
        public static bool IsValidMove(CardModel cardToMove, PileView targetPile)
        {
            // Regra 1 : Soltou a carta no tableau
            if(targetPile.Type == PileView.PileType.Tableau)
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

            return false;
        }
    }
}