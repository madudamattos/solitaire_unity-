using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Solitaire.Views;
using Solitaire.Managers;
using Solitaire.Logic;
using System.Collections.Generic;

namespace Solitaire.Input
{
    [RequireComponent(typeof(PileView))]
    public class StockInteraction: MonoBehaviour, IPointerClickHandler
    {
        private PileView _stockPile;

        [Header("References")]
        [SerializeField] private PileView _wastePile;

        private  void Awake()
        {
            _stockPile = GetComponent<PileView>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_stockPile.GetPileCount() > 0)
            {
                DrawCards(1);
            }
            else if(_wastePile.GetPileCount() > 0)
            {
                RecycleWaste();
            }    

        }

        private void DrawCards(int maxAmount)
        {
            List<CardView> draggedCards = new List<CardView>();
            // Evita erro se o stock tiver menos cartas do que a quantidade desejada de compra 
            int amountToDraw = Mathf.Min(maxAmount, _stockPile.GetPileCount());

            List<CardView> stockCards = _stockPile.GetCardsInPile();

            for(int i=0; i<amountToDraw; i++)
            {
                CardView card = stockCards[stockCards.Count - 1 - i];
                draggedCards.Add(card);
            }

            ICommand moveCommand = new MoveCardsCommand(draggedCards, _stockPile, _wastePile);
            CommandManager.AddCommand(moveCommand);

            draggedCards.Clear();
        }    

        private void RecycleWaste()
        {
            List<CardView> wasteCards = _wastePile.GetCardsInPile();

            ICommand moveCommand = new MoveCardsCommand(wasteCards, _wastePile, _stockPile);
            CommandManager.AddCommand(moveCommand);
        }

    }
}