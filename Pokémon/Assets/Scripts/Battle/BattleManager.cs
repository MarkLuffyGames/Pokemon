using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum BattleState
{
    Start,
    SelectPlayerAction,
    SelectPlayerMovemet,
    ExecuteActions,
    PlayerAction,
    RivalAction,
    SelectPokemon,
    Busy,
    EndBattle
}
public class BattleManager : MonoBehaviour
{
    [Tooltip("Unidad de batalla del jugador.")]
    [SerializeField] private BattleUnit playerUnit;
    [Tooltip("Unidad de batalla del rival.")]
    [SerializeField] private BattleUnit rivalUnit;
    [Tooltip("Caja de dialogo de la batalla.")]
    [SerializeField] private BattleDialogBox battleDialogBox;
    [Tooltip("Menu de seleccion de Pokemons")]
    [SerializeField] private PartyHUD playerPartyHUD;

    [SerializeField] private BattleState state;

    [SerializeField] private int currentSelectedAction;
    [SerializeField] private int currentSelectedMovement;
    [SerializeField] private int currentPokemonSelected;
    [SerializeField] private bool isBattlePokemon;

    private InputAction moveMenu;
    private InputAction select;
    private InputAction back;

    private bool isFainted;

    public event Action<bool> OnFinishedBattle;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;

    private List<Pokemon> playerPokemonList;


    private void Start()
    {
        moveMenu = InputSystem.actions.FindAction("MoveMenu");
        select = InputSystem.actions.FindAction("Select");
        back = InputSystem.actions.FindAction("Back");


        moveMenu.performed += MoveMenu_performed;
        select.started += Select_started;
        back.started += Back_started;
    }

    private void MoveMenu_performed(InputAction.CallbackContext obj)
    {
        if (moveMenu.WasPressedThisFrame())
        {
            var noveDir = OneDirectionMove.OneDirection(moveMenu.ReadValue<Vector2>().normalized,Vector3.zero);
            switch (state)
            {
                case BattleState.SelectPlayerAction:
                    HandlePlayerActionSelection(noveDir);
                    break;
                case BattleState.SelectPlayerMovemet:
                    HandlePlayerMovementSelection(noveDir);
                    break;
                case BattleState.SelectPokemon:
                    HandleSelectPokemon(noveDir);
                    break;
                default:
                    break;
            }
        }
        
    }
    private void Select_started(InputAction.CallbackContext obj)
    {
        switch (state)
        {
            case BattleState.SelectPlayerAction:
                SelectAction();
                break;
            case BattleState.SelectPlayerMovemet:
                SelectMovement();
                break;
            case BattleState.SelectPokemon:
                StartCoroutine(SwitchPokemon());
                break;
            default:
                break;
        }
    }

    private void Back_started(InputAction.CallbackContext obj)
    {
        switch (state)
        {
            case BattleState.SelectPlayerMovemet:
                PlayerActionSelect();
                break;
            case BattleState.SelectPokemon:
                if(playerUnit.pokemon.HP != 0) PlayerActionSelect();
                break;
            default:
                break;
        }
    }

    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.Start;

        currentSelectedAction = 0;
        currentSelectedMovement = 0;

        battleDialogBox.StartBattle();

        playerUnit.SetupPokemon(playerParty.GetFirstNoFaintedPokemon());
        playerPokemonList = new List<Pokemon>(playerParty.Pokemons);
        rivalUnit.SetupPokemon(wildPokemon);

        battleDialogBox.SetPokemonMovement(playerUnit.pokemon);


