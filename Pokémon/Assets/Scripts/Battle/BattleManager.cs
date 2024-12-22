using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
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
                if(playerPokemonList[currentPokemonSelected].HP > 0 && !isBattlePokemon)
                {
                    if (playerUnit.pokemon.HP > 0)
                    {
                        StartCoroutine(ExecuteActions(2));
                    }
                    else
                    {
                        StartCoroutine(SwitchPokemon(true));
                    }
                }
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
            StartCoroutine(ExecuteActions(1));
        }
        else if (currentSelectedAction == 2)
        {
            SelectingPokemon();
        }
        else if (currentSelectedAction == 3)
        {
            StartCoroutine(ExecuteActions(3));
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
            StartCoroutine(ExecuteActions(0));
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

    private IEnumerator ExecuteActions(int actionTypeIndex)
    {
        state = BattleState.ExecuteActions;

        battleDialogBox.ToggleActionBox(false);
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleMovesBox(false);

        bool playerFirst = false;

        if(actionTypeIndex == 0)
        {
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
        }
        else
        {
            playerFirst = true;
        }
        

        if(playerFirst)
        {
            if(actionTypeIndex == 0)
            {
                yield return StartCoroutine(PlayerAttack());
            }
            else if(actionTypeIndex == 1)
            {
                yield return ThrowPokeball();
            }
            else if(actionTypeIndex == 2)
            {
                yield return SwitchPokemon(false);
            }
            else if(actionTypeIndex == 3)
            {
                yield return TryToScapeFromTheBattle(playerUnit.pokemon, rivalUnit.pokemon);
            }

            if (!isFainted && state == BattleState.PlayerAction) yield return StartCoroutine(RivalAttack());
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
                yield return GetExperience();

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
            yield return battleDialogBox.SetDialog(
            $"{playerUnit.pokemon.Base.PokemonName} a usado " +
            $"{playerUnit.pokemon.Moves[currentSelectedMovement].MoveBase.MoveName}.");
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
            yield return battleDialogBox.SetDialog(
            $"{rivalUnit.pokemon.Base.PokemonName} a usado " +
            $"{randomMove.MoveBase.MoveName}.");
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

        yield return battleDialogBox.SetDialog(
            $"{attacker.pokemon.Base.PokemonName} a usado " +
            $"{move.MoveBase.MoveName}.");

        attacker.AnimationAttack();

        yield return new WaitForSeconds(0.5f);

        defender.AnimationRecibeDamage();

        yield return StartCoroutine(defender.BattleHUD.UpdateHP(defender.pokemon.HP));

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

        if (damageDescription.IsFainted)
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

    private IEnumerator SwitchPokemon(bool currentPokemonIsFainted)
    {
        if (playerPokemonList[currentPokemonSelected].HP > 0 && !isBattlePokemon)
        {
            state = BattleState.PlayerAction;
            playerPartyHUD.gameObject.SetActive(false);
            battleDialogBox.ToggleDialogText(true);
            battleDialogBox.ToggleMovesBox(false);
            battleDialogBox.ToggleActionBox(false);
            if(playerUnit.pokemon.HP > 0)yield return battleDialogBox.SetDialog($"Vuelve {playerUnit.pokemon.Base.PokemonName}.");
            playerUnit.AnimationFainted();
            yield return new WaitForSeconds(1);
            yield return battleDialogBox.SetDialog($"Ve {playerPokemonList[currentPokemonSelected].Base.PokemonName}.");
            playerUnit.SetupPokemon(playerPokemonList[currentPokemonSelected]);
            battleDialogBox.SetPokemonMovement(playerUnit.pokemon);
            ChangePokemon(currentPokemonSelected);
            yield return new WaitForSeconds(1);
            if(currentPokemonIsFainted) PlayerActionSelect();
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
        state = BattleState.PlayerAction;

        yield return battleDialogBox.SetDialog($"Lanzando una {pokeball.name}.");

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

            if (playerParty.AddPokemonToParty(rivalUnit.pokemon))
            {
                yield return battleDialogBox.SetDialog($"{rivalUnit.pokemon.Base.PokemonName} se a agragado al equipo.");
            }
            else
            {
                yield return battleDialogBox.SetDialog($"{rivalUnit.pokemon.Base.PokemonName} se a enviado al PC.");
            }
            

            Destroy(pokeballInstance);
            EndBattle(true);
        }
        else
        {
            Destroy(pokeballInstance);
            yield return rivalUnit.BreakOutAnimation();

        }

    }

    private int TryToCatchPokemon(Pokemon pokemon)
    {
        int bonusBall = 1;
        int bonusStats = 1;

        float a = (3 * pokemon.maxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * bonusBall * bonusStats / (3 * pokemon.maxHP);

        if (a > 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        Debug.Log(b);

        int shakeCount = 0;

        for (int i = 0; i < 4; i++)
        {
            if(Random.Range(0, 65536) < b)
            {
                shakeCount++;
            }
        }

        return shakeCount;
    }

    private IEnumerator TryToScapeFromTheBattle(Pokemon escapingPokemon, Pokemon rivalPokemon)
    {
        state = BattleState.PlayerAction;

        float p = (escapingPokemon.Speed * 128 / rivalPokemon.Speed + 30);
        if(p >= 256)
        {
            yield return battleDialogBox.SetDialog("Has escapado con exito.");
            EndBattle(false);
        }
        else
        {
            int random = Random.Range(0, 256);

            if(random < p)
            {
                yield return battleDialogBox.SetDialog("Has escapado con exito.");
                EndBattle(false);
            }
            else
            {
                yield return battleDialogBox.SetDialog($"{escapingPokemon.Base.PokemonName} no pudo escapar de la batalla.");
            }
        }
    }

    private IEnumerator GetExperience()
    {
        int baseExp = rivalUnit.pokemon.Base.BaseExp;
        int rivalLevel = rivalUnit.pokemon.Level;

        // Cálculo base de experiencia
        float experiencia = (baseExp * rivalLevel) / 7;

        /*// Aplicar modificadores
        if (esIntercambiado)
            experiencia *= 1.5f; // +50% por ser Pokémon intercambiado

        if (tieneHuevoSuerte)
            experiencia *= 1.5f; // +50% por Huevo Suerte

        if (isTrainer)
            experiencia *= 1.5f; // +50% por combate contra entrenador*/

        playerUnit.pokemon.Experience += Mathf.FloorToInt(experiencia);
        yield return battleDialogBox.SetDialog($"{playerUnit.pokemon.Base.PokemonName} a ganado {experiencia} de experiencia.");

        yield return LevelUp();
    }

    private IEnumerator LevelUp()
    {
        int currentExp = playerUnit.pokemon.Experience;
        int necessaryExp = playerUnit.pokemon.Base.GetNecessaryExpForLevel(playerUnit.pokemon.Level);
        int necessaryExpToNext = playerUnit.pokemon.Base.GetNecessaryExpForLevel(playerUnit.pokemon.Level + 1);

        if (playerUnit.pokemon.Experience > playerUnit.pokemon.Base.GetNecessaryExpForLevel(playerUnit.pokemon.Level + 1))
        {
            yield return playerUnit.BattleHUD.UpdateExp(necessaryExpToNext, necessaryExp, necessaryExpToNext);
            playerUnit.pokemon.LevelUp();
            playerUnit.BattleHUD.SetPokemonData(playerUnit.pokemon, true);
            if (playerUnit.pokemon.CheckCanLearnNewMovement().Count > 0)
            {
                foreach (var move in playerUnit.pokemon.CheckCanLearnNewMovement())
                {
                    if (playerUnit.pokemon.Moves.Count < 4)
                    {
                        playerUnit.pokemon.LearnMovement(move);
                        yield return battleDialogBox.SetDialog($"{playerUnit.pokemon.Base.PokemonName} a aprendido {move.MoveBase.MoveName}.");
                    }
                    else
                    {
                        yield return battleDialogBox.SetDialog($"{playerUnit.pokemon.Base.PokemonName} intenta aprender {move.MoveBase.MoveName}.");
                        yield return battleDialogBox.SetDialog($"Pero {playerUnit.pokemon.Base.PokemonName} ya conoce cuatro movimientos.");
                    }
                }
            }
            yield return new WaitForSeconds(1);
            yield return LevelUp();
        }
        else
        {
            yield return playerUnit.BattleHUD.UpdateExp(currentExp, necessaryExp, necessaryExpToNext);
            yield return new WaitForSeconds(1);
        }
        
    }

}
