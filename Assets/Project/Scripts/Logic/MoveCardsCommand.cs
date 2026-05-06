using System.Collections.Generic;
using Solitaire.Views;
using Solitaire.Managers;

namespace Solitaire.Logic
{
    public class MoveCardsCommand : ICommand
    {
        private List<CardView> _cardsMoved;
        private PileView _sourcePile;
        private PileView _targetPile;
        private bool _flippedSourceCard;

        public MoveCardsCommand(List<CardView> cardsMoved, PileView source, PileView target)
        {
            _cardsMoved = new List<CardView>(cardsMoved);
            _sourcePile = source;
            _targetPile = target;
        }

        public void Execute()
        {
            _flippedSourceCard = MoveExecutor.ExecuteMove(_cardsMoved, _sourcePile, _targetPile);
        }

        public void Undo()
        {
            MoveExecutor.UndoMove(_cardsMoved, _sourcePile, _targetPile, _flippedSourceCard);
        }

    }
}