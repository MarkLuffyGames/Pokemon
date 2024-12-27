using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMoveUI : MonoBehaviour
{
    [SerializeField] private Image pokemonImage;
    [SerializeField] private Image barSelector;
    [SerializeField] private Vector3 barSelectorOffset;
    [SerializeField] private TextMeshProUGUI pokemonNameText;
    [SerializeField] private TextMeshProUGUI movePowerText;
    [SerializeField] private TextMeshProUGUI moveAccuracyText;
    [SerializeField] private TextMeshProUGUI moveDescriptionText;
    [SerializeField] private List<TextMeshProUGUI> moveNameText;
    [SerializeField] private List<TextMeshProUGUI> movePpText;
    [SerializeField] private List<Move> moveList;

    private int moveSelected;

    public void SetMovementData(Pokemon pokemon, Move moveToLearn)
    {
        moveList = new List<Move>(pokemon.Moves)
        {
            moveToLearn
        };
        for (int i = 0; i < moveList.Count; i++)
        {
            moveNameText[i].text = moveList[i].MoveBase.MoveName;
            movePpText[i].text = $"pp{moveList[i].PowerPoints}/{moveList[i].MoveBase.PP}";
        }
        pokemonImage.sprite = pokemon.Base.FrontSprite;
        pokemonNameText.text = pokemon.Base.PokemonName;
        moveSelected = 0;
        UpdateMovementData();
    }

    private void UpdateMovementData()
    {
        barSelector.transform.SetParent(moveNameText[moveSelected].transform, false);
        barSelector.transform.localPosition = barSelectorOffset;
        movePowerText.text = moveList[moveSelected].MoveBase.Power.ToString();
        moveAccuracyText.text = moveList[moveSelected].MoveBase.Accuracy.ToString();
        moveDescriptionText.text = moveList[moveSelected].MoveBase.Description;
        
    }

    public void ChangeMoveSelected(int moveSelected)
    {
        this.moveSelected = moveSelected;
        UpdateMovementData();
    }
}
