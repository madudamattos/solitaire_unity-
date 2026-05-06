using UnityEngine;
using Solitaire.Core;
using Solitaire.Managers;

public class UIManager: MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _gameTablePanel;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += HandleStateChange;
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChange;        
    }

    private void HandleStateChange(GameState state)
    {
        _menuPanel.SetActive(state == GameState.Menu);
        _gameTablePanel.SetActive(state == GameState.Dealing || state == GameState.Playing);
    }

    public void UI_UndoLastMove()
    {
        CommandManager.UndoLastCommand();
    }
}