using UnityEngine;
using System.Collections.Generic;
using Solitaire.Views;

public class BoardManager : MonoBehaviour
{
    [Header("Structures")]
    public PileView stockPile;
    public List<PileView> foundationPiles;
    public List<PileView> tableauPiles;
    public void ClearBoard()
    {
        // Lógica para limpar listas das pilhas...
    }
}