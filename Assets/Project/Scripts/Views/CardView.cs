using UnityEngine;
using DG.Tweening; // Usando DOTween para o movimento 
using Solitaire.Presenters;

namespace Solitaire.Views 
{
    public class CardView : MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D collider;

        [Header("On initialization references")]
        [SerializeField] private Sprite cardFront;
        [SerializeField] private Sprite cardBack;
        
        private CardPresenter _presenter;

        public CardPresenter Presenter => _presenter;
         public void Setup(Sprite front, Sprite back)
        {
            cardFront = front; 
            cardBack = back;
        }

        public void Bind(CardPresenter presenter) { _presenter = presenter; }
        public void SetFaceUp(bool isFaceUp){ spriteRenderer.sprite = isFaceUp ? cardFront : cardBack; }
        public void RequestFlip() { _presenter?.FlipCard(); }

        public void SetCollider(bool enable)
        {
            collider.enabled = enable;
        }
        public void SetSortingOrder(int order)
        {
            spriteRenderer.sortingOrder = order;
        }

        public void MoveTo(Vector3 targetPosition, float duration, float delay, System.Action onComplete = null)
        {
            // O método DOMove realiza a interpolação do transform.position
            transform.DOMove(targetPosition, duration)
                     .SetDelay(delay)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() => onComplete?.Invoke());
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();

            transform.DOKill();
        }
    }
}
