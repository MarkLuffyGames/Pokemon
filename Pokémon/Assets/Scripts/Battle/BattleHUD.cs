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

    public void SetPokemonData(Pokemon pokemon, bool levelUp = false)
    {
        pokemonName.text = pokemon.Base.PokemonName;
        pokemonLevel.text = $"lv {pokemon.Level}";
        healtBar.SetHP(pokemon.HP, pokemon.maxHP);
        if(expBar != null)expBar.SetExp(levelUp ? pokemon.Base.GetNecessaryExpForLevel(pokemon.Level):pokemon.Experience,
            pokemon.Base.GetNecessaryExpForLevel(pokemon.Level),
            pokemon.Base.GetNecessaryExpForLevel(pokemon.Level + 1));
    }

    public IEnumerator UpdateHP(int hp)
    {
        yield return StartCoroutine(healtBar.UpdateHealt(hp));
    }

    public IEnumerator UpdateExp(Pokemon pokemon)
    {
        int currentExp = pokemon.Experience;
        int necessaryExp = pokemon.Base.GetNecessaryExpForLevel(pokemon.Level);
        int necessaryExpToNext = pokemon.Base.GetNecessaryExpForLevel(pokemon.Level + 1);

        if (currentExp > necessaryExpToNext)
        {
            yield return StartCoroutine(expBar.UpdateExp(necessaryExpToNext, necessaryExp, necessaryExpToNext));
            pokemon.LevelUp();
            SetPokemonData(pokemon, true);
            yield return new WaitForSeconds(1);
            yield return UpdateExp(pokemon);
        }
        else
        {
            yield return StartCoroutine(expBar.UpdateExp(currentExp, necessaryExp, necessaryExpToNext));
        }

        yield return new WaitForSeconds(1);
    }

}
