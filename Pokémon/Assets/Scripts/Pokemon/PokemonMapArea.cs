using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMapArea : MonoBehaviour
{
    [SerializeField] private List<PokemonBase> wildPokemon;
    [SerializeField] private int minLevel = 2;
    [SerializeField] private int maxLevel = 15;

    public PokemonBase GetRandomPokemon()
    {
        return wildPokemon[Random.Range(0, wildPokemon.Count)];
    }

    public int GetRandomLevel()
    {
        return Random.Range(minLevel, maxLevel+1);
    }
}
