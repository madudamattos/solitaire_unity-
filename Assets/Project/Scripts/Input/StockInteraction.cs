using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Solitaire.Views;

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
            // Evita erro se o stock tiver menos cartas do que a quantidade desejada de compra 
            int amountToDraw = Mathf.Min(maxAmount, _stockPile.GetPileCount());

            for(int i=0; i<amountToDraw; i++)
            {
                CardView card = _stockPile.GetLastCard();

                _stockPile.RemoveCard(card);
                _wastePile.AddCard(card);

                card.SetSortingOrder(_wastePile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = _wastePile;

                card.transform.SetParent(_wastePile.transform);

                if(!card.Presenter.Model.IsFaceUp) card.RequestFlip();
            }
            
            _wastePile.UpdateWasteVisuals();
        }   

        private void RecycleWaste()
        {
            int count = _wastePile.GetPileCount();

            for(int i=0; i<count; i++)
            {
                CardView card = _wastePile.GetLastCard();
                _wastePile.RemoveCard(card);
                _stockPile.AddCard(card);

                card.transform.position = _stockPile.transform.position;
                card.transform.SetParent(_stockPile.transform);

                card.SetSortingOrder(_stockPile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = _stockPile;
                
                card.RequestFlip();
            }
        }


    }
}