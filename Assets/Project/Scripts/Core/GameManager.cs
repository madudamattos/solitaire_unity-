using Solitaire.Models;
using Solitaire.Views;
using Solitaire.Logic;
using Solitaire.Factories;
using Solitaire.Managers;
using Solitaire.Core;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [HideInInspector][Header("Game State Control")]
    public static GameManager Instance { get; private set; }
    public GameState CurrentState {get; private set;}
    public event Action<GameState> OnStateChanged;

    [Header("Managers")]
    [SerializeField] private DeckFactory _deckFactory;
    [SerializeField] private BoardManager _boardManager;

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
        CommandManager.ClearHistory();
        
        // criação das cartas
        List<CardModel> deckModels = DeckGenerator.CreateFullDeck();
        
        List<CardView> cardViews = _deckFactory.CreateDeck(deckModels);

        // organizar cartas em pilhas 
        Dealer dealer = new Dealer();
        dealer.Deal(cardViews, _boardManager.tableauPiles, _boardManager.stockPile, () => ChangeState(GameState.Playing));
    }       

    public void UI_StartGame() => ChangeState(GameState.Dealing);
    public void UI_ReturnToMenu() => ChangeState(GameState.Menu);
}
