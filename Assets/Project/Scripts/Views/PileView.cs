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

        public Vector3 GetNextCardPositionDeal()
        {
            if(Type == PileType.Tableau)
            {
                float offset = 0.45f;

                // offset vertical para as cartas do tableau aparecerem cascateadas
                float yOffset = CardsInPile.Count * - offset;
                return transform.position + new Vector3(0, yOffset, 0); 
            }
            return transform.position;
        }
        
        public Vector3 GetNextCardPosition()
        {
            if(Type == PileType.Tableau)
            {
                // Se a pilha estiver vazia, retorna a âncora principal da pilha
                if (CardsInPile.Count == 0)
                    return transform.position;
                
                float offset = 0.45f;
                if(CardsInPile.Count > 1)
                {
                    if(GetLastCard().Presenter.Model.IsFaceUp)
                        offset = 0.95f;
                }

                // Pega a posição real na tela da última carta e desce a partir dela
                Vector3 lastCardPos = CardsInPile.Last().transform.position;
                
                return lastCardPos + new Vector3(0, -offset, 0); 
            }
            else if(Type == PileType.Waste)
            {
                if (CardsInPile.Count == 0)
                    return transform.position;

                float offset = 0.95f;

                // Pega a posição real na tela da última carta e desce a partir dela
                Vector3 lastCardPos = CardsInPile.Last().transform.position;
                
                return lastCardPos + new Vector3(offset, 0 , 0); 
            }
            
            // Para Stock e Foundations, todas as cartas ficam na posição exata da pilha
            return transform.position;
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

            float xOffset = 1.0f;

            for(int i = count - 1; i >= 0; i--) 
            {
                CardView card = CardsInPile[i];
                Vector3 targetPos = transform.position;

                if(i >= count - 3) 
                {
                    // inverte o indice para o slot visual (0, 1, 2)
                    int slotPos = i - Mathf.Max(0, count - 3); 
                    targetPos.x += slotPos * xOffset;
                }

                card.MoveTo(targetPos, 0.2f, 0f);
                card.SetSortingOrder(i);
            }
        }

    }
}
