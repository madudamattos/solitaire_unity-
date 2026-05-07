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
    public event Action<bool> OnAutoCompleteAvailable;

    [Header("Managers")]
    [SerializeField] private DeckFactory _deckFactory;
    [SerializeField] private BoardManager _boardManager;

    private void OnEnable()
    {
        MoveExecutor.OnBoardStateChanged += EvaluateWinCondition;        
    }

    private void OnDisable()
    {
        MoveExecutor.OnBoardStateChanged -= EvaluateWinCondition;     
    }
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
        dealer.Deal(cardViews, _boardManager._tableauPiles, _boardManager._stockPile, () => StartGame());
    } 

    private void EvaluateWinCondition()
    {
        if(CurrentState != GameState.Playing) return;

        if(WinValidator.CheckForVictory(_boardManager._foundationPiles))
        {
            TriggerVictory();
            return;
        }

        bool canAuto = WinValidator.CanAutoComplete(_boardManager._tableauPiles, _boardManager._stockPile, _boardManager._wastePile);
        OnAutoCompleteAvailable?.Invoke(canAuto);
    }      

    private void TriggerVictory()
    {
        ChangeState(GameState.GameOver);
        Debug.Log("Congratulations! You won.");
    }

    public void StartAutoComplete()
    {
        ChangeState(GameState.AutoComplete);
        CommandManager.ClearHistory();

        _boardManager.RunAutoComplete();
    }

    public void StartGame() => ChangeState(GameState.Playing);
    public void StartDeal() => ChangeState(GameState.Dealing);
    public void ReturnToMenu() => ChangeState(GameState.Menu);
}
