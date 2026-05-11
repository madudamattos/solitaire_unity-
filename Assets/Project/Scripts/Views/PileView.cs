using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Solitaire.Core;

namespace Solitaire.Views
{
    public class PileView : MonoBehaviour
    {
        [SerializeField] public PileType Type;

        [SerializeField] public Suit Suit;

        // lista visual das cartas que estão filhas desta pilha 
        private List<CardView> CardsInPile = new List<CardView>();

        public Vector3 GetNextCardPosition()
        {
            
            if(Type == PileType.Foundation || Type == PileType.Stock || CardsInPile.Count <= 0 ) return transform.position;

            float offset = 0.92f;

            float yOffset = 0;
            float zOffset = 0;

            if (Type == PileType.Waste){
                yOffset = -offset;
                return CardsInPile.Last().transform.position + new Vector3(0, yOffset, zOffset); 
            }

            // offset vertical para as cartas do tableau aparecerem cascateadas
            for(int i=0; i<CardsInPile.Count; i++)
            {
                yOffset -= CardsInPile[i].Presenter.Model.IsFaceUp? offset : offset/2;
            }

            return transform.position + new Vector3(0, yOffset, zOffset); 

        }

        public void AddCard(CardView card)
        {
            CardsInPile.Add(card);
        }

        public CardView GetLastCard()
        {
            return CardsInPile.Last();
        }

        public int GetPileCount()
        {
            return CardsInPile.Count;
        }

        public List<CardView> GetCardsInPile()
        {
            return CardsInPile;
        }

        public void RemoveCard(CardView card)
        {
            if(CardsInPile.Contains(card))
                CardsInPile.Remove(card);
        }

        public List<CardView> GetCardsFrom(CardView startCard)
        {
            int startIndex = CardsInPile.IndexOf(startCard); 

            // Retorna uma lista vazia caso a carta não seja encontrada (fallback de segurança)
            if (startIndex == -1 ) return new List<CardView>();
            
            return CardsInPile.GetRange(startIndex, CardsInPile.Count - startIndex);
        }


        public void UpdateWasteVisuals()
        {
            if(Type != PileType.Waste) return;

            int count = CardsInPile.Count;
            if(count == 0) return;

            float yOffset = 0.92f;
            float zOffset = 0.01f;

            for(int i = count - 1; i >= 0; i--) 
            {
                CardView card = CardsInPile[i];
                Vector3 targetPos = transform.position;

                if(i >= count - 3) 
                {
                    // inverte o indice para o slot visual (0, 1, 2)
                    int slotPos = i - Mathf.Max(0, count - 3); 
                    targetPos.y -= slotPos * yOffset;
                    targetPos.z -= slotPos *zOffset;
                }

                card.MoveTo(targetPos, 0.2f, 0f);
                card.SetSortingOrder(i);
            }
        }

    }
}