        yield return battleDialogBox.SetDialog($"Un {rivalUnit.pokemon.Base.PokemonName} salvaje aparecio.");
        yield return new WaitForSeconds(1);
        PlayerActionSelect();
    }

    private void EndBattle(bool playerWasWon)
    {
        state = BattleState.EndBattle;
        OnFinishedBattle(playerWasWon);
    }

    private void PlayerActionSelect()
    {
        state = BattleState.SelectPlayerAction;

        playerPartyHUD.gameObject.SetActive(false);
        
        battleDialogBox.SetDialogActionText($"¿Que deberia hacer {playerUnit.pokemon.Base.PokemonName}?");
        battleDialogBox.SelectedAction(currentSelectedAction);

    }

    private void PlayerMovementSelect()
    {
        state = BattleState.SelectPlayerMovemet;

        battleDialogBox.ToggleMovesBox(true);
        battleDialogBox.SelectedMovement(currentSelectedMovement, playerUnit.pokemon.Moves[currentSelectedMovement]);
    }

    private void SelectingPokemon()
    {
        state = BattleState.SelectPokemon;
        currentPokemonSelected = 0;
        isBattlePokemon = true;
        playerPartyHUD.gameObject.SetActive(true);
        playerPartyHUD.SetPartyData(playerPokemonList);
    }

    private void HandlePlayerActionSelection(Vector2 moveDir)
    {
        if (moveMenu.ReadValue<Vector2>() == Vector2.zero) return;

        Selector(ref currentSelectedAction, 3, moveDir);

        battleDialogBox.SelectedAction(currentSelectedAction);

    } 

    private void SelectAction()
    {
        if (currentSelectedAction == 0)
        {
            PlayerMovementSelect();
        }
        else if (currentSelectedAction == 1)
        {
            StartCoroutine(ThrowPokeball());
        }
        else if (currentSelectedAction == 2)
        {
            SelectingPokemon();
        }
        else if (currentSelectedAction == 3)
        {
            EndBattle(false);
        }
    }

    private void HandlePlayerMovementSelection(Vector2 moveDir)
    {
        if (moveMenu.ReadValue<Vector2>() == Vector2.zero) return;

        Selector(ref currentSelectedMovement, playerUnit.pokemon.Moves.Count -1, moveDir);

        battleDialogBox.SelectedMovement(currentSelectedMovement, playerUnit.pokemon.Moves[currentSelectedMovement]);

    }

    private void SelectMovement()
    {
        if (playerUnit.pokemon.Moves[currentSelectedMovement].PowerPoints > 0)
        {
            StartCoroutine(ExecuteMovement());
        }
        else
        {
            //No quedan PP.
        }
    }

    private void Selector( ref int index, int positionAmount, Vector2 moveDir)
    {
        if (moveDir == Vector2.up)
        {
            if (index == 2 || index == 3)
            {
                index -= 2;
            }
        }
        else if (moveDir == Vector2.down)
        {
            if (index == 0 || index == 1)
            {
                if(index + 2 <= positionAmount) index += 2;
            }
        }
        else if (moveDir == Vector2.left)
        {
            if (index == 1 || index == 3)
            {
                index -= 1;
            }
        }
        else if (moveDir == Vector2.right)
        {
            if (index == 0 || index == 2)
            {
                if (index + 1 <= positionAmount) index += 1;
            }
        }
    }

    private IEnumerator ExecuteMovement()
    {
        state = BattleState.ExecuteActions;

        battleDialogBox.ToggleActionBox(false);
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleMovesBox(false);

        bool playerFirst = false;

        if (playerUnit.pokemon.Speed > rivalUnit.pokemon.Speed)
        {
            playerFirst = true;
        }
        else if (playerUnit.pokemon.Speed < rivalUnit.pokemon.Speed)
        {
            playerFirst = false;
        }
        else
        {
            playerFirst = Random.Range(0, 2) == 0 ? true : false;
        }

        if(playerFirst)
        {
            yield return StartCoroutine(PlayerAttack());
            if(!isFainted) yield return StartCoroutine(RivalAttack());
        }
        else
        {
            yield return StartCoroutine(RivalAttack());
            if(!isFainted) yield return StartCoroutine(PlayerAttack());
        }

        if(!isFainted)
        {
            PlayerActionSelect();
        }
        else
        {
            isFainted = false;
            if(playerUnit.pokemon.HP == 0)
            {   
                if(playerParty.GetFirstNoFaintedPokemon() != null)
                {
                    SelectingPokemon();
                }
                else
                {
                    EndBattle(false);
                }
            }
            else
            {
                EndBattle(true);
            }
        }
        
    }

    private IEnumerator PlayerAttack()
    {
        state = BattleState.PlayerAction;

        if (playerUnit.pokemon.Moves[currentSelectedMovement].MoveBase.MoveClass != MoveClass.Status)
        {
            yield return StartCoroutine(DamageMove(
                playerUnit, rivalUnit, playerUnit.pokemon.Moves[currentSelectedMovement]));
        }
        else
        {
            // Ataques de Estado.
        }

    }

    private IEnumerator RivalAttack()
    {
        state = BattleState.RivalAction;

        Move randomMove = RandomMove(rivalUnit.pokemon.Moves);

        if (randomMove.MoveBase.MoveClass != MoveClass.Status)
        {
            yield return StartCoroutine(DamageMove(
                rivalUnit, playerUnit, randomMove));
        }
        else
        {
            // Ataques de Estado.
        }

    }

    private Move RandomMove(List<Move> moves)
    {
        int random = Random.Range(0, moves.Count);
        if (moves[random].PowerPoints > 0)
        {
            return moves[random];
        }
        else
        {
            bool remainPP = false;
            foreach (Move move in moves)
            {
                if(move.PowerPoints > 0)
                {
                    remainPP = true;
                    break;
                }
            }
            if (remainPP)
            {
                return RandomMove(moves);
            }
            
        }

        return null;
    }

    private IEnumerator DamageMove(BattleUnit attacker, BattleUnit defender, Move move)
    {
        move.PowerPoints--;

        var damageDescription = defender.pokemon.RecibeDamage(move, attacker.pokemon);
        var isfainted = damageDescription.IsFainted;

        yield return battleDialogBox.SetDialog(
            $"{attacker.pokemon.Base.PokemonName} a usado " +
            $"{move.MoveBase.MoveName}.");

        attacker.AnimationAttack();

        yield return new WaitForSeconds(0.5f);

        defender.AnimationRecibeDamage();

        yield return StartCoroutine(defender.BattleHUD.UpdateData(defender.pokemon.HP));

        if (damageDescription.type != "")
        {
            yield return battleDialogBox.SetDialog(
            $"¡El ataque es {damageDescription.type}!");
        }

        if (damageDescription.Critical)
        {
            yield return battleDialogBox.SetDialog(
            $"¡Ha sido un golpe crítico!");
        }

        if (isfainted)
        {
            this.isFainted = true;
            yield return battleDialogBox.SetDialog($"{defender.pokemon.Base.PokemonName} se a debilitado.");
            defender.AnimationFainted();
            yield return new WaitForSeconds(1.5f);
        }
    }


    
    private void HandleSelectPokemon(Vector2 moveDir)
    {
        if (moveDir == Vector2.up)
        {
            if(isBattlePokemon) currentPokemonSelected = 0;
            if(currentPokemonSelected == 0) isBattlePokemon = false;
            currentPokemonSelected--;
            if (currentPokemonSelected == 0) isBattlePokemon = true;
            if (currentPokemonSelected < 0) currentPokemonSelected = playerPokemonList.Count - 1;
        }
        else if (moveDir == Vector2.down)
        {
            if(isBattlePokemon) currentPokemonSelected = 0;
            if (currentPokemonSelected == 0) isBattlePokemon = false;
            currentPokemonSelected++;
            if (currentPokemonSelected > playerPokemonList.Count - 1) currentPokemonSelected = 0;
            if (currentPokemonSelected == 0) isBattlePokemon = true;
        }
        else if (moveDir == Vector2.left)
        {
            if(currentPokemonSelected != playerPokemonList.Count)
            {
                isBattlePokemon = true;
            }
            else
            {
                //Estoy en el boton exit.
            }
        }
        else if (moveDir == Vector2.right)
        {
            isBattlePokemon = false;
            if (currentPokemonSelected == 0) currentPokemonSelected++;
        }


        playerPartyHUD.SelectPokemon(isBattlePokemon ? 0 : currentPokemonSelected);
    }

    private IEnumerator SwitchPokemon()
    {
        if (playerPokemonList[currentPokemonSelected].HP > 0 && !isBattlePokemon)
        {

            state = BattleState.ExecuteActions;
            playerPartyHUD.gameObject.SetActive(false);
            battleDialogBox.ToggleDialogText(true);
            battleDialogBox.ToggleMovesBox(false);
            battleDialogBox.ToggleActionBox(false);
            if(playerUnit.pokemon.HP > 0)yield return battleDialogBox.SetDialog($"Vuelve {playerUnit.pokemon.Base.PokemonName}.");
            playerUnit.AnimationFainted();
            yield return new WaitForSeconds(1);
            yield return battleDialogBox.SetDialog($"Ve {playerPokemonList[currentPokemonSelected].Base.PokemonName}");
            playerUnit.SetupPokemon(playerPokemonList[currentPokemonSelected]);
            battleDialogBox.SetPokemonMovement(playerUnit.pokemon);
            ChangePokemon(currentPokemonSelected);
            PlayerActionSelect();
        }
    }

    private void ChangePokemon(int pokemonToChangeIndex)
    {
        Pokemon temp = playerPokemonList[pokemonToChangeIndex];
        playerPokemonList[pokemonToChangeIndex] = playerPokemonList[0];
        playerPokemonList[0] = temp;
    }

    [SerializeField] private GameObject pokeball;
    private IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        yield return battleDialogBox.SetDialog($"Lanzando una {pokeball.name}");

        var pokeballInstance = Instantiate(pokeball,playerUnit.transform.position + Vector3.left * 4, Quaternion.identity);

        var pokeballSpt = pokeballInstance.GetComponent<SpriteRenderer>();

        yield return pokeballSpt.transform.DOJump(rivalUnit.transform.position + Vector3.up * 1.5f, 1, 1, 1).WaitForCompletion();

        yield return rivalUnit.CapturedAnimation();

        yield return pokeballSpt.transform.DOMoveY(rivalUnit.transform.position.y - 1.5f, 0.4f).WaitForCompletion();

        int numberOfShakes = TryToCatchPokemon(rivalUnit.pokemon);

        for (int i = 0; i < Mathf.Min(numberOfShakes, 3); i++)
        {
            yield return new WaitForSeconds(1f);
            yield return pokeballSpt.transform.DOPunchRotation(new Vector3(0,0,20),1,5).WaitForCompletion();
        }

        if(numberOfShakes == 4)
        {
            yield return battleDialogBox.SetDialog($"¡Has atrapado un {rivalUnit.pokemon.Base.PokemonName}!");
            yield return new WaitForSeconds(1);

            Destroy(pokeballInstance);
            EndBattle(true);
        }
        else
        {
            Debug.Log("El pokemon a escapdo");
        }

    }

    private int TryToCatchPokemon(Pokemon pokemon)
    {
        int bonusBall = 1;
        int bonusStats = 1;

        float a = (3 * pokemon.maxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * bonusBall * bonusStats / (3 * pokemon.maxHP);

        Debug.Log(a);
        if (a > 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while (shakeCount < 4)
        {
            if(Random.Range(0, 65536) >= b) { break; }

            shakeCount++;
        }

        return shakeCount;
    }
}
