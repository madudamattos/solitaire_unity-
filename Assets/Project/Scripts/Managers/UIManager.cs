using UnityEngine;
using Solitaire.Core;
using Solitaire.Managers;
using Solitaire.Logic;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager: MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _gameTablePanel;
    [SerializeField] private GameObject _victoryPanel;
    
    [Header("Buttons")]
    [SerializeField] private GameObject _autoCompleteButton;
    [SerializeField] private Button _undoButton;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private TextMeshProUGUI _timerText;

    private void OnEnable()
    {
        GameManager.Instance.OnStateChanged += HandleStateChange;
        GameManager.Instance.OnAutoCompleteAvailable += ToggleAutoCompleteButton;
        GameManager.Instance.OnMovesChanged += UpdateMovesText;
        GameManager.Instance.OnTimeChanged += UpdateTimeText;
        CommandManager.OnCommandExecuted += UpdateUndoButton;
        CommandManager.OnCommandUndone += UpdateUndoButton;
        CommandManager.OnHistoryCleared += UpdateUndoButton;
    }

    public void Start()
    {
        UpdateUndoButton();
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChange;
            GameManager.Instance.OnAutoCompleteAvailable -= ToggleAutoCompleteButton;
            GameManager.Instance.OnMovesChanged -= UpdateMovesText;
            GameManager.Instance.OnTimeChanged -= UpdateTimeText;
        }    

        CommandManager.OnCommandExecuted -= UpdateUndoButton;
        CommandManager.OnCommandUndone -= UpdateUndoButton;
        CommandManager.OnHistoryCleared -= UpdateUndoButton;
    }

    private void HandleStateChange(GameState state)
    {
        _menuPanel.SetActive(state == GameState.Menu);
        _gameTablePanel.SetActive(state == GameState.Dealing || state == GameState.Playing);
        _victoryPanel.SetActive(state == GameState.GameOver);

        if (state != GameState.Playing)
            _autoCompleteButton.SetActive(false);
    }

    private void ToggleAutoCompleteButton(bool isAvailable)
    {
        _autoCompleteButton.SetActive(isAvailable);
    }

    private void UpdateUndoButton()
    {
        _undoButton.interactable = CommandManager.HasCommands;
    }

    private void UpdateMovesText(int moves)
    {
        _movesText.text = moves.ToString();
    }

    private void UpdateTimeText(int seconds)
    {
        // Formata os segundos totais para o padrão MM:SS
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        _timerText.text = time.ToString(@"hh\:mm\:ss");
    }
    

    public void UI_StartGame()
    {
        GameManager.Instance.StartDeal();
    }

    public void UI_ReturnToMenu()
    {
        GameManager.Instance.ReturnToMenu();
    }

    public void UI_TriggerAutoComplete()
    {
        GameManager.Instance.StartAutoComplete();
    }

    public void UI_UndoLastMove()
    {
        CommandManager.UndoLastCommand();
    }

}