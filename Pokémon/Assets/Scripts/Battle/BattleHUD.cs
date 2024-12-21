using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pokemonName;
    [SerializeField] private TextMeshProUGUI pokemonLevel;
    [SerializeField] private HealtBar healtBar;
    [SerializeField] private ExpBar expBar;

    public void SetPokemonData(Pokemon pokemon)
    {
        pokemonName.text = pokemon.Base.PokemonName;
        pokemonLevel.text = $"lv {pokemon.Level}";
        healtBar.SetHP(pokemon.HP, pokemon.maxHP);
        if(expBar != null)expBar.SetExp(pokemon.Experience,
            pokemon.Base.GetNecessaryExpForNextLevel(pokemon.Level),
            pokemon.Base.GetNecessaryExpForNextLevel(pokemon.Level + 1));
    }

    public IEnumerator UpdateHP(int hp)
    {
        yield return StartCoroutine(healtBar.UpdateHealt(hp));
    }

    public IEnumerator UpdateExp(Pokemon pokemon)
    {
        yield return StartCoroutine(expBar.UpdateExp(pokemon.Experience, 
            pokemon.Base.GetNecessaryExpForNextLevel(pokemon.Level), 
            pokemon.Base.GetNecessaryExpForNextLevel(pokemon.Level + 1)));
    }

}
