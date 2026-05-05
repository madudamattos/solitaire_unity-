using System.Linq;
using Solitaire.Models;
using Solitaire.Views;
using Solitaire.Logic;
using Solitaire.Presenters;
using System.Collections.Generic;
using UnityEngine;
using Solitaire.Core;
using System;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private DeckData deckData;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Back cardsBack = Back.Blue;

    [Header("Structures")]
    [SerializeField] private List<PileView> tableauPiles;
    [SerializeField] private PileView stockPile; 

    [HideInInspector][Header("Variables")]
    private List<CardPresenter> _cardPresenters = new List<CardPresenter>();
    private List<CardView> _cardViews = new List<CardView>();

    [HideInInspector][Header("Game State Control")]
    public static GameManager Instance { get; private set; }
    public GameState CurrentState {get; private set;}
    public event Action<GameState> OnStateChanged;

    public GameObject menu;
    public GameObject gameTable;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);

        switch(CurrentState)
        {
            case GameState.Menu:
                // ativa o menu, desativa o jogo
                break;
            case GameState.Dealing:
                menu.SetActive(false);
                gameTable.SetActive(true);
                InitializeGame();
                break;
            case GameState.Playing:
                // Gameplat liberado
                break;
            // ...
        }
    }

    private void InitializeGame()
    {
        // criação das cartas
        List<CardModel> deckModels = DeckGenerator.CreateFullDeck();
        
        foreach (var model in deckModels)
        {
            GameObject cardObj = Instantiate(cardPrefab, transform.position, Quaternion.identity);

            CardView view = cardObj.GetComponent<CardView>();
            _cardViews.Add(view);
            
            Sprite cardSprite = GetSpriteFrontForCard(model.Rank, model.Suit);
            Sprite cardBackSprite = GetSpriteBackForCard(cardsBack);
            view.Setup(cardSprite, cardBackSprite);
        
            CardPresenter presenter = new CardPresenter(model, view);
            _cardPresenters.Add(presenter);
        }

        // organizar cartas em pilhas 
        Dealer dealer = new Dealer();
        dealer.Deal(_cardViews, tableauPiles, stockPile, () => ChangeState(GameState.Playing));
    }       
    
    private Sprite GetSpriteFrontForCard(Rank rank, Suit suit)
    {
        return deckData.cards.Find(c => c.rank == rank && c.suit == suit).cardSprite;
    }

    private Sprite GetSpriteBackForCard(Back color)
    {
        return deckData.cardsBack.Find(c => c.color == color).cardBackSprite;
    }

    public void UI_StartGame() => ChangeState(GameState.Dealing);
    public void UI_ReturnToMenu() => ChangeState(GameState.Menu);
}
