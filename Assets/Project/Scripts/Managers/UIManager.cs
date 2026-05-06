using UnityEngine;
using Solitaire.Core;
using Solitaire.Managers;

public class UIManager: MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _gameTablePanel;
    [SerializeField] private GameObject _victoryPanel;
    
    [Header("Buttons")]
    [SerializeField] private GameObject _autoCompleteButton;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += HandleStateChange;
        GameManager.Instance.OnAutoCompleteAvailable += ToggleAutoCompleteButton;
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChange;
            GameManager.Instance.OnAutoCompleteAvailable -= ToggleAutoCompleteButton;
        }
                
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