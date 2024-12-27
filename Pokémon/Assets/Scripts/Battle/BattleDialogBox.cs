using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [Tooltip("Dialgo que describe lo que va pasando en el combate.")]
    [SerializeField] private TextMeshProUGUI dialogText;
    [Tooltip("Dialgo de la BattleAction.")]
    [SerializeField] private TextMeshProUGUI dialogActionText;
    [Tooltip("Cantidad de letras que se muestran por segundo en el dialogo.")]
    [SerializeField] private float textSpeed = 10.0f;

    [SerializeField] private GameObject actionBox;
    [SerializeField] private GameObject movesBox;
    [SerializeField] private GameObject answerBox;

    [SerializeField] private List<TextMeshProUGUI> actionsTexts;
    [SerializeField] private List<TextMeshProUGUI> movementTexts;
    [SerializeField] private List<TextMeshProUGUI> answerTexts;

    [SerializeField] private TextMeshProUGUI ppText;
    [SerializeField] private TextMeshProUGUI typeText;

    public void StartBattle()
    {
        ToggleDialogText(true);
        ToggleActionBox(false);
        ToggleMovesBox(false);
        ToggleAnswerBox(false);
    }
    public IEnumerator SetDialog(string message, float textSpeed = 0)
    {
        ToggleDialogText(true);
        ToggleActionBox(false);
        ToggleMovesBox(false);
        ToggleAnswerBox(false);

        dialogText.text = "";
        foreach (var item in message)
        {
            dialogText.text += item;
            yield return new WaitForSeconds(1 / (textSpeed == 0 ? this.textSpeed : textSpeed));
        }

        yield return new WaitForSeconds(1);
    }
    public void SetDialogActionText(string message)
    {
        ToggleDialogText(false);
        ToggleMovesBox(false);
        ToggleActionBox(true);
        ToggleAnswerBox(false);

        dialogActionText.text = message;
    }

    public void ToggleDialogText(bool activated)
    {
        dialogText.enabled = activated;
    }

    public void ToggleActionBox(bool activated)
    {
        actionBox.SetActive(activated);
    }

    public void ToggleMovesBox(bool activated)
    {
        movesBox.SetActive(activated);
    }

    public void ToggleAnswerBox(bool activated)
    {
        answerBox.SetActive(activated);
    }

    public void SelectedAction(int selectAction)
    {
        for (int i = 0; i < actionsTexts.Count; i++)
        {
            actionsTexts[i].GetComponentInChildren<Image>().enabled = i == selectAction ? true : false;
        }
    }

    public void SetPokemonMovement(Pokemon pokemon)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].text = "-";
        }

        for (int i = 0; i < pokemon.Moves.Count; i++)
        {
            movementTexts[i].text = pokemon.Moves[i].MoveBase.MoveName;
        }
    }

    public void SelectedMovement(int selectMovement, Move movement)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].GetComponentInChildren<Image>().enabled = i == selectMovement ? true : false;
        }

        ppText.text = $"{movement.PowerPoints}/{movement.MoveBase.PP}";
        typeText.text = $"Type: {movement.MoveBase.MoveType}";
    }

    public void SelectAnswer(int selectedAnswer)
    {
        for (int i = 0; i < answerTexts.Count; i++)
        {
            answerTexts[i].GetComponentInChildren<Image>().enabled = i == selectedAnswer ? true : false;
        }
    }
}
