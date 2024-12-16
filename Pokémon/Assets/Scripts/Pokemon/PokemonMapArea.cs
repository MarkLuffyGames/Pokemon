using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMapArea : MonoBehaviour
{
    [SerializeField] private List<PokemonBase> wildPokemon;

    public PokemonBase GetRandomPokemon()
    {
        return wildPokemon[Random.Range(0, wildPokemon.Count)];
    }
}
