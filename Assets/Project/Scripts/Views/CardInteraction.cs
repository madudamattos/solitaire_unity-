using UnityEngine;
using UnityEngine.EventSystems;
using Solitaire.Presenters;
using Solitaire.Models;
using Solitaire.Logic;

namespace Solitaire.Views
{
    public class CardInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Visual References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider; 
        [SerializeField] private CardView _cardView;

        [SerializeField] private LayerMask _dropLayerMask;
        private Camera _mainCamera;
        
        [Header("Variables")]
        private Vector3 _offset;
        private Vector3 _originalPosition;
        private int _originalSortingOrder;
        public PileView CurrentPile { get; set; }

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
            _originalPosition = transform.position;

            // _collider.enabled = false;

            Vector3 mouseWorldPos = GetMouseWorldPosition(eventData.position);
            _offset = transform.position - mouseWorldPos;

            _originalSortingOrder = _spriteRenderer.sortingOrder;
            _spriteRenderer.sortingOrder = 100;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = GetMouseWorldPosition(eventData.position) + _offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // _collider.enabled = true;
            _spriteRenderer.sortingOrder = _originalSortingOrder;

            // Dispara um raio na posiçao do mouse para ver o que atingimos
            Vector3 mousePos = GetMouseWorldPosition(eventData.position);
            Collider2D hit = Physics2D.OverlapPoint(mousePos, _dropLayerMask);

            // Verifica se atingimos algo e se esse algo possui o PileView
            if(hit != null && hit.TryGetComponent(out PileView targetPile))
            {   
                CardModel modelToMove = _cardView.Presenter.Model; 

                if(MoveValidator.IsValidMove(modelToMove, targetPile))
                {
                    // atualiza a logica de posicionamento da carta nas pilhas
                    CurrentPile.RemoveCard(_cardView);
                    targetPile.AddCard(_cardView);
                    
                    // vira a ultima carta da pilha antiga 
                    if(CurrentPile.Type == PileView.PileType.Tableau && CurrentPile.GetPileCount() > 0)
                    {
                        CardView cardLeftBehind = CurrentPile.GetLastCard();
                        if(cardLeftBehind.Presenter.Model.IsFaceUp == false)
                            cardLeftBehind.RequestFlip();
                    }

                    // atualiza o visual
                    transform.position = targetPile.GetNextCardPosition();
                    transform.SetParent(targetPile.transform);
                    _cardView.SetSortingOrder(targetPile.GetPileCount());

                    // atualiza current pile para a pilha nova
                    CurrentPile = targetPile;
                    Debug.Log("Achou posição certa. Atualizando jogo");
                }
                else
                {
                    Debug.Log("Encontrou pile view mas a posicao nao é valida");
                    transform.position = _originalPosition;
                }
            }
            else
            {
                transform.position = _originalPosition;
                Debug.Log("Não encontrou pile view, voltando para a posicao original");
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