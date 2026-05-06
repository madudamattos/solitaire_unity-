using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Solitaire.Models;
using Solitaire.Logic;
using Solitaire.Core;
using Solitaire.Views;
using Solitaire.Managers;

namespace Solitaire.Input
{
    public class CardInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Visual References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider; 
        [SerializeField] private CardView _cardView;

        [Header("Physics")]
        [SerializeField] private LayerMask _dropLayerMask;
        private Camera _mainCamera;
        
        [Header("Variables")]
        public PileView CurrentPile { get; set; }
        private List<CardView> _draggedCards = new List<CardView>();
        private List<Vector3> _offsets = new List<Vector3>();
        private List<Vector3> _originalPositions = new List<Vector3>();
        private List<int> _originalSortingOrders = new List<int>();

        // controle de estado
        private bool _isDragging = false;
        private int _currentPointerId;

        private void Awake()
        {
            _mainCamera = Camera.main;

            if(_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if(_collider == null)
                _collider = GetComponentInChildren<Collider2D>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(_isDragging) return;
            if(!MoveValidator.CanDrag(_cardView, CurrentPile)) return;
            if (CommandManager.IsProcessing) return;
            
            _isDragging = true;
            _currentPointerId = eventData.pointerId;

            // Obtem todas as cartas a partir da carta clicada
            if(CurrentPile.Type == PileType.Tableau)
            {
                _draggedCards = CurrentPile.GetCardsFrom(_cardView);
            }
            else
            {
                _draggedCards = new List<CardView>();
                _draggedCards.Add(_cardView);
            }

            _offsets.Clear();
            _originalPositions.Clear();
            _originalSortingOrders.Clear();

            Vector3 mouseWorldPos = GetMouseWorldPosition(eventData.position);
        
            for(int i=0; i< _draggedCards.Count; i++)
            {
                CardView card = _draggedCards[i];
                Transform cardTransform = card.transform;
                SpriteRenderer cardRenderer = card.GetComponentInChildren<SpriteRenderer>();
                Collider2D cardCollider = card.GetComponentInChildren<Collider2D>();

                _originalPositions.Add(cardTransform.position);
                _offsets.Add(cardTransform.position - mouseWorldPos);
                _originalSortingOrders.Add(cardRenderer.sortingOrder);
                cardRenderer.sortingOrder = 100 + i;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(!_isDragging || eventData.pointerId != _currentPointerId) return;
            Vector3 mouseWorldPos = GetMouseWorldPosition(eventData.position);

            // Aplica offset individualmente em cada carta 
            for(int i=0; i<_draggedCards.Count; i++)
            {
                _draggedCards[i].transform.position = mouseWorldPos + _offsets[i];
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(!_isDragging || eventData.pointerId != _currentPointerId) return;

            _isDragging = false;

            // Dispara um raio na posiçao do mouse para ver o que atingimos
            Vector3 mousePos = GetMouseWorldPosition(eventData.position);
            Collider2D hit = Physics2D.OverlapPoint(mousePos, _dropLayerMask);

            // Verifica se atingimos algo e se esse algo possui o PileView
            if(hit != null && hit.TryGetComponent(out PileView targetPile))
            {   
                CardModel modelToMove = _cardView.Presenter.Model; 

                if(MoveValidator.IsValidMove(modelToMove, targetPile, _draggedCards.Count))
                {
                    ICommand moveCommand = new MoveCardsCommand(_draggedCards, CurrentPile, targetPile);
                    CommandManager.AddCommand(moveCommand);
                }
                else
                {
                    ReturnCardsToOriginalPositions();
                }
            }
            else
            {
                ReturnCardsToOriginalPositions();
            }

            _draggedCards.Clear();
        }
        
        private void ReturnCardsToOriginalPositions()
        {
            for(int i=0; i< _draggedCards.Count; i++)
            {
                _draggedCards[i].transform.position = _originalPositions[i];
                _draggedCards[i].GetComponentInChildren<SpriteRenderer>().sortingOrder = _originalSortingOrders[i]; 
            }
        }

        private Vector3 GetMouseWorldPosition(Vector2 screenPosition)
        {
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;
            return worldPosition;
        }
    }
}