using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pokemonName;
    [SerializeField] private TextMeshProUGUI pokemonLevel;
    [SerializeField] private HealtBar healtBar;

    public void SetPokemonData(Pokemon pokemon)
    {
        pokemonName.text = pokemon.Base.PokemonName;
        pokemonLevel.text = $"lv {pokemon.Level}";
        healtBar.SetHP(pokemon.HP, pokemon.maxHP);
    }

    public IEnumerator UpdateData(int hp)
    {
        yield return StartCoroutine(healtBar.UpdateHealt(hp));
    }

}
