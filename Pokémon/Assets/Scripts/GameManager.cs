using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum GameState {Travel, Battle }
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private GameObject mainCamera;

    private GameState gameState;

    private void Awake()
    {
        Application.targetFrameRate = 120;
        gameState = GameState.Travel;
    }
    private void Start()
    {
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnFinishedBattle += FinishPokemonBattle;
    }

    private void FinishPokemonBattle(bool hasWon)
    {
        gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        mainCamera.SetActive(true);
    }

    private void StartPokemonBattle()
    {
        gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        mainCamera.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemonBase = FindObjectsOfType<PokemonMapArea>();
        var wildPokemon = new Pokemon(wildPokemonBase[0].GetRandomPokemon(), playerParty.Pokemons[0].Level);
        battleManager.HandleStartBattle(playerParty, wildPokemon);
    }

    private void Update()
    {
        if(gameState == GameState.Travel)
        {
            playerController.HandleUpdate();
        }
        else if(gameState == GameState.Battle)
        {
            //battleManager.HandleUpdate();
        }
    }
}
